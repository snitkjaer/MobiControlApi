using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace MobiControlApi
{
    public class MonitorSotiFolderConfig
    {
        public string FolderPath;
        string Interval;

        public MonitorSotiFolderConfig(string folderPath, string interval)
        {
            FolderPath = folderPath;
            Interval = interval;
        }

        public TimeSpan tsInterval => XmlConvert.ToTimeSpan(Interval);




        /*
            "Group": 
            {
              "FolderPath" : "",
              "Interval" : "PT30S"
            }      
         */

        public static MonitorSotiFolderConfig GetConfigFromJObject(JObject jsonConfig)
        {
            return jsonConfig.ToObject<MonitorSotiFolderConfig>();
        }

        public static MonitorSotiFolderConfig GetConfigFromJsonString(string jsonConfig)
        {
            return JsonConvert.DeserializeObject<MonitorSotiFolderConfig>(jsonConfig);
        }
    }
}
