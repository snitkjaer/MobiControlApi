using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;

namespace MobiControlApi
{
    public partial class Api : LogAbstraction
    {
        public async Task<bool> SendActionToDevicesAsync(string deviceId, string Action, string Message)
        {
            // POST /devices/actions/DeviceIds

            List<string> deviceIds = new List<string>();
            deviceIds.Add(deviceId);

            ActionBody actionBody = new ActionBody(deviceIds, new ActionInfo(Action, Message));

            // Generate resourcePath
            string resourcePath = "devices/actions";
            string actionBodyJson = actionBody.ToJsonString();
            // Call GetJsonAsync
            return await PostJsonAsync(resourcePath, actionBodyJson);
        }



    }

    public class ActionBody
    {
        public List<string> DeviceIds;
        public ActionInfo ActionInfo;

        public ActionBody(List<string> deviceIds, ActionInfo actionInfo)
        {
            DeviceIds = deviceIds;
            ActionInfo = actionInfo;
        }

        public string ToJsonString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

    }

    public class ActionInfo
    {
        public string Action;
        public string Message;

        public ActionInfo(string action, string message)
        {
            Action = action;
            Message = message;
        }
    }
}

