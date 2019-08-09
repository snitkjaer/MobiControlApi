﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;

namespace MobiControlApi
{
    public partial class Api : LogAbstraction
    {
        public enum ActionType { CheckIn, Wipe, Lock, Unenroll, SendMessage, Locate, SendScript, Delete, Disable, Enable, Rename, SetPasscode, AppleSoftwareUpdateScan, AppleSoftwareUpdateSchedule, AppleSoftwareUpdateRefreshStatus, SetDeviceName, ResetPasscode, AllowSotiSurf, BlockSotiSurf, ClearRestrictions, BypassActivationLock, SoftReset, RemoteRing, TurnOffSuspend, StartTracking, StopTracking, ClearSotiSurfCache, BlockSotiHub, AllowSotiHub, ClearSotiHubCache, DisablePasscodeLock, ScanForViruses, UpdateVirusDefinitions, SendTestPage, FactoryReset, EnableWorkProfile, DisableWorkProfile, SendSmsMessage, SetWallpaper, ResetUserBinding, EnableLostMode, DisableLostMode, BlockExchangeAccess, AllowExchangeAccess, EnableAgentUpgrade, DisableAgentUpgrade, MigrateToELMAgent, UpgradeAgentNow, SyncFilesNow, SendScriptViaSms, EnrollInEFOTA, LinuxSoftwareUpdateSchedule, UpgradeFirmware, UpdateManagementProfile, SendScriptViaPns, ResetContainerPasscode, UpgradeAgent, UpdateLicense, PlaySound, LinuxSoftwareUpdateScan, SetFirmwarePassword, UnlockUserAccount, MigrateToAndroidEnterprise };

        public class ActionInfo
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public ActionType Action;
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string Message;

            public ActionInfo(ActionType actionType, string message)
            {
                this.Action = actionType;
                this.Message = message;
            }
        }

        public async Task<bool> SendActionToDevicesAsync(string deviceId, ActionType actionType)
        {
            return await SendActionToDevicesAsync(deviceId, new ActionInfo(actionType, null));
        }

        public async Task<bool> SendActionToDevicesAsync(string deviceId, ActionInfo actionInfo)
        {
            // POST /devices/{deviceId}/actions

            /*
                {
                  "Action": "CheckIn"
                }
            
            JObject objBody = new JObject(
                new JProperty("Action", Action.ToString())
            );

     */

            // Generate resourcePath
            //      /devices/{deviceId}/actions
            string resourcePath = "devices/" + deviceId + "/actions";



            string body = JsonConvert.SerializeObject(actionInfo);


            // Call GetJsonAsync
            return await PostJsonAsync(resourcePath, body);
        }

    }

   

}

