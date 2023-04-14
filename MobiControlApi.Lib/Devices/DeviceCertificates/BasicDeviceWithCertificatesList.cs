using System;
using Microsoft.ApplicationInsights.DataContracts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobiControlApi.Devices.DeviceCertificates
{
    public class BasicDeviceWithCertificatesList
    {
        private Api api;

        public BasicDeviceWithCertificatesList(Api api)
        {
            this.api = api;
        }


        public async Task<List<BasicDeviceWithCertificates>> GetAllDevicesCertificatesAsync(string deviceGroupPath, bool includeSubgroups)
        {
            // Get list of all devices
            List<BasicDevice> basicDevices = await api.GetBasicDeviceListFromSotiAsync(deviceGroupPath, includeSubgroups);

            // Get certificates for them
            List<BasicDeviceWithCertificates> basicDevicesWithCerts = await GetDevicesCertificatesAsync(basicDevices);

            return basicDevicesWithCerts;
        }

        public async Task<List<BasicDeviceWithCertificates>> GetDevicesCertificatesAsync(List<BasicDevice> basicDevices)
        {
            List<BasicDeviceWithCertificates> deviceWithCertificates = new List<BasicDeviceWithCertificates>();

            foreach (BasicDevice basicDevice in basicDevices)
            {
                BasicDeviceWithCertificates bd = await BasicDeviceWithCertificates.GetBasicDeviceWithCertificates(basicDevice, api);
                deviceWithCertificates.Add(bd);
            }

            return deviceWithCertificates;
        }


    }
}

