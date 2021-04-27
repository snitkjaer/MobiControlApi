using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiControlApi.UnitTestMs.Devices
{
    [TestClass]
    public class GetDeviceListJsonSearchDbAsyncTest
    {
        static CancellationToken token;
        static MobiControlApiConfig mobiControlApiConfig = MobiControlApiConfig.GetConfigFromJsonFile("MobiControlServerApiConfig.json");

        [TestMethod]
        public async Task GetDeviceListJsonSearchDbAsyncTest_CountRoot()
        {
            #region Arrange
            Api mcApi = new Api(mobiControlApiConfig, null, token);

            #endregion

            #region Act
            String responseJosn = await mcApi.GetDeviceListJsonSearchDbAsync("/", null, true, false, 0, 1000);

            #endregion

            #region Assert
            int noDevices = Regex.Matches(responseJosn, "DeviceId").Count;
            Assert.IsTrue(noDevices == 355);
            #endregion
        }


        [TestMethod]
        public async Task GetDeviceListJsonSearchDbAsyncTest_CountSub()
        {
            #region Arrange
            Api mcApi = new Api(mobiControlApiConfig, null, token);

            #endregion

            #region Act
            String responseJosn = await mcApi.GetDeviceListJsonSearchDbAsync("/Test", null, true, false, 0, 1000);

            #endregion

            #region Assert
            int noDevices = Regex.Matches(responseJosn, "DeviceId").Count;
            Assert.IsTrue(noDevices == 224);
            #endregion
        }
    }
}
