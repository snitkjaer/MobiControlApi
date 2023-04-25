using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;
using System.Net.Http;

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
        private readonly HttpClient httpClient;

        // Configuration input
        private MobiControlApiConfig MobiControlApiConfig;
        private List<MonitorSotiFolderConfig> listMonitorSotiFolderConfig;

        // SOTI Server API
        public Api mcApi;
        public List<MonitorSotiFolder> listMonitorSotiFolder;

        // Constructor
        public MonitorSotiFolders(MobiControlApiConfig mobiControlApiConfig, List<MonitorSotiFolderConfig> listMonitorSotiFolderConfig, TelemetryClient tc, CancellationToken token, HttpClient httpClient)
        {
            this.token = token;
            this.httpClient = httpClient;
            this.tc = tc;
            this.listMonitorSotiFolderConfig = listMonitorSotiFolderConfig;
            this.MobiControlApiConfig = mobiControlApiConfig;

            // Start a task for each SOTI folder to be monitored
            listMonitorSotiFolder = new List<MonitorSotiFolder>();

            // Create task list for each SOTI folder to be monitored
            foreach (MonitorSotiFolderConfig folder in listMonitorSotiFolderConfig)
            {
                // Start monitoring folder but dont pass any know devices i.e. on start (or restart) all will come up as new devices
                MonitorSotiFolder monitorSotiGroup = new MonitorSotiFolder(folder, null, tc, token);
                monitorSotiGroup.NewDeviceList += MonitorSotiGroup_NewDeviceList;
                monitorSotiGroup.RemovedDeviceList += MonitorSotiGroup_RemovedDeviceList;
                listMonitorSotiFolder.Add(monitorSotiGroup);

            }

            // Validate connnection to the MC server
            if (mcApi == null)
            {
                // Validate connnection to the MC server
                mcApi = new Api(MobiControlApiConfig, tc, token, httpClient);

            }

        }

       // public MonitorSotiFolders(JObject mobiControlApiConfigJson, JArray mobiControlGroupsToMonitorJson, TelemetryClient tc, CancellationToken token)
       //     :this

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
                    mcApi = new Api(MobiControlApiConfig, tc, token, httpClient);

                }

                // Start the monitoring task (CacheDevices)
                mcApi.StartMonitor().ConfigureAwait(false);

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


        public async Task ResetKnownDeviceIds()
        {
            try
            {
                // Itterate over monitored groups and return device list
                foreach (MonitorSotiFolder monitorSotiFolder in listMonitorSotiFolder)
                {
                    await monitorSotiFolder.ResetKnownDeviceIds();

                }
            }
            catch (Exception ex)
            {
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
                    mcApi = new Api(MobiControlApiConfig, tc, token, httpClient);

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
                    mcApi = new Api(MobiControlApiConfig, tc, token, httpClient);

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


        // Get device list for all monitored folders
        public async Task<List<BasicDevice>> GetBasicDeviceListAsync()
        {
            List<BasicDevice> listDevices = new List<BasicDevice>();

            try
            {
                if (mcApi == null)
                    // Validate connnection to the MC server
                    mcApi = new Api(MobiControlApiConfig, tc, token, httpClient);

                // Itterate over monitored groups and return device dict
                foreach (var group in listMonitorSotiFolder)
                {
                    listDevices.AddRange(await group.GetBasicDeviceListAsync(mcApi));
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
