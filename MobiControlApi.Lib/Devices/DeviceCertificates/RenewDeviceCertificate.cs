using System.Threading.Tasks;


namespace MobiControlApi.Devices.DeviceCertificates
{
	public class RenewDeviceCertificate : LogAbstraction
    {
        private Api api;

        public RenewDeviceCertificate(Api api)
        {
            this.api = api;
        }

        public async Task<bool> Renew(string deviceId, string referenceId)
        {
            // Generate resourcePath
            // /devices/{deviceId}/certificates/{referenceId}/actions
            string resourcePath = $"/devices/{deviceId}/certificates/{referenceId}/actions";
            string action = "{  \"ActionKind\": \"Renew\"}";


            Log($"Renewing device certificate {referenceId} for device {deviceId}", SeverityLevel.Information);

            // Call GetJsonAsync
            return await api.PostJsonAsync(resourcePath, action);
        }
    }
}

