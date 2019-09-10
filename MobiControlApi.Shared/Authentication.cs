using System;
using System.Text;
using System.Threading;
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
        CancellationToken cancellationToken;
        Uri baseUri;
		MobiControlApiConfig config;
		string grant_type;
	    string Token;
		DateTime? TokenExpireTime;
        public TimeSpan httpTimeout = new TimeSpan(0, 0, 20);

        // HttpClient used for authentication requests
        private static readonly HttpClient httpClient = new HttpClient();

        // Constructor
		public Authentication(MobiControlApiConfig config, CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;

            this.baseUri = new Uri("https://" + config.FQDN);
           
            // Save config
			this.config = config;
            
            //grant_type=password&username=Administrator&password=1
            grant_type = "grant_type=password&username=" + config.Username + "&password=" + config.Password;

            httpClient.Timeout = httpTimeout;


        }
        
		// Get authentication token and lat caller konw it it's new
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

        // Validate the token
		private void ValidateToken()
		{
			try
			{
                // If the token is defined
				if (Token != null)
				{
                    // If expire time is undefined i.e. null
					if (TokenExpireTime == null)
					{
						Token = null;
					}
					else
					{
                        // If the token has expired
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

        // Request a token
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
                HttpResponseMessage bearerResult = await httpClient.SendAsync(requestToken, cancellationToken);
                string bearerData = await bearerResult.Content.ReadAsStringAsync();
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
