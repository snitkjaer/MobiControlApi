using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiControlApi.UnitTestMs.Devices
{
    [TestClass]
    public class GetBasicDeviceListJsonSearchDbAsyncTest
    {
        static CancellationToken token;
        static MobiControlApiConfig mobiControlApiConfig = MobiControlApiConfig.GetConfigFromJsonFile("MobiControlServerApiConfig.json");

        [TestMethod]
        public async Task GetBasicDeviceListJsonSearchDbAsyncTest_CountRoot()
        {
            #region Arrange
            Api mcApi = new Api(mobiControlApiConfig, null, token);
            String responseJosn = await mcApi.GetDeviceListJsonSearchDbAsync("/", null, true, false, 0, 1000);
            int noDevices = Regex.Matches(responseJosn, "DeviceId").Count;
            #endregion

            #region Act
            List<BasicDevice> devices = await mcApi.GetBasicDeviceListJsonSearchDbAsync("/", null, true, false, 0, 1000);

            #endregion

            #region Assert
            Assert.AreEqual(noDevices, devices.Count);
            #endregion
        }


        [TestMethod]
        public async Task GetBasicDeviceListJsonSearchDbAsyncTest_CountSub()
        {
            #region Arrange
            Api mcApi = new Api(mobiControlApiConfig, null, token);
            String responseJosn = await mcApi.GetDeviceListJsonSearchDbAsync(TestData.groupName, null, true, false, 0, 1000);
            int noDevices = Regex.Matches(responseJosn, "DeviceId").Count;
            #endregion

            #region Act
            List<BasicDevice> devices = await mcApi.GetBasicDeviceListJsonSearchDbAsync(TestData.groupName, null, true, false, 0, 1000);

            #endregion

            #region Assert
            Assert.AreEqual(noDevices, devices.Count);
            #endregion
        }
    }





}
