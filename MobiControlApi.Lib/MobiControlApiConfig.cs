using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;


namespace MobiControlApi
{
    public class MobiControlApiConfig
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

        // Cache
        public bool CacheDevices = false;
        public string CacheUpdateInterval = null;
        public string MaxSotiResponseTime = null;
        public string MaxCacheAge = null;

        public TimeSpan tsCacheUpdateInterval => XmlConvert.ToTimeSpan(CacheUpdateInterval);
        public TimeSpan tsMaxSotiResponseTime => XmlConvert.ToTimeSpan(MaxSotiResponseTime);
        public TimeSpan tsMaxCacheAge => XmlConvert.ToTimeSpan(MaxCacheAge);

        public MobiControlApiConfig(string fQDN, string clientId, string clientSecret, string username, string password)
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
            "Password":"",
            "CacheDevices": true,
            "CacheUpdateInterval": "PT60S",
            "MaxSotiResponseTime": "PT10S",
            "MaxCacheAge": "PT120S"           

        }

        */

        public static MobiControlApiConfig GetConfigFromJObject(JObject jsonConfig)
		{
			return jsonConfig.ToObject<MobiControlApiConfig>();
		}

		public static MobiControlApiConfig GetConfigFromJsonString(string jsonConfig)
        {
            return JsonConvert.DeserializeObject<MobiControlApiConfig>(jsonConfig, new TimeSpanConverter());
        }

        public static MobiControlApiConfig GetConfigFromJsonFile(string jsonConfigFilePath)
        {
            return JsonConvert.DeserializeObject<MobiControlApiConfig>(System.IO.File.ReadAllText(jsonConfigFilePath), new TimeSpanConverter());
        }

    }
}
