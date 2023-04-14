using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobiControlApi.Devices.DeviceCertificates
{
    public class BasicDeviceWithCertificates
    {
        public BasicDevice basicDevice;
        public List<DeviceCertificate> deviceCertificates;

        public BasicDeviceWithCertificates(BasicDevice basicDevice, List<DeviceCertificate> deviceCertificates)
        {
            this.basicDevice = basicDevice;
            this.deviceCertificates = deviceCertificates;
        }

        public static async Task<BasicDeviceWithCertificates> GetBasicDeviceWithCertificates(BasicDevice basicDevice, Api api)
        {
            GetDeviceCertificates getDeviceCertificates = new GetDeviceCertificates(api);

            List<DeviceCertificate> certs = await getDeviceCertificates.GetDeviceCertificatesAsync(basicDevice.DeviceId);


            return new BasicDeviceWithCertificates(basicDevice, certs);

        }
    }
}

