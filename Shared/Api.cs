using System;
using System.Threading.Tasks;
using System.Net.Http;

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

		public Api(string jsonConfig)
        {
			// Create config object
			config = Config.GetConfig(jsonConfig);

            // Create Authentication object
            authentication = new Authentication(config);
           
        }  

		public async Task<string> GetJsonAsync(string resourcePath)
        {         
			string token = await authentication.GetAuthenticationHeader();
			if (token == null)
                return null;
                    
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
              
    }
}
