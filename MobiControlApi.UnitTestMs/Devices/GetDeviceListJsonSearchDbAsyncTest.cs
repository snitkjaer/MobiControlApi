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
    public class GetDeviceListJsonSearchDbAsyncTest
    {
        static CancellationToken token;
        static MobiControlApiConfig mobiControlApiConfig = MobiControlApiConfig.GetConfigFromJsonFile("MobiControlServerApiConfig.json");
        HttpClient httpClient = new HttpClient();

        [DataTestMethod]
        [DataRow("/", true)]
        [DataRow(TestData.groupName, true)]
        public async Task GetDeviceListJsonSearchDbAsyncTest_Count(string groupPath, bool includeSubGroups)
        {
            #region Arrange
            Api mcApi = new Api(mobiControlApiConfig, null, token, httpClient);

            #endregion

            #region Act
            String responseJosn = await mcApi.GetDeviceListJsonSearchDbAsync(groupPath, null, includeSubGroups, false, 0, 1000);

            #endregion

            #region Assert
            int noDevices = Regex.Matches(responseJosn, "DeviceId").Count;
            Assert.IsTrue(noDevices > 100);
            #endregion
        }

    }
}
