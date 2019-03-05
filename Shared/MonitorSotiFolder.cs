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
        public List<string> listKnownDeviceIds;


        List<string> listRemovedDeviceIds;
        
        

        /*
            "Group": 
            {
              "FolderPath" : "",
              "Interval" : "PT30S"
            }      
         */

        public MonitorSotiFolder(string jsonConfig, List<string> listStartKnownDeviceIds)
        {
            // Import ćonfig from json
            monitorSotiGroupConfig = MonitorSotiFolderConfig.GetConfigFromJsonString(jsonConfig);


            // If we get a null list of know devices - create an empty list.
            if (listStartKnownDeviceIds != null)
            {
                listKnownDeviceIds = listStartKnownDeviceIds;
            }
            else
                listKnownDeviceIds = new List<string>();

        }   


        public async Task<int> Start(Api mcApi, CancellationToken cancellationToken)
        {
            do
            {
                try
                {
                    List<string> listNewDeviceIds = new List<string>();
                    List<string> listKnownDeviceIdsAtLastScan = listKnownDeviceIds;

                    // reset temp dict and list
                    listRemovedDeviceIds = listKnownDeviceIds;  // Set to all known - found devices will be removed leaving remove

                    // Get current devices in SOTI folder from server
                    listKnownDeviceIds = await mcApi.GetDeviceIdListAsync(monitorSotiGroupConfig.FolderPath, false, cancellationToken);



                    // Itterate over device found
                    foreach (string deviceId in listKnownDeviceIds)
                    {

                        // We need to find:
                        // - Newly added devices
                        // - Removed devices

                        // Add all to new know devices

                        // If device id is known
                        if (listKnownDeviceIdsAtLastScan.Contains(deviceId))
                        {
                            // Device id is known and was found in latest folder scan (or initial given list)
                            // -> Remove it for the removed list
                            listRemovedDeviceIds.Remove(deviceId);
                        }
                        else
                        {
                            // Device is in unknown
                            // Add to new devices dict
                            listNewDeviceIds.Add(deviceId);

                        }



                        // If new devices was found
                        if (listNewDeviceIds.Count > 0)
                        {
                            OnNewDeviceList(this, listNewDeviceIds);
                        }

                        // If devices were removed
                        if (listRemovedDeviceIds.Count > 0)
                            OnRemovedDeviceList(this, listRemovedDeviceIds);


                    }

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

        public async Task<List<Device>> GetDeviceListAsync(Api mcApi, CancellationToken cancellationToken)
        {
            return await mcApi.GetDeviceListAsync(monitorSotiGroupConfig.FolderPath, cancellationToken);
        }
    }
}
