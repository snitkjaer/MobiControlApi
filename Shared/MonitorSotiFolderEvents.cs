using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace MobiControlApi
{
    public class MonitorSotiFolderEvents
    {
        #region NewDeviceList Event
        // Delegate
        public delegate void NewDeviceListEventHandler(object sender, List<string> listNewDeviceIds);

        // The event
        public event NewDeviceListEventHandler NewDeviceList;

        // The method which fires the Event
        protected void OnNewDeviceList(object sender, List<string> listNewDeviceIds)
        {
            // Check if there are any Subscribers
            // Call the Event
            NewDeviceList?.Invoke(sender, listNewDeviceIds);
        }
        #endregion

        #region NewDeviceDict Event
        // Delegate
        public delegate void NewDeviceDictEventHandler(object sender, Dictionary<string, JObject> dirNewDevices);

        // The event
        public event NewDeviceDictEventHandler NewDeviceDict;

        // The method which fires the Event
        protected void OnNewDeviceDict(object sender, Dictionary<string, JObject> dirNewDevices)
        {
            // Check if there are any Subscribers
            // Call the Event
            NewDeviceDict?.Invoke(sender, dirNewDevices);
        }
        #endregion


        #region RemovedDeviceList Event
        // Delegate
        public delegate void RemovedDeviceListEventHandler(object sender, List<string> listRemovedDeviceIds);

        // The event
        public event RemovedDeviceListEventHandler RemovedDeviceList;

        // The method which fires the Event
        protected void OnRemovedDeviceList(object sender, List<string> listRemovedDeviceIds)
        {
            // Check if there are any Subscribers
            // Call the Event
            RemovedDeviceList?.Invoke(sender, listRemovedDeviceIds);
        }
        #endregion
    }
}
