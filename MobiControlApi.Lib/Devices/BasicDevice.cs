using System;
using Newtonsoft.Json;

namespace MobiControlApi
{
    public class BasicDevice
    {
        [JsonProperty("DeviceId", NullValueHandling = NullValueHandling.Ignore)]
        public string DeviceId { get; set; }

        [JsonProperty("DeviceName", NullValueHandling = NullValueHandling.Ignore)]
        public string DeviceName { get; set; }

        [JsonProperty("IMEI_MEID_ESN", NullValueHandling = NullValueHandling.Ignore)]
        public string ImeiMeidEsn { get; set; }

        [JsonProperty("Path", NullValueHandling = NullValueHandling.Ignore)]
        public string Path { get; set; }

        [JsonProperty("Model", NullValueHandling = NullValueHandling.Ignore)]
        public string Model { get; set; }

        [JsonProperty("Kind", NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; }

        [JsonProperty("IsAgentOnline", NullValueHandling = NullValueHandling.Ignore)]
        public bool Online { get; set; }

        public static BasicDevice FromJson(string json) => JsonConvert.DeserializeObject<BasicDevice>(json, Helpers.Converter.Settings);
    }

}
