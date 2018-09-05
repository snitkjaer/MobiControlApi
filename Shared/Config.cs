using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MobiControlApi
{
    public class Config
    {
		// The is fully qualified domain name for the server
        public string FQDN = "server.domain.tld";

        // Needs to be generated on SOTI MobiControl server using MCadmin.exe
		public string ClientId = "";
		public string ClientSecret = "";
        // Create in the SOTI MobiControl user management - user must have admin rights 
		public string Username = "";
		public string Password = "";

		public Uri baseUri => new Uri("https://" + FQDN + "/MobiControl/api/");

		public Config(string fQDN, string clientId, string clientSecret, string username, string password)
		{
			FQDN = fQDN;
			ClientId = clientId;
			ClientSecret = clientSecret;
			Username = username;
			Password = password;
		}


		/*

        {
            "FQDN": "server.domain.tld",
            "ClientId": "",
            "ClientSecret": "",
            "Username": "",
            "Password":""

        }

        */

		public static Config GetConfigFromJObject(JObject jsonConfig)
		{
			return jsonConfig.ToObject<Config>();
		}

		public static Config GetConfigFromJsonString(string jsonConfig)
        {
            return JsonConvert.DeserializeObject<Config>(jsonConfig);
        }

    }
}
