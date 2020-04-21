using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MobiControlApi.Devices
{
    public static class CollectedData
    {
        /*

        // Get device list for specific group using /device/search MC 14+ API (reads elasticsearch DB)
        private async Task<string> GetDeviceListJsonSearchDbAsync(string deviceGroupPath, string filter, bool includeSubgroups, bool verifyAndSync, int skip, int take)
        {

            // Generate resourcePath
            //string resourcePath = "devices/search?path=%255C" + deviceGroupPath.Replace(" ", "%2520").Replace("/", "%255C");

            string resourcePath = "devices/search?path=" + deviceGroupPath;

            if (!String.IsNullOrEmpty(filter))
                resourcePath += "&filter=" + filter.Replace(" ", "%2520").Replace("/", "%255C");

            // if (includeSubgroups)

            resourcePath += "&includeSubgroups=" + includeSubgroups.ToString().ToLower();

            resourcePath +=
                "&skip=" + skip.ToString()
                + "&take=" + take.ToString();

            // Call GetJsonAsync
            return await GetJsonAsync(resourcePath);


        }
        */



        // GET /devices/{deviceId}/collectedData
        // Retrieve Collected Data for a Device


        public static async Task<List<CollectedDataModel>> GetCollectedDataForDeviceAsync(IWebApi webApi, string deviceId, DateTimeOffset startDate, DateTimeOffset stopDate, string builtInDataType, string customDataType)
        {
            List<CollectedDataModel> collectedDateModels;

            string resourcePath = "devices/" + deviceId + "/collectedData?" + Helpers.GetUrlStartStop(startDate, stopDate) + "&builtInDataType=" + builtInDataType + "&customDataType=" + customDataType;

            collectedDateModels = await GetObjectListFromWebApi.GetObjectListFromSotiAsync<CollectedDataModel>(webApi, resourcePath);

            return collectedDateModels;
        }





        // GET /devices/collectedData
        // Retrieve Collected Data in Bulk

        public static async Task<List<CollectedDataModel>> GetCollectedDataAsync(IWebApi webApi, string deviceGroupPath, DateTimeOffset startDate, DateTimeOffset stopDate, string builtInDataType, string customDataType)
        {
            List<CollectedDataModel> collectedDateModels;

            string resourcePath = "devices/collectedData?" + Helpers.GetUrlStartStop(startDate, stopDate);

            if (!String.IsNullOrWhiteSpace(deviceGroupPath))
            {
                resourcePath += "&path=" + deviceGroupPath;
            }

            if (!String.IsNullOrWhiteSpace(builtInDataType))
            {
                resourcePath += "&builtInDataType=" + builtInDataType;
            }

            if (!String.IsNullOrWhiteSpace(customDataType))
            {
                resourcePath += "&customDataType=" + customDataType;
            }

            // Get data from web API
            collectedDateModels = await GetObjectListFromWebApi.GetObjectListFromSotiAsync<CollectedDataModel>(webApi, resourcePath);

            return collectedDateModels;
        }



    }


    // Response Class (Status 200)

    /*
  {
    "$type": "CollectedInteger",
    "Kind": "Integer",
    "Value": 86,
    "DeviceId": "357539090249448",
    "Timestamp": "2019-11-15T10:30:02.653Z",
    "StatTypeId": "BatteryStatus"
  },
 */

    public class CollectedDataModel
    {
        public string Value { get; set; }
        public string DeviceId { get; set; }
        public DateTime Timestamp { get; set; }
        public string StatTypeId { get; set; }
    }

}
