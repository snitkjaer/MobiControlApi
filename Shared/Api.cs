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
		Config config;

		private static readonly HttpClient httpClient = new HttpClient();
		Authentication authentication;

		public Api(string FQDN, string ClientId, string ClientSecret, string Username, string Password)
        {         
            // Create config object
			config = new Config(FQDN, ClientId, ClientSecret, Username, Password);

			// Create Authentication object
			authentication = new Authentication(config);
           
        }      

		public Api(JObject jsonConfig)
        {
			// Create config object
			config = Config.GetConfigFromJObject(jsonConfig);

            // Create Authentication object
            authentication = new Authentication(config);
           
        }  

		public Api(string jsonConfig)
        {
            // Create config object
			config = Config.GetConfigFromJsonString(jsonConfig);

            // Create Authentication object
            authentication = new Authentication(config);
           
        }

		// This is the lowest level of the API - accepting raw resource path and retruning a json string
        #region low level API 
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
        

        #endregion

		public async Task<string> GetDeviceListJsonAsync(string deviceGroupPath, CancellationToken cancellationToken)
        {

			deviceGroupPath = deviceGroupPath.Replace(" ", "%2520").Replace("/", "%255C");

			// Generate resourcePath
			string resourcePath = "devices?path=%255C" + deviceGroupPath; // "MARK%255CZebra%2520TC56";

			// Call GetJsonAsync
			return await GetJsonAsync(resourcePath, cancellationToken);

           
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
