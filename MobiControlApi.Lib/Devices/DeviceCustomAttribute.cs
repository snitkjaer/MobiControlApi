using System;
using Newtonsoft.Json;

namespace MobiControlApi.Devices
{
    public class DeviceCustomAttribute
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }
        [JsonProperty("dataType", NullValueHandling = NullValueHandling.Ignore)]
        public CustomAttributeDataType DataType { get; set; }


        public static DeviceCustomAttribute FromJson(string json) => JsonConvert.DeserializeObject<DeviceCustomAttribute>(json, Helpers.Converter.Settings);
    }

    public enum CustomAttributeDataType
    {
        Boolean,
        Numeric,
        Text,
        DateTime,
        Enumerator,
        Date
    }
}

