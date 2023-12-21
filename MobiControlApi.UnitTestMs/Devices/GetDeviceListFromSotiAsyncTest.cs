using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace MobiControlApi.UnitTestMs.Devices
{
    [TestClass]
    public class GetDeviceListFromSotiAsyncTest
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
            Api mcApi = new Api(mobiControlApiConfig, token, httpClient);
            String responseJosn = await mcApi.GetDeviceListJsonSearchDbAsync(groupPath, null, includeSubGroups, false, 0, 1000);
            int noDevices = Regex.Matches(responseJosn, "DeviceId").Count;
            #endregion

            #region Act
            List<Device> devices= await mcApi.GetDeviceListFromSotiAsync(groupPath, includeSubGroups);

            #endregion

            #region Assert
            Assert.AreEqual(noDevices, devices.Count);
            #endregion
        }

    }
}
