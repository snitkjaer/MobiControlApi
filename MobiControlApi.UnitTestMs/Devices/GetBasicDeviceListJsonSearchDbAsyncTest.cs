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
    public class GetBasicDeviceListJsonSearchDbAsyncTest
    {
        static CancellationToken token;
        static MobiControlApiConfig mobiControlApiConfig = MobiControlApiConfig.GetConfigFromJsonFile("MobiControlServerApiConfig.json");
        HttpClient httpClient = new HttpClient();

        [DataTestMethod]
        [DataRow("/", true, TestData.numberOfDevicesRoot)]
        [DataRow("//", true, TestData.numberOfDevicesRoot)]
        [DataRow("", true, TestData.numberOfDevicesRoot)]
        [DataRow(TestData.groupName, true, TestData.numberOfDevicesGroup)]
        public async Task GetBasicDeviceListJsonSearchDbAsyncTest_CountRoot(string groupPath, bool includeSubGroups, int numberOfDevices)
        {
            #region Arrange
            Api mcApi = new Api(mobiControlApiConfig, token, httpClient);
            #endregion

            #region Act
            List<BasicDevice> devices = await mcApi.GetBasicDeviceListJsonSearchDbAsync(groupPath, null, includeSubGroups, false, 0, 1000);
            #endregion

            #region Assert
            Assert.AreEqual(numberOfDevices, devices.Count);
            #endregion
        }
    }

}
