using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace MobiControlApi
{
    public class Api
    {
		MobiControlApiConfig config;

		private static readonly HttpClient httpClient = new HttpClient();
		Authentication authentication;

        public Api(MobiControlApiConfig mobiControlApiConfig)
        {
            // Create config object
            config = mobiControlApiConfig;

            // Create Authentication object
            authentication = new Authentication(config);

        }

        public Api(string FQDN, string ClientId, string ClientSecret, string Username, string Password)
        {         
            // Create config object
			config = new MobiControlApiConfig(FQDN, ClientId, ClientSecret, Username, Password);

			// Create Authentication object
			authentication = new Authentication(config);
           
        }      

		public Api(JObject jsonConfig)
        {
			// Create config object
			config = MobiControlApiConfig.GetConfigFromJObject(jsonConfig);

            // Create Authentication object
            authentication = new Authentication(config);
           
        }  

		public Api(string jsonConfig)
        {
            // Create config object
			config = MobiControlApiConfig.GetConfigFromJsonString(jsonConfig);

            // Create Authentication object
            authentication = new Authentication(config);
           
        }

		// This is the lowest level of the API - accepting raw resource path and retruning a json string
        #region low level SOTI REST Web API 
		public async Task<string> GetJsonAsync(string resourcePath, CancellationToken cancellationToken)
        {         
			string token = await authentication.GetAuthenticationToken();
			if (token == null)
                return null;

			httpClient.DefaultRequestHeaders.Clear();
			httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    
            // Create http request 
			var request = new HttpRequestMessage
            {
				Method = HttpMethod.Get,
				RequestUri = new Uri(config.baseUri, resourcePath)
            };            

            // Send http request
            HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)            
				return await response.Content.ReadAsStringAsync();
			else
			    return null;
        }

        
		public async Task<bool> PostJsonAsync(string resourcePath, string body)
        {
            string token = await authentication.GetAuthenticationToken();
            if (token == null)
                return false;

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var request = new HttpRequestMessage
            {
				Method = HttpMethod.Post,
                RequestUri = new Uri(config.baseUri, resourcePath),
				Content = new StringContent(body),
                
                
            };
			request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json") { CharSet = "UTF-8" };

            HttpResponseMessage response = await httpClient.SendAsync(request);
			if (response.IsSuccessStatusCode)
				return true;
            else
                return false;
        }

        public async Task<bool> PutJsonAsync(string resourcePath, string body)
        {
            string token = await authentication.GetAuthenticationToken();
            if (token == null)
                return false;

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(config.baseUri, resourcePath),
                Content = new StringContent(body),


            };
            request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json") { CharSet = "UTF-8" };

            HttpResponseMessage response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return true;
            else
                return false;
        }


        #endregion

        // Get device list for specific group using /device MC 13+ API (reads directly from SOTI SQL DB)
        private async Task<string> GetDeviceListJsonFromSotiDbAsync(string deviceGroupPath, int skip, int take, CancellationToken cancellationToken)
        {

			deviceGroupPath = deviceGroupPath.Replace(" ", "%2520").Replace("/", "%255C");

			// Generate resourcePath
			string resourcePath = "devices?path=%255C" + deviceGroupPath; // "MARK%255CZebra%2520TC56";

            resourcePath +=
                "&skip=" + skip.ToString()
                + "&take=" + take.ToString();

            // Call GetJsonAsync
            return await GetJsonAsync(resourcePath, cancellationToken);

           
        }

        // Get device list for specific group using /device/search MC 14+ API (reads eleatic search DB)
        private async Task<string> GetDeviceListJsonSearchDbAsync(string deviceGroupPath, string filter, bool includeSubgroups, bool verifyAndSync, int skip, int take, CancellationToken cancellationToken)
        {

            // Generate resourcePath
            //string resourcePath = "devices/search?path=%255C" + deviceGroupPath.Replace(" ", "%2520").Replace("/", "%255C");

            string resourcePath = "devices/search?path=" + deviceGroupPath;

            if (!String.IsNullOrEmpty(filter))
                resourcePath += "&filter=" + filter.Replace(" ", "%2520").Replace("/", "%255C");

           // if (includeSubgroups)
            
            resourcePath += "&includeSubgroups="+ includeSubgroups.ToString().ToLower();

            resourcePath += 
                "&skip=" + skip.ToString()
                + "&take=" + take.ToString();



            // Call GetJsonAsync
            return await GetJsonAsync(resourcePath, cancellationToken);

        }

        public bool useSearchDbToGetDevices = false;

        public async Task<List<string>> GetDeviceIdListAsync(string deviceGroupPath, bool includeSubgroups, CancellationToken cancellationToken)
        {
            List<string> listDeviceIds = new List<string>();

            int deviceOffset = 0;
            int deviceBatchSize = 50;
            string resultJson = null;

            while (true)
            {
                // Get devices in SOTI folder
                if(useSearchDbToGetDevices)
                    resultJson = await GetDeviceListJsonSearchDbAsync(deviceGroupPath, null, includeSubgroups, true, deviceOffset, deviceOffset + deviceBatchSize, cancellationToken);
                else
                    resultJson = await GetDeviceListJsonFromSotiDbAsync(deviceGroupPath, deviceOffset, deviceOffset + deviceBatchSize, cancellationToken);

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

        /*
        public async Task<Dictionary<string, JObject>> GetDeviceIdJsonDictAync(string deviceGroupPath, bool includeSubgroups, CancellationToken cancellationToken)
        {
            Dictionary<string, JObject> dirDevices = new Dictionary<string, JObject>();

            int deviceOffset = 0;
            int deviceBatchSize = 50;

            while (true)
            {
                // Get devices in SOTI folder
                string resultJson = await GetDeviceListJsonAsync2(deviceGroupPath, null, includeSubgroups, true, deviceOffset, deviceOffset + deviceBatchSize, cancellationToken);

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
                            dirDevices.Add(deviceId, device);
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

            return dirDevices;
        }
        */

        public async Task<List<Device>> GetDeviceListAsync(string deviceGroupPath, CancellationToken cancellationToken)
        {
            List<Device> listDevices = new List<Device>();

            int deviceOffset = 0;
            int deviceBatchSize = 50;
            string resultJson = null;

            while (true)
            {
                // Get devices in SOTI folder
                // Get devices in SOTI folder
                if (useSearchDbToGetDevices)
                    resultJson = await GetDeviceListJsonSearchDbAsync(deviceGroupPath, null, false, true, deviceOffset, deviceOffset + deviceBatchSize, cancellationToken);
                else
                    resultJson = await GetDeviceListJsonFromSotiDbAsync(deviceGroupPath, deviceOffset, deviceOffset + deviceBatchSize, cancellationToken);


                // If we got a result - parse it
                if (resultJson != null)
                {
                    // String to json array
                    JArray devices = JArray.Parse(resultJson);

                    // Itterate over device found
                    foreach (JObject deviceJson in devices)
                    {
                        // deviceId
                        Device device = Device.FromJson(deviceJson.ToString());

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

            return listDevices;
        }



        public async Task<bool> SendActionToDevicesAsync(string deviceId,string Action, string Message)
        {
			// POST /devices/actions/DeviceIds

			List<string> deviceIds = new List<string>();
			deviceIds.Add(deviceId);
            
			ActionBody actionBody = new ActionBody(deviceIds, new ActionInfo(Action, Message));

            // Generate resourcePath
			string resourcePath = "devices/actions";
			string actionBodyJson = actionBody.ToJsonString();
            // Call GetJsonAsync
			return await PostJsonAsync(resourcePath, actionBodyJson);
        }

        public async Task<string> GetCustomAttributesAsync(string deviceId, CancellationToken cancellationToken)
        {
            // GET /devices/{deviceId}/customAttributes

            List<string> deviceIds = new List<string>();
            deviceIds.Add(deviceId);

            // Generate resourcePath
            string resourcePath = "devices/" + deviceId + "/customAttributes";

            // Call GetJsonAsync
            return await GetJsonAsync(resourcePath, cancellationToken);
        }


        public async Task<bool> SetCustomAttributeAsync(string deviceId, string customAttributeId, string customAttributeValue)
        {
            // PUT /devices/{deviceId}/customAttributes/{customAttributeId}

            List<string> deviceIds = new List<string>();
            deviceIds.Add(deviceId);

            // Generate resourcePath
            string resourcePath = "devices/" + deviceId + "/customAttributes/" + customAttributeId;
            string CustomAttributeBodyJson = new CustomAttributeBody(customAttributeValue).ToJsonString();
            // Call GetJsonAsync
            return await PutJsonAsync(resourcePath, CustomAttributeBodyJson);
        }




    }


    public class CustomAttributeBody
    {
        public string customAttributeValue;

        public CustomAttributeBody(string customAttributeValue)
        {
            this.customAttributeValue = customAttributeValue;
        }

        public string ToJsonString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }


    public class ActionBody
    {
		public List<string> DeviceIds;
		public ActionInfo ActionInfo;

		public ActionBody(List<string> deviceIds, ActionInfo actionInfo)
		{
			DeviceIds = deviceIds;
			ActionInfo = actionInfo;
		}

		public string ToJsonString()
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(this);
		}

	}
   
	public class ActionInfo
    {
		public string Action;
		public string Message;

		public ActionInfo(string action, string message)
		{
			Action = action;
			Message = message;
		}
	}
}
