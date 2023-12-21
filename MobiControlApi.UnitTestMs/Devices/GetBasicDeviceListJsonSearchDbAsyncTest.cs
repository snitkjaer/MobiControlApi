﻿using System;
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
        [DataRow("/", true)]
        [DataRow("//", true)]
        [DataRow("", true)]
        [DataRow(TestData.groupName, true)]
        public async Task GetBasicDeviceListJsonSearchDbAsyncTest_CountRoot(string groupPath, bool includeSubGroups)
        {
            #region Arrange
            Api mcApi = new Api(mobiControlApiConfig, token, httpClient);
            String responseJson = await mcApi.GetDeviceListJsonSearchDbAsync(groupPath, null, includeSubGroups, false, 0, 1000);
            int noDevices = Regex.Matches(responseJson, "DeviceId").Count;
            #endregion

            #region Act
            List<BasicDevice> devices = await mcApi.GetBasicDeviceListJsonSearchDbAsync(groupPath, null, includeSubGroups, false, 0, 1000);
            #endregion

            #region Assert
            Assert.AreEqual(noDevices, devices.Count);
            #endregion
        }
    }

}
