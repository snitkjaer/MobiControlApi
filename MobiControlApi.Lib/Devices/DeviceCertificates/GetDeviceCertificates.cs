using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MobiControlApi.Devices.DeviceCertificates
{
    public class GetDeviceCertificates:LogAbstraction
    {
        private Api api;

        public GetDeviceCertificates(Api api)
        {
            this.api = api;
        }


        // Get DeviceWithCertificates
        public async Task<List<DeviceCertificate>> GetDeviceCertificatesAsync(string DeviceId)
        {

            // Generate resourcePath
            string resourcePath = "devices/" + DeviceId + "/certificates";

            // Get from SOTI
            string jsonCertificates = await api.GetJsonAsync(resourcePath);

            // Convert
            List<DeviceCertificate> certs = JsonConvert.DeserializeObject<List<DeviceCertificate>>(jsonCertificates);

            Log("DeviceId " + DeviceId + " has " + certs.Count + " certs", SeverityLevel.Verbose);


            return certs;
        }
    }
}

