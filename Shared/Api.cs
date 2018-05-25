using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace MobiControlApi.Shared
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
		public async Task<string> GetJsonAsync(string resourcePath)
        {         
			string token = await authentication.GetAuthenticationToken();
			if (token == null)
                return null;

			httpClient.DefaultRequestHeaders.Clear();
			httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                     
			var request = new HttpRequestMessage
            {
				Method = HttpMethod.Get,
				RequestUri = new Uri(config.baseUri, resourcePath)
            };            
            
            HttpResponseMessage response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)            
				return await response.Content.ReadAsStringAsync();
			else
			    return null;
        }

        /*
		public async Task<string> PutJsonAsync(string resourcePath)
        {
            string token = await authentication.GetAuthenticationToken();
            if (token == null)
                return null;

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(config.baseUri, resourcePath)
            };

            HttpResponseMessage response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
                return null;
        }
        */


        #endregion

		public async Task<string> GetDeviceListJsonAsync(string deviceGroupPath)
        {

			deviceGroupPath = deviceGroupPath.Replace(" ", "%2520").Replace("/", "%255C");

			// Generate resourcePath
			string resourcePath = "devices?path=%255C" + deviceGroupPath; // "MARK%255CZebra%2520TC56";

			// Call GetJsonAsync
			return await GetJsonAsync(resourcePath);

           
        }
              
    }
}
