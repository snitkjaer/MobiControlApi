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

        // Thread / task control
        CancellationTokenSource cts;

        // Configuration input
        private JObject MobiControlApiConfigJson;
        private JArray MobiControlFolderssToMonitorJson;

        // SOTI Server API
        Api mcApi;
        List<MonitorSotiFolder> listMonitorSotiFolder;

        // Constructor
        public MonitorSotiFolders(JObject mobiControlApiConfigJson, JArray mobiControlGroupsToMonitorJson)
        {
            MobiControlApiConfigJson = mobiControlApiConfigJson;
            MobiControlFolderssToMonitorJson = mobiControlGroupsToMonitorJson;
        }

        // Destructor
        ~MonitorSotiFolders()
        {
            if (cts != null)
                cts.Cancel();
        }

        public async Task Start()
        {
            try
            {
                cts = new CancellationTokenSource();

                // Validate connnection to the MC server
                mcApi = new Api(MobiControlApiConfigJson);

                // Start a task for each SOTI folder to be monitored
                listMonitorSotiFolder = new List<MonitorSotiFolder>();

                List<Task> listTask = new List<Task>();
                


                // Create task list for each SOTI folder to be monitored
                foreach (var group in MobiControlFolderssToMonitorJson)
                {
                    // Start monitoring folder but dont pass any know devices i.e. on start (or restart) all will come up as new devices
                    MonitorSotiFolder monitorSotiGroup = new MonitorSotiFolder(group.ToString(), null);
                    monitorSotiGroup.NewDeviceDict += MonitorSotiGroup_NewDeviceDict;
                    monitorSotiGroup.NewDeviceList += MonitorSotiGroup_NewDeviceList;
                    monitorSotiGroup.RemovedDeviceList += MonitorSotiGroup_RemovedDeviceList;
                    listMonitorSotiFolder.Add(monitorSotiGroup);
                    // Start task
                    listTask.Add(monitorSotiGroup.Start(mcApi, cts.Token));

                }

                await Task.WhenAll(listTask);


                //List<Task<string>> downloadTasksQuery = listMonitorSotiGroup.Select(t => t.Start).ToList();

                // Use ToArray to execute the query and start the download tasks.  
                //Task<int>[] downloadTasks = listMonitorSotiGroup.
                // You can do other work here before awaiting.  

                // Await the completion of all the running tasks.  
                //int[] lengths = await Task.WhenAll(downloadTasks);


                //Task<int> 
                // Parallel.ForEach(listMonitorSotiGroup, task => task.Start());

                //await Task.WhenAll(listMonitorSotiGroup):
            }
            catch (Exception ex)
            {
               
            }



        }

        private void MonitorSotiGroup_RemovedDeviceList(object sender, List<string> listRemovedDeviceIds)
        {
            OnRemovedDeviceList(sender, listRemovedDeviceIds);
        }

        private void MonitorSotiGroup_NewDeviceList(object sender, List<string> listNewDeviceIds)
        {
            OnNewDeviceList(sender, listNewDeviceIds);
        }

        private void MonitorSotiGroup_NewDeviceDict(object sender, Dictionary<string, JObject> dirNewDevices)
        {
            OnNewDeviceDict(sender, dirNewDevices);
        }
    }
}
