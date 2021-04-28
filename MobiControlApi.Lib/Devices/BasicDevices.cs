using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.ApplicationInsights.DataContracts;
using System.Threading;
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

        // Get basic device
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

    }
}