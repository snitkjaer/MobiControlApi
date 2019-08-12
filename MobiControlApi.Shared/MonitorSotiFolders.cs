using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;

namespace MobiControlApi
{
    public class MonitorSotiFolders : MonitorSotiFolderEvents
    {
        /*
         * Monitor devices in a SOTI group / tree
         * 
         * Events
         * - New device detected
         * - Device removed
         * 
        */
        CancellationToken token;

        // Configuration input
        private JObject MobiControlApiConfigJson;
        private JArray MobiControlFolderssToMonitorJson;

        // SOTI Server API
        public Api mcApi;
        public List<MonitorSotiFolder> listMonitorSotiFolder;

        // Constructor
        public MonitorSotiFolders(JObject mobiControlApiConfigJson, JArray mobiControlGroupsToMonitorJson, TelemetryClient tc, CancellationToken token)
        {
            this.token = token;
            this.tc = tc;
            MobiControlApiConfigJson = mobiControlApiConfigJson;
            MobiControlFolderssToMonitorJson = mobiControlGroupsToMonitorJson;

            // Start a task for each SOTI folder to be monitored
            listMonitorSotiFolder = new List<MonitorSotiFolder>();

            // Create task list for each SOTI folder to be monitored
            foreach (var group in MobiControlFolderssToMonitorJson)
            {
                // Start monitoring folder but dont pass any know devices i.e. on start (or restart) all will come up as new devices
                MonitorSotiFolder monitorSotiGroup = new MonitorSotiFolder(group.ToString(), null, tc, token);
                monitorSotiGroup.NewDeviceList += MonitorSotiGroup_NewDeviceList;
                monitorSotiGroup.RemovedDeviceList += MonitorSotiGroup_RemovedDeviceList;
                listMonitorSotiFolder.Add(monitorSotiGroup);

            }
        }

        // List of monitoring tasks
        List<Task> listTask;

        // Start monitor tasks for all folders and wait for cancellation
        public async Task Start()
        {
            try
            {
                // list of monitoring tasks
                listTask = new List<Task>();

                if (mcApi == null)
                {
                    // Validate connnection to the MC server
                    mcApi = new Api(MobiControlApiConfigJson, tc, token);

                }

                
                // Create task list for each SOTI folder to be monitored
                foreach (var group in listMonitorSotiFolder)
                {
                    // Start task
                    listTask.Add(group.Start(mcApi));
                }

                // Wait for all tasks to complete (will happen when they are cancelled
                await Task.WhenAll(listTask);

            }
            catch (Exception ex)
            {
                Log("Exception starting MobiControl group monitor", SeverityLevel.Error);
                TrackException(ex);
            }
        }

        // Get device id list for all monitored folders
        public async Task<List<string>> GetDeviceIdListAsync()
        {
            List<string> listDeviceIds = new List<string>();

            try
            {
                if (mcApi == null)
                    // Validate connnection to the MC server
                    mcApi = new Api(MobiControlApiConfigJson, tc, token);

                // Itterate over monitored groups and return device list
                foreach (var group in listMonitorSotiFolder)
                {

                    listDeviceIds.AddRange(await group.GetDeviceIdListAsync(mcApi));

                }
            }
            catch (Exception ex)
            {
                TrackException(ex);
            }

            return listDeviceIds;
        }



        // Get device list for all monitored folders
        public async Task<List<Device>> GetDeviceListAsync()
        {
            List<Device> listDevices = new List<Device>();

            try
            {
                if (mcApi == null)
                    // Validate connnection to the MC server
                    mcApi = new Api(MobiControlApiConfigJson, tc, token);

                // Itterate over monitored groups and return device dict
                foreach (var group in listMonitorSotiFolder)
                {
                    listDevices.AddRange(await group.GetDeviceListAsync(mcApi));
                }
            }
            catch (Exception ex)
            {
                TrackException(ex);
            }

            return listDevices;
        }

        protected void MonitorSotiGroup_RemovedDeviceList(object sender, List<string> listRemovedDeviceIds)
        {
            OnRemovedDeviceList(sender, listRemovedDeviceIds);
        }

        protected void MonitorSotiGroup_NewDeviceList(object sender, List<string> listNewDeviceIds)
        {
            OnNewDeviceList(sender, listNewDeviceIds);
        }

    }
}
