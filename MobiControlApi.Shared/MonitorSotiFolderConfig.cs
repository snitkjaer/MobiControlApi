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
        public bool IncludeSubFolders = false;

        public MonitorSotiFolderConfig(string folderPath, string interval, bool includeSubFolders)
        {
            FolderPath = folderPath;
            Interval = interval;
            IncludeSubFolders = includeSubFolders;
        }

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
              "IncludeSubFolders" : true
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
