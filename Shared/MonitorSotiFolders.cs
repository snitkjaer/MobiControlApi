using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

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
        Api mcApi;
        public List<MonitorSotiFolder> listMonitorSotiFolder;

        // Constructor
        public MonitorSotiFolders(JObject mobiControlApiConfigJson, JArray mobiControlGroupsToMonitorJson, CancellationToken token)
        {
            this.token = token;
            MobiControlApiConfigJson = mobiControlApiConfigJson;
            MobiControlFolderssToMonitorJson = mobiControlGroupsToMonitorJson;

            // Start a task for each SOTI folder to be monitored
            listMonitorSotiFolder = new List<MonitorSotiFolder>();

            // Create task list for each SOTI folder to be monitored
            foreach (var group in MobiControlFolderssToMonitorJson)
            {
                // Start monitoring folder but dont pass any know devices i.e. on start (or restart) all will come up as new devices
                MonitorSotiFolder monitorSotiGroup = new MonitorSotiFolder(group.ToString(), null);
                monitorSotiGroup.NewDeviceList += MonitorSotiGroup_NewDeviceList;
                monitorSotiGroup.RemovedDeviceList += MonitorSotiGroup_RemovedDeviceList;
                listMonitorSotiFolder.Add(monitorSotiGroup);

            }
        }

        // List of monitoring tasks
        List<Task> listTask;

        public async Task Start()
        {
            try
            {
                // Validate connnection to the MC server
                mcApi = new Api(MobiControlApiConfigJson);

                // list of monitoring tasks
                listTask = new List<Task>();
                
                // Create task list for each SOTI folder to be monitored
                foreach (var group in listMonitorSotiFolder)
                {
                    // Start task
                    listTask.Add(group.Start(mcApi, token));
                }

                // Wait for all tasks to complete (will happen when they are cancelled
                await Task.WhenAll(listTask);

            }
            catch (Exception ex)
            {
            }
        }


        public async Task<List<string>> GetDeviceIdListAsync()
        {
            List<string> listDeviceIds = new List<string>();

            try
            {
                // Validate connnection to the MC server
                mcApi = new Api(MobiControlApiConfigJson);

                // Itterate over monitored groups and return device list
                foreach (var group in listMonitorSotiFolder)
                {

                    listDeviceIds.AddRange(await group.GetDeviceIdListAsync(mcApi, token));

                }
            }
            catch (Exception e)
            {

            }

            return listDeviceIds;
        }

        public async Task<List<Device>> GetDeviceListAsync()
        {
            List<Device> listDevices = new List<Device>();

            try
            {
                // Validate connnection to the MC server
                mcApi = new Api(MobiControlApiConfigJson);

                // Itterate over monitored groups and return device dict
                foreach (var group in listMonitorSotiFolder)
                {
                    listDevices.AddRange(await group.GetDeviceListAsync(mcApi, token));
                }
            }
            catch (Exception e)
            {

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
