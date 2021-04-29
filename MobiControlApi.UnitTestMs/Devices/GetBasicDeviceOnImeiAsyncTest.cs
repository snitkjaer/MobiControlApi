using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiControlApi.UnitTestMs.Devices
{
    [TestClass]
    public class GetBasicDeviceOnImeiAsyncTest
    {
        static CancellationToken token;
        static MobiControlApiConfig mobiControlApiConfig = MobiControlApiConfig.GetConfigFromJsonFile("MobiControlServerApiConfig.json");
        HttpClient httpClient = new HttpClient();

        [DataTestMethod]
        [DataRow("123456789012345", false)]
        [DataRow(TestData.deviceImei, true)]
        public async Task GetBasicDeviceOnImeiAsyncTest_Test(string imei, bool found)
        {
            #region Arrange
            Api mcApi = new Api(mobiControlApiConfig, null, token, httpClient);

            #endregion

            #region Act
            BasicDevice basicDevice = await mcApi.GetBasicDeviceOnImeiAsync(imei);

            #endregion

            #region Assert
            if (found)
                Assert.IsNotNull(basicDevice);
            else
                Assert.IsNull(basicDevice);
            #endregion
        }

    }
}
