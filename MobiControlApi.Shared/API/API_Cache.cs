using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;

namespace MobiControlApi
{
    public partial class Api: LogAbstraction
    {
        // I many different folders are individually monitored it can create a lot of calls to SOTI API
        // To minimize this a device cache can be enabled, which will only be update when used and it out of date
        public bool CacheDevices
        {
            get
            {
                return config.CacheDevices;
            }
        }

        List<Device> listCacheDevices;
        DateTime CacheLastUpdate = DateTime.MinValue;



        // Get list of devices
        public async Task<List<Device>> GetCacheDeviceListAsync(string deviceGroupPath, bool includeSubgroups)
        {
            List<Device> returnList;

            // Check cache validity
            await CheckCacheValidity();

            // Make sure deviceGroupPath match value returned by SOTI API
            deviceGroupPath = deviceGroupPath.TrimStart('/').Replace('/','\\');

            // Do we need to include devices in sub groups
            if(!includeSubgroups)
            {
                // No sub group
                returnList = listCacheDevices
                    .Where((d) => d.Path.TrimStart('\\') == deviceGroupPath)
                    .ToList();
            }
            else
            {
                // Include devices in sub group
                returnList = listCacheDevices
                    .Where((d) => d.Path.TrimStart('\\').StartsWith(deviceGroupPath, true, System.Globalization.CultureInfo.InvariantCulture))
                    .ToList();
            }

            return returnList;
        }

        /*
        // Get list of devices
        public async Task<List<string>> GetCacheDeviceIDListAsync(string deviceGroupPath, bool includeSubgroups)
        {
            // Update listCacheDevices if needed
            List<Device> deviceList = await GetCacheDeviceListAsync(deviceGroupPath, includeSubgroups);
            List<string> deviceIdList = deviceList
                .Select((arg) => arg.DeviceId)
                .ToList();

            return deviceIdList;

        }
        */

        


        Stopwatch stopWatchForceUpdate = new Stopwatch();


        private async Task CheckCacheValidity()
        {
            TimeSpan DeltaTime = DateTime.Now - CacheLastUpdate;
            TimeSpan DeltaMax = config.tsCacheUpdateInterval + config.tsMaxSotiResponseTime;
            if (DeltaTime > DeltaMax)
            {
                if(DeltaTime > config.tsMaxCacheAge)
                {
                    // If we are just starting cache will alway be expired but listCacheDevices will be null
                    if (listCacheDevices != null)
                        Log("SOTI device cache is above max age - try forcing update", SeverityLevel.Error);

                    // Call update cache
                    await UpdateCachedDeviceList();
                }
                else
                   Log("SOTI device cache is out of date but not above max age", SeverityLevel.Warning);
            }




        }


        private readonly System.Threading.EventWaitHandle waitHandleRequestUpdate = new System.Threading.AutoResetEvent(false);

        private async Task UpdateCachedDeviceListOnInterval()
        {

            while (!cancellationToken.IsCancellationRequested)
            {
                // Call update cache
                await UpdateCachedDeviceList();

                // Wait before updating again
                if (!cancellationToken.IsCancellationRequested)
                    await Task.Delay(config.tsCacheUpdateInterval, cancellationToken);


            }

        }


        //static readonly object lockObjectUpdateCachedDeviceList = new object();

        //Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        private async Task UpdateCachedDeviceList()
        {
            //Asynchronously wait to enter the Semaphore. If no-one has been granted access to the Semaphore, code execution will proceed, otherwise this thread waits here until the semaphore is released 
            await semaphoreSlim.WaitAsync(cancellationToken);
            try
            {
                // if closing down
                if (cancellationToken.IsCancellationRequested)
                    return;

                List<Device> deviceList = await GetDeviceListFromSotiAsync("/", true);
                CacheLastUpdate = DateTime.Now;
                listCacheDevices = deviceList;
            }
            catch (Exception ex)
            {
                Log("Exception updating SOTI cached device list", SeverityLevel.Error);
                TrackException(ex);
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                semaphoreSlim.Release();
            }

        }

    }
}
