using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace MobiControlApi
{
	/*
	 * Token request
	 * 
        POST https://server.domain.tld/MobiControl/api/token HTTP/1.1
        Host: server.domain.tld
        Authorization: Basic QXBwbGljYXRpb24xOkFwcGxpY2F0aW9uMVBhc3N3b3Jk
        Content-Type: application/x-www-form-urlencoded
        Content-Length: 53

        grant_type=password&username=Administrator&password=1 
    */

	public class Authentication
	{
		Uri baseUri;
		MobiControlApiConfig config;
		string grant_type;
	    string Token;
		DateTime TokenExpireTime;

        // HttpClient used for authentication requests
		private static readonly HttpClient httpClient = new HttpClient();

        // Constructor
		public Authentication(MobiControlApiConfig config)
        {
			this.baseUri = new Uri("https://" + config.FQDN);
           
            // Save config
			this.config = config;
            
            //grant_type=password&username=Administrator&password=1
            grant_type = "grant_type=password&username=" + config.Username + "&password=" + config.Password;
            
        }
        
		// Get authentication header
		public async Task<string> GetAuthenticationToken()
        {
			ValidateToken();
            
            // If Token is null or expired
            if (Token == null)
            {
				// Request new token
				if (!await RequestToken())
					return null;             
            }
			return Token;       
        }

		private void ValidateToken()
		{
			try
			{
				if (Token != null)
				{
					if (TokenExpireTime == null)
					{
						Token = null;
					}
					else
					{
						if (DateTime.Now > TokenExpireTime)
                            Token = null;
					}               
				}
			}
			catch
			{
				Token = null;
			}
		}

        private async Task<bool> RequestToken()
        {
            try
            {
                var requestToken = new HttpRequestMessage
				{
                    Method = HttpMethod.Post,
					RequestUri = new Uri(baseUri, "/MobiControl/api/token"),
                    Content = new StringContent(grant_type)
                };

                requestToken.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded") { CharSet = "UTF-8" };
                requestToken.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Basic",
                        Convert.ToBase64String(
                            Encoding.ASCII.GetBytes(
                                            string.Format("{0}:{1}", config.ClientId, config.ClientSecret))));

                // try to get token from MobiControl server
                var bearerResult = await httpClient.SendAsync(requestToken);
                var bearerData = await bearerResult.Content.ReadAsStringAsync();
				Token = JObject.Parse(bearerData)["access_token"].ToString();
				int expires_in = Int32.Parse(JObject.Parse(bearerData)["expires_in"].ToString());
				TokenExpireTime = DateTime.Now.AddSeconds(expires_in - 10);
                return true;

            }
            catch (Exception ex)
            {
				Token = null;
                throw ex;
            }


        }
    }
}
