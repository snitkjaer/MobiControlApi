using System;
using System.Threading.Tasks;
using System.Net.Http;

namespace MobiControlApi.Shared
{
    public class Api
    {
        Uri baseUri;
		private static readonly HttpClient httpClient = new HttpClient();
		Authentication authentication;

		public Api(string FQDN, string ClientId, string ClientSecret, string User, string Password)
        {
			this.baseUri = new Uri("https://" + FQDN + "/MobiControl/api/");
           
			// Create Authentication object
			authentication = new Authentication(FQDN, ClientId, ClientSecret, User, Password);
           
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
				RequestUri = new Uri(baseUri, resourcePath)
            };            

            HttpResponseMessage response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)            
				return await response.Content.ReadAsStringAsync();
			else
			    return null;
        }
              
    }
}
