using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace MobiControlApi
{
    public class MonitorSotiFolderConfig
    {
        public string FolderPath;
        public bool IncludeSubFolders = false;
        public string Interval;
        public string RescanTime = "";


        public DateTime dtRescanTime => XmlConvert.ToDateTime(RescanTime, "HH:mm");
        public TimeSpan tsInterval => XmlConvert.ToTimeSpan(Interval);


        /*
            "Group": 
            {
              "FolderPath" : "",
              "Interval" : "PT30S"
              "IncludeSubFolders" : true
              "RescanTime": "01:00"
            }      
         */


        public MonitorSotiFolderConfig(string folderPath, string interval, bool includeSubFolders)
        {
            FolderPath = folderPath;
            Interval = interval;
            IncludeSubFolders = includeSubFolders;
        }



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
