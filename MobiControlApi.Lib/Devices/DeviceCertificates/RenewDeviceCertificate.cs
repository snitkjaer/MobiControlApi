using System;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.ApplicationInsights.DataContracts;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using System.Runtime.ConstrainedExecution;

namespace MobiControlApi.Devices.DeviceCertificates
{
	public class RenewDeviceCertificate : LogAbstraction
    {
        private Api api;

        public RenewDeviceCertificate(Api api)
        {
            this.api = api;
        }

        private async Task<bool> Renew(string deviceId, string referenceId)
        {
            // Generate resourcePath
            // /devices/{deviceId}/certificates/{referenceId}/actions
            string resourcePath = $"/devices/{deviceId}/certificates/{referenceId}/actions";
            string action = "{\n  \"ActionKind\": \"Renew\"\n}";


            Log($"Renewing device certificate {referenceId} for device {deviceId}", SeverityLevel.Verbose);

            // Call GetJsonAsync
            return await api.PutJsonAsync(resourcePath, action);
        }
    }
}

