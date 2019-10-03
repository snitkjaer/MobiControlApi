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


        static readonly object lockObject = new object();


        Stopwatch stopWatchForceUpdate = new Stopwatch();


        private async Task CheckCacheValidity()
        {
            TimeSpan DeltaTime = DateTime.Now - CacheLastUpdate;
            TimeSpan DeltaMax = config.tsCacheUpdateInterval + config.tsMaxSotiResponseTime;
            if (DeltaTime > DeltaMax)
            {
                if(DeltaTime > config.tsMaxCacheAge)
                {
                    Log("SOTI device cache is above max age - try forcing update", SeverityLevel.Error);

                    // This could be called by multiple threads
                    lock (lockObject)
                    {
                        // If some other thread has already started updating the cache
                        if (stopWatchForceUpdate.IsRunning && stopWatchForceUpdate.Elapsed < config.tsMaxSotiResponseTime)
                        {
                            // Skip update - some other thread is already doing it
                            Log("SOTI device list cache force update in progress - skipping", SeverityLevel.Information);
                            return;
                        }
                        else
                        {
                            // Reset and start timer
                            stopWatchForceUpdate.Stop();
                            stopWatchForceUpdate.Reset();
                            stopWatchForceUpdate.Start();
                        }
                    }

                    await UpdateCachedDeviceList();
                    stopWatchForceUpdate.Stop();

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
                await UpdateCachedDeviceList();

                // Wait before updating again
                if (!cancellationToken.IsCancellationRequested)
                    await Task.Delay(config.tsCacheUpdateInterval, cancellationToken);
            }

        }

        object lockObjectCacheUpdate = new object();

        private async Task UpdateCachedDeviceList()
        {

            if (Monitor.TryEnter(lockObjectCacheUpdate, 5000))            {                try                {
                    // Place code protected by the Monitor here.  
                    // Update list Cache Devices
                    useSearchDbToGetDevices = true;
                    List<Device> deviceList = await GetDeviceListFromSotiAsync("/", true);
                    CacheLastUpdate = DateTime.Now;
                    listCacheDevices = deviceList;
                }
                catch (Exception ex)
                {
                    Log("Exception updating SOTI cached device list", SeverityLevel.Error);
                    TrackException(ex);
                }                finally                {                    Monitor.Exit(lockObjectCacheUpdate);                }            }            else            {
                // Code to execute if the attempt times out.  
                Log("Timeout waiting for UpdateCachedDeviceList to free up from another thread ", SeverityLevel.Error);            }


        }

    }
}
