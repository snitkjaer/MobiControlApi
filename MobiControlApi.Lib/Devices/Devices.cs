using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MobiControlApi
{
    public partial class Api : LogAbstraction
    {
        // Get device list for specific group using /device MC 13+ API (reads directly from SOTI SQL DB)
        private async Task<string> GetDeviceListJsonFromSotiDbAsync(string deviceGroupPath, int skip, int take)
        {
            deviceGroupPath = EncodeUrl(deviceGroupPath);

            // Generate resourcePath
            string resourcePath = "devices?path=%255C" + deviceGroupPath; // "MARK%255CZebra%2520TC56";

            resourcePath +=
                "&skip=" + skip.ToString()
                + "&take=" + take.ToString();

            // Call GetJsonAsync
            return await GetJsonAsync(resourcePath);

        }

        // Get device list for specific group using /device/search MC 14+ API (reads elasticsearch DB)
        public async Task<string> GetDeviceListJsonSearchDbAsync(string deviceGroupPath, string filter, bool includeSubgroups, bool verifyAndSync, int skip, int take)
        {

            // Generate resourcePath
            string resourcePath = GenerateSeachDbResourcePath(deviceGroupPath, filter, includeSubgroups, verifyAndSync, skip, take);

            // Call GetJsonAsync
            return await GetJsonAsync(resourcePath);


        }

        // devices/search?&includeSubgroups=true&skip=0&take=1000"
        // devices/search?groupPath=%255C%255CTest&includeSubgroups=true&skip=0&take=1000"
        public string GenerateSeachDbResourcePath(string deviceGroupPath, string filter, bool includeSubgroups, bool verifyAndSync, int skip, int take)
        {
            // search db
            string resourcePath = "devices/search?";

            // groupPath
            // remove any leading forward slashes
            deviceGroupPath = deviceGroupPath.TrimStart('/');
            if (String.IsNullOrEmpty(deviceGroupPath))
                resourcePath += "";
            else
                resourcePath += "&groupPath=" + EncodeUrl("//" + deviceGroupPath);

            // filter
            if (!String.IsNullOrEmpty(filter))
                resourcePath += "&filter=" + EncodeUrl(filter);

            // includeSubgroups
            resourcePath += "&includeSubgroups=" + includeSubgroups.ToString().ToLower();

            // skip & take
            resourcePath +=
                "&skip=" + skip.ToString()
                + "&take=" + take.ToString();


            return resourcePath;
        }

        public string EncodeUrl(string url)
        {
            return url
                .Replace("/", "%255C")
                .Replace(" ", "%2520")
                .Replace("=", "%3D")
                .Replace("\"", "%2527")
                ;
        }



        /*

        // Get list of device id's
        private async Task<List<string>> GetDeviceIdListFromSotiAsync(string deviceGroupPath, bool includeSubgroups)
        {
            List<string> listDeviceIds = new List<string>();

            int deviceOffset = 0;
            int deviceBatchSize = 50;
            string resultJson = null;

            while (true)
            {
                // Get devices in SOTI folder
                if (useSearchDbToGetDevices)
                    resultJson = await GetDeviceListJsonSearchDbAsync(deviceGroupPath, null, includeSubgroups, true, deviceOffset, deviceOffset + deviceBatchSize);
                else
                    resultJson = await GetDeviceListJsonFromSotiDbAsync(deviceGroupPath, deviceOffset, deviceOffset + deviceBatchSize);

                // If we got a result - parse it
                if (resultJson != null)
                {
                    // String to json array
                    JArray devices = JArray.Parse(resultJson);

                    // Itterate over device found
                    foreach (JObject device in devices)
                    {
                        // deviceId
                        string deviceId = (string)device["DeviceId"];

                        if (!String.IsNullOrEmpty(deviceId))
                            listDeviceIds.Add(deviceId);
                    }

                    // Do we expect more devices?
                    if (devices.Count == deviceBatchSize)
                        deviceOffset += deviceBatchSize;
                    else
                        break;
                }
                else
                    break;


            }

            return listDeviceIds;
        }
        */

        // Get list of devices
        public async Task<List<Device>> GetDeviceListAsync(string deviceGroupPath, bool includeSubgroups)
        {
            if (CacheDevices)
                return await GetCacheDeviceListAsync(deviceGroupPath, includeSubgroups);
            else
                return await GetDeviceListFromSotiAsync(deviceGroupPath, includeSubgroups);
        }


        // Get list of devices
        public async Task<List<Device>> GetDeviceListFromSotiAsync(string deviceGroupPath, bool includeSubgroups)
        {

            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

            List<Device> listDevices = new List<Device>();


            string resultJson = null;

            try
            {
                stopWatch.Start();

                // Get devices
                resultJson = await GetDeviceListJsonSearchDbAsync(deviceGroupPath, null, includeSubgroups, false, 0, 1000);


                // If we got a result - parse it
                if (resultJson != null)
                {
                    // String to json array
                    JArray devices = JArray.Parse(resultJson);

                    // Itterate over device found
                    foreach (JObject deviceJson in devices)
                    {

                        // parse device
                        Device device = ParseDeviceJson(deviceJson.ToString());

                        if (device != null)
                            listDevices.Add(device);
                    }
                }

                // Stop
                stopWatch.Stop();


                var properties = new Dictionary<string, string>
                             {
                                 { "GroupPath", deviceGroupPath },
                                 { "CurrentDeviceCount",listDevices.Count.ToString()}
                             };
                TrackEvent("SotiGetGroupDeviceList", stopWatch.Elapsed, properties);

            }
            catch (Exception ex)
            {
                // Stop
                stopWatch.Stop();

                Log("Exception getting device list for '" + deviceGroupPath + "' in " + stopWatch.Elapsed.ToString(), SeverityLevel.Error);
                TrackException(ex);
            }


            return listDevices;
        }


        // Get list of devices
        public async Task<List<Device>> OldGetDeviceListFromSotiAsync(string deviceGroupPath, bool includeSubgroups)
        {

                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

                List<Device> listDevices = new List<Device>();

                int deviceOffset = 0;
                int deviceBatchSize = 50;
                string resultJson = null;

                try
                {
                    stopWatch.Start();

                    // SOTI API only returns a batch of 50 devices - continue to itterate over device batche
                    while (true)
                    {
                        // Get devices in SOTI folder
                        if (useSearchDbToGetDevices)
                            resultJson = await GetDeviceListJsonSearchDbAsync(deviceGroupPath, null, includeSubgroups, true, deviceOffset, deviceOffset + deviceBatchSize);
                        else
                            resultJson = await GetDeviceListJsonFromSotiDbAsync(deviceGroupPath, deviceOffset, deviceOffset + deviceBatchSize);


                        // If we got a result - parse it
                        if (resultJson != null)
                        {
                            // String to json array
                            JArray devices = JArray.Parse(resultJson);

                            // Itterate over device found
                            foreach (JObject deviceJson in devices)
                            {

                                // parse device
                                Device device = ParseDeviceJson(deviceJson.ToString());

                                if (device != null)
                                    listDevices.Add(device);
                            }

                            // Do we expect more devices?
                            if (devices.Count == deviceBatchSize)
                                deviceOffset += deviceBatchSize;
                            else
                                break;
                        }
                        else
                            break;


                    }

                    // Stop
                    stopWatch.Stop();


                    var properties = new Dictionary<string, string>
                             {
                                 { "GroupPath", deviceGroupPath },
                                 { "CurrentDeviceCount",listDevices.Count.ToString()}
                             };
                    TrackEvent("SotiGetGroupDeviceList", stopWatch.Elapsed, properties);

                }
                catch (Exception ex)
                {
                    // Stop
                    stopWatch.Stop();

                    Log("Exception getting device list for '" + deviceGroupPath + "' in " + stopWatch.Elapsed.ToString(), SeverityLevel.Error);
                    TrackException(ex);
                }                return listDevices;
        }


        public Device ParseDeviceJson(string deviceJson)
        {
            Device device = null;

            try
            {
                if (!String.IsNullOrWhiteSpace(deviceJson))
                    device = Device.FromJson(deviceJson);
            }
            catch (Exception ex)
            {
                TrackException(ex);
                Log("Exception parsing device json '" + deviceJson, SeverityLevel.Error);
            }


            return device;
        }


        // Get device
        public async Task<Device> GetDeviceAsync(string deviceId)
        {
            // GET /devices/{deviceId}

            // Generate resourcePath
            string resourcePath = "devices/" + deviceId;

            string jsonDevice = await GetJsonAsync(resourcePath);

            Device device = ParseDeviceJson(jsonDevice);

            if(device != null)
            {
                Log("DeviceId " + deviceId + " was found", SeverityLevel.Verbose);
            }

            return device;
        }
    }
}
