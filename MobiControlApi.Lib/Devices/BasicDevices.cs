using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace MobiControlApi
{
    public partial class Api : LogAbstraction
    {
        // Get device list for specific group using /device/search MC 14+ API (reads elasticsearch DB)
        public async Task<List<BasicDevice>> GetBasicDeviceListJsonSearchDbAsync(string deviceGroupPath, string filter, bool includeSubgroups, bool verifyAndSync, int skip, int take)
        {

            // 
            string resultJson = await GetDeviceListJsonSearchDbAsync(deviceGroupPath, filter, includeSubgroups, verifyAndSync, skip, take);


            List<BasicDevice> listDevices = new List<BasicDevice>();

            // If we got a result - parse it
            if (resultJson != null)
            {
                // String to json array
                JArray devices = JArray.Parse(resultJson);

                // Itterate over device found
                foreach (JObject deviceJson in devices)
                {
                    BasicDevice basicDevice = ParseBasicDeviceJson(deviceJson.ToString());

                    if (basicDevice != null)
                        listDevices.Add(basicDevice);
                }


            }

            Log("Found " + listDevices.Count.ToString() + " devices in group '" + deviceGroupPath + "' with filter '" + filter + "'", SeverityLevel.Verbose);

            return listDevices;

        }


        // Get list of devices
        public async Task<List<BasicDevice>> GetBasicDeviceListFromSotiAsync(string deviceGroupPath, bool includeSubgroups)
        {

            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

            List<BasicDevice> listDevices = new List<BasicDevice>();


            try
            {
                stopWatch.Start();

                // Get devices
                listDevices = await GetBasicDeviceListJsonSearchDbAsync(deviceGroupPath, null, includeSubgroups, false, 0, 1000);


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

                Log("Exception getting basic device list for '" + deviceGroupPath + "' in " + stopWatch.Elapsed.ToString(), SeverityLevel.Error);
                TrackException(ex);
            }


            return listDevices;

        }


        public BasicDevice ParseBasicDeviceJson(string deviceJson)
        {
            BasicDevice basicDevice = null;

            try
            {
                if (deviceJson != null)
                    basicDevice = BasicDevice.FromJson(deviceJson);
            }
            catch (Exception ex)
            {
                TrackException(ex);
                Log("Exception parsing basic device json '" + deviceJson.ToString() + "'", SeverityLevel.Error);
            }


            return basicDevice;
        }

        // Get basic device on device id
        public async Task<BasicDevice> GetBasicDeviceAsync(string deviceId)
        {
            // GET /devices/{deviceId}

            // Generate resourcePath
            string resourcePath = "devices/" + deviceId;

            string jsonDevice = await GetJsonAsync(resourcePath);

            BasicDevice device = ParseBasicDeviceJson(jsonDevice);

            if (device != null)
            {
                Log("DeviceId " + deviceId + " was found", SeverityLevel.Verbose);
            }

            return device;
        }

        // Get basic device on imei
        public async Task<BasicDevice> GetBasicDeviceOnImeiAsync(string deviceImei)
        {
            // GET search=IMEI_MEID_ESN%20Equal%20"353857081861581"&subGroups=true&count=250
            BasicDevice basicDevice = null;


            if(!String.IsNullOrWhiteSpace(deviceImei))
            {
                // This does not seem to work anymore
                //var devices = await GetBasicDeviceListJsonSearchDbAsync("/", "IMEI_MEID_ESN=\"" + deviceImei + "\"", true, false, 0, 1);
                var basicDevices = await GetBasicDeviceListFromSotiAsync("/", true);

                var devices = basicDevices.Where(d => d.ImeiMeidEsn == deviceImei).ToList();

                if (devices.Count == 1)
                    basicDevice = devices.First();

                if (basicDevice == null)
                {
                    Log("Device Imei " + deviceImei + " was not found", SeverityLevel.Warning);
                }
            }

            return basicDevice;
        }

    }
}