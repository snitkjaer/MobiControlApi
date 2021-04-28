using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.ApplicationInsights.DataContracts;
using System.Threading;
using System.Linq;

namespace MobiControlApi
{
    public partial class Api : LogAbstraction
    {

        // Get list of device id's
        public async Task<List<string>> GetDeviceIdListAsync(string deviceGroupPath, bool includeSubgroups)
        {
            // Get basic device info
            List<BasicDevice> deviceList = await GetBasicDeviceListJsonSearchDbAsync(deviceGroupPath, null, includeSubgroups, false, 0, 1000);
            // Convert to device ID
            List<string> deviceIdList = deviceList
                .Select((arg) => arg.DeviceId)
                .ToList();

            return deviceIdList;

        }
    }
}
