using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;

namespace MobiControlApi

{
    public class MonitorSotiFolder : MonitorSotiFolderEvents
    {
        CancellationToken token;

        MonitorSotiFolderConfig monitorSotiGroupConfig;
        public string FolderPath => monitorSotiGroupConfig.FolderPath;


        // Known device Dictionary
        public List<string> listKnownDeviceIds { get; private set; }



        /*
            "Group": 
            {
              "FolderPath" : "",
              "Interval" : "PT30S"
            }      
         */

        public MonitorSotiFolder(string jsonConfig, List<string> listStartKnownDeviceIds, TelemetryClient tc, CancellationToken token)
            :this (MonitorSotiFolderConfig.GetConfigFromJsonString(jsonConfig), listStartKnownDeviceIds, tc,token)
        {}

        public MonitorSotiFolder(MonitorSotiFolderConfig config, List<string> listStartKnownDeviceIds, TelemetryClient tc, CancellationToken token)
        {
            this.token = token;
            this.tc = tc;

            // Import ćonfig from json
            monitorSotiGroupConfig = config;


            // If we get a null list of know devices - create an empty list.
            if (listStartKnownDeviceIds != null)
            {
                listKnownDeviceIds = listStartKnownDeviceIds;
            }
            else
                listKnownDeviceIds = new List<string>();

        }

        // Start monitoring of a folder at an interval
        public async Task<int> Start(Api mcApi)
        {
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            List<string> listAddedDevices;
            List<string> listRemovedDevices;
            List<string> listCurrentDevices = new List<string>();

            if(monitorSotiGroupConfig.IncludeSubFolders)
                Log("Scanning SOTI folder '" + monitorSotiGroupConfig.FolderPath + "' including sub folders every " + monitorSotiGroupConfig.tsInterval.ToString(), SeverityLevel.Information);
            else
                Log("Scanning SOTI folder '" + monitorSotiGroupConfig.FolderPath + "' every " + monitorSotiGroupConfig.tsInterval.ToString(), SeverityLevel.Information);

            // Start monitoring loop
            while (!token.IsCancellationRequested)
            {
                try
                {
                    // Reset and start watch to time call to SOTI API
                    stopWatch.Reset();
                    stopWatch.Start();

                    // Get current devices in SOTI folder from server
                    listCurrentDevices = await mcApi.GetDeviceIdListAsync(monitorSotiGroupConfig.FolderPath, monitorSotiGroupConfig.IncludeSubFolders);

                    // Stop
                    stopWatch.Stop();

                    DeviceListDiff(listCurrentDevices, listKnownDeviceIds, out listAddedDevices, out listRemovedDevices);



                    // If new devices was found
                    if (listAddedDevices.Count > 0)
                        {
                            OnNewDeviceList(this, listAddedDevices);
                        }

                    // If devices were removed
                    if (listRemovedDevices.Count > 0)
                        OnRemovedDeviceList(this, listRemovedDevices);

                    // Save list of know devices
                    listKnownDeviceIds = listCurrentDevices;

                    // Stop
                    stopWatch.Stop();


                    var properties = new Dictionary<string, string>
                             {
                                 { "GroupPath", monitorSotiGroupConfig.FolderPath },
                                 { "CurrentDeviceCount",listCurrentDevices.Count().ToString()},
                                 { "UsingDeviceCache",mcApi.CacheDevices.ToString()}
                             };

                    TrackEvent("SotiMonitorScanGroup", stopWatch.Elapsed, properties);

                }
                catch (Exception ex)
                {
                    // Stop
                    stopWatch.Stop();

                    Log("Exception scanning SOTI groupe '" + monitorSotiGroupConfig.FolderPath + "' in " + stopWatch.Elapsed.ToString(), SeverityLevel.Error);
                    TrackException(ex);
                }


                // Wait before scanning again
                if (!token.IsCancellationRequested)
                    await Task.Delay(monitorSotiGroupConfig.tsInterval, token);

            }


            return 0;
        }


        // Diff 
        /// 
        public static void DeviceListDiff(List<string> listCurrentDevices, List<string> listLastCurrentDevices, out List<string> listAddedDevices, out List<string> listRemovedDevices)
        {
            // Input: listCurrentDevices + listLastCurrentDevices
            // Output: listAddedDevices + listRemovedDevices

            // Those devices that have been added since last time
            listAddedDevices = listCurrentDevices.Except(listLastCurrentDevices).ToList();
            // Those devices that have been removed since last time
            listRemovedDevices = listLastCurrentDevices.Except(listCurrentDevices).ToList();

        }


        public async Task<List<string>> GetDeviceIdListAsync(Api mcApi)
        {
            return await mcApi.GetDeviceIdListAsync(monitorSotiGroupConfig.FolderPath, monitorSotiGroupConfig.IncludeSubFolders); 
        }

        public async Task<List<Device>> GetDeviceListAsync(Api mcApi)
        {
            return await mcApi.GetDeviceListAsync(monitorSotiGroupConfig.FolderPath, monitorSotiGroupConfig.IncludeSubFolders);
        }
    }
}
