using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiControlApi.UnitTestMs.API
{
    [TestClass]
    public class GetDeviceListJsonSearchDbAsyncTest
    {
        static CancellationToken token;
        static MobiControlApiConfig mobiControlApiConfig = MobiControlApiConfig.GetConfigFromJsonFile("MobiControlServerApiConfig.json");


        [TestMethod]
        public async System.Threading.Tasks.Task GetDeviceListAsync_Count()
        {
            #region Arrange
            Api mcApi = new Api(mobiControlApiConfig, null, token);

            #endregion

            #region Act
            List<Device> devices = await mcApi.GetDeviceListAsync("/Steward", true);

            #endregion

            #region Assert
            Assert.IsTrue(devices.Count == 224);
            #endregion
        }
    }
}
