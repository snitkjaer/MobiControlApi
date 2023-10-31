using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiControlApi.Devices.DeviceCertificates;

namespace MobiControlApi.UnitTestMs.Devices
{
    [TestClass]
    public class SendActionToDevicesAsyncTest
    {
        static CancellationToken token;
        static MobiControlApiConfig mobiControlApiConfig = MobiControlApiConfig.GetConfigFromJsonFile("MobiControlServerApiConfig.json");
        HttpClient httpClient = new HttpClient();

        [DataTestMethod]
        [DataRow(TestData.deviceImei)]
        public async Task SendActionToDevicesAsyncTest_SyncFilesNows(string imei)
        {
            #region Arrange
            Api mcApi = new Api(mobiControlApiConfig, null, token, httpClient);

            #endregion

            #region Act
            BasicDevice basicDevice = await mcApi.GetBasicDeviceOnImeiAsync(imei);
            bool result = await mcApi.SendActionToDevicesAsync(basicDevice.DeviceId, Api.ActionType.SyncFilesNow);
            #endregion

            #region Assert
            Assert.IsTrue(result);
            #endregion
        }


        [DataTestMethod]
        [DataRow(TestData.deviceImei)]
        public async Task SendActionToDevicesAsyncTest_CheckIn(string imei)
        {
            #region Arrange
            Api mcApi = new Api(mobiControlApiConfig, null, token, httpClient);

            #endregion

            #region Act
            BasicDevice basicDevice = await mcApi.GetBasicDeviceOnImeiAsync(imei);
            bool result = await mcApi.SendActionToDevicesAsync(basicDevice.DeviceId, Api.ActionType.CheckIn);
            #endregion

            #region Assert
            Assert.IsTrue(result);
            #endregion
        }

        // // Install client certificate on device
        [DataTestMethod]
        [DataRow(TestData.deviceImei, TestData.p12FilePath, TestData.certificatePassword)]
        public async Task InstallCertificateOnDevice(string imei, string p12FilePath, string certificatePassword)
        {
            #region Arrange
            Api mcApi = new Api(mobiControlApiConfig, null, token, httpClient);

            #endregion

            #region Act
            BasicDevice basicDevice = await mcApi.GetBasicDeviceOnImeiAsync(imei);
            bool result = await mcApi.InstallCertificateOnDevice(basicDevice.DeviceId, p12FilePath, certificatePassword);
            #endregion

            #region Assert
            Assert.IsTrue(result);
            #endregion
        }



        // Renew a SCEP certificate installed by SOTI
        [DataTestMethod]
        [DataRow(TestData.deviceImei, TestData.certificateRenewSerialNumber)]
        public async Task RenewDeviceCertificateTest(string imei, string certSerialNumber)
        {
            #region Arrange
            Api mcApi = new Api(mobiControlApiConfig, null, token, httpClient);
            BasicDevice basicDevice = await mcApi.GetBasicDeviceOnImeiAsync(imei);
            DeviceCertificate deviceCertificate = null;
            BasicDeviceWithCertificates basicDeviceWithCertificates = await BasicDeviceWithCertificates.GetBasicDeviceWithCertificates(basicDevice, mcApi);
            foreach (DeviceCertificate cer in basicDeviceWithCertificates.deviceCertificates)
            {
                if( cer.SerialNumber == certSerialNumber)
                {
                    deviceCertificate = cer;
                    break;
                }
            }
            #endregion

            #region Act
            RenewDeviceCertificate renewDeviceCertificate = new RenewDeviceCertificate(mcApi);

            bool result = await renewDeviceCertificate.Renew(basicDevice.DeviceId, deviceCertificate.ReferenceId);

            #endregion

            #region Assert
            Assert.IsTrue(result);
            #endregion
        }

    }
}
