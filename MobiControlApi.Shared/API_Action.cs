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
        public enum ActionInfo { CheckIn, Wipe, Lock, Unenroll, SendMessage, Locate, SendScript, Delete, Disable, Enable, Rename, SetPasscode, AppleSoftwareUpdateScan, AppleSoftwareUpdateSchedule, AppleSoftwareUpdateRefreshStatus, SetDeviceName, ResetPasscode, AllowSotiSurf, BlockSotiSurf, ClearRestrictions, BypassActivationLock, SoftReset, RemoteRing, TurnOffSuspend, StartTracking, StopTracking, ClearSotiSurfCache, BlockSotiHub, AllowSotiHub, ClearSotiHubCache, DisablePasscodeLock, ScanForViruses, UpdateVirusDefinitions, SendTestPage, FactoryReset, EnableWorkProfile, DisableWorkProfile, SendSmsMessage, SetWallpaper, ResetUserBinding, EnableLostMode, DisableLostMode, BlockExchangeAccess, AllowExchangeAccess, EnableAgentUpgrade, DisableAgentUpgrade, MigrateToELMAgent, UpgradeAgentNow, SyncFilesNow, SendScriptViaSms, EnrollInEFOTA, LinuxSoftwareUpdateSchedule, UpgradeFirmware, UpdateManagementProfile, SendScriptViaPns, ResetContainerPasscode, UpgradeAgent, UpdateLicense, PlaySound, LinuxSoftwareUpdateScan, SetFirmwarePassword, UnlockUserAccount, MigrateToAndroidEnterprise };



        public async Task<bool> SendActionToDevicesAsync(string deviceId, ActionInfo Action)
        {
            // POST /devices/{deviceId}/actions

            /*
                {
                  "Action": "CheckIn"
                }
             */
            JObject objBody = new JObject(
                new JProperty("Action", Action.ToString())
            );



            // Generate resourcePath
            //      /devices/{deviceId}/actions
            string resourcePath = "devices/" + deviceId + "/actions";

            // Call GetJsonAsync
            return await PostJsonAsync(resourcePath, objBody.ToString());
        }

    }

}

