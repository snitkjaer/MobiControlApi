using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiControlApi.UnitTestMs
{
    [TestClass]
    public class DevicesTest
    {
        static CancellationToken token;
        static MobiControlApiConfig mobiControlApiConfig = MobiControlApiConfig.GetConfigFromJsonFile("MobiControlServerApiConfig.json");
        

        [TestMethod]
        public async System.Threading.Tasks.Task TestGetDeviceListAsync()
        {
            #region Arrange
            Api mcApi = new Api(mobiControlApiConfig, null, token);

            #endregion

            #region Act
            List<Device> devices = await mcApi.GetDeviceListAsync("/Steward", false);

            #endregion

            #region Assert
            Assert.IsTrue(devices.Count > 10 );
            #endregion
        }
    }
}
