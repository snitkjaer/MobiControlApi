using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MobiControlApi

{
    public class MonitorSotiFolder : MonitorSotiFolderEvents
    {
        MonitorSotiFolderConfig monitorSotiGroupConfig;
        public string FolderPath => monitorSotiGroupConfig.FolderPath;


        // Known device Dictionary
        Dictionary<string, JObject> dictKnownDevices;
        public List<string> listKnownDeviceIds
        {
            get
            {
                return new List<string>(this.dictKnownDevices.Keys);
            }
        }



        // temp lists
        Dictionary<string, JObject> dictNewDevices;
        private List<string> listNewDeviceIds
        {
            get
            {
                return new List<string>(this.dictNewDevices.Keys);
            }
        }

        List<string> listRemovedDeviceIds;
        
        

        /*
            "Group": 
            {
              "FolderPath" : "",
              "Interval" : "PT30S"
            }      
         */

        public MonitorSotiFolder(string jsonConfig, List<string> listKnownDeviceIds)
        {
            // Import ćonfig from json
            monitorSotiGroupConfig = MonitorSotiFolderConfig.GetConfigFromJsonString(jsonConfig);
            // If we get a null list of know devices - create an empty list.
            dictKnownDevices = new Dictionary<string, JObject>();
            if (listKnownDeviceIds != null)
            {
                foreach(string deviceId in listKnownDeviceIds)
                {
                    dictKnownDevices.Add(deviceId, null);
                }
            }

        }   

        // Get device info JObject from device id
        public JObject getDeviceInfo(string deviceId)
        {
            JObject device = null;

            try
            {
                dictKnownDevices.TryGetValue(deviceId, out device);
            }
            catch { }

            return device;
        }

        public async Task<int> Start(Api mcApi, CancellationToken cancellationToken)
        {
            do
            {
                try
                {
                    Dictionary<string, JObject> dirNewKnownDevices = new Dictionary<string, JObject>();
                    // Save
                    List<string> listKnownDeviceIdsAtLastScan = listKnownDeviceIds;

                    // reset temp dict and list
                    dictNewDevices = new Dictionary<string, JObject>();
                    listRemovedDeviceIds = listKnownDeviceIds;  // Set to all known - found devices will be removed leaving remove

                    // Get device in SOTI folder and update known devices
                    dirNewKnownDevices = await mcApi.GetDeviceIdJsonDictAync(monitorSotiGroupConfig.FolderPath, false, cancellationToken);

                    // Itterate over device found
                    foreach (var device in dirNewKnownDevices)
                    {
                        // We need to find:
                        // - Newly added devices
                        // - Removed devices

                        // If device id is known
                        if (listKnownDeviceIdsAtLastScan.Contains(device.Key))
                        {
                            // Device id is known and was found in latest folder scan (or initial given list)
                            // -> Remove it for the removed list
                            listRemovedDeviceIds.Remove(device.Key);
                        }
                        else
                        {
                            // Device is in unknown
                            // Add to new devices dict
                            dictNewDevices.Add(device.Key, device.Value);

                        }
                    }

                    // If new devices was found
                    if (dictNewDevices.Count > 0)
                    {
                        OnNewDeviceList(this, listNewDeviceIds);
                        OnNewDeviceDict(this, dictNewDevices);
                    }

                    // If devices were removed
                    if (listRemovedDeviceIds.Count > 0)
                        OnRemovedDeviceList(this, listRemovedDeviceIds);




                }
                catch (Exception e)
                {
                    // log.Error("Exception", e);
                }

                if (!cancellationToken.IsCancellationRequested)
                    await Task.Delay(monitorSotiGroupConfig.tsInterval, cancellationToken);
                /*
                else
                    OnDoneWatching(this);
                    */                   
            }
            while (!cancellationToken.IsCancellationRequested);

            return 0;
        }


        public async Task<List<string>> GetDeviceIdListAsync(Api mcApi, CancellationToken cancellationToken)
        {
            return await mcApi.GetDeviceIdListAsync(monitorSotiGroupConfig.FolderPath, false, cancellationToken); 
        }

        public async Task<Dictionary<string, JObject>> GetDeviceIdJsonDictAync(Api mcApi, CancellationToken cancellationToken)
        {
            return await mcApi.GetDeviceIdJsonDictAync(monitorSotiGroupConfig.FolderPath, false, cancellationToken);
        }

        public async Task<List<Device>> GetDeviceListAsync(Api mcApi, CancellationToken cancellationToken)
        {
            return await mcApi.GetDeviceListAsync(monitorSotiGroupConfig.FolderPath, false, cancellationToken);
        }
    }
}
