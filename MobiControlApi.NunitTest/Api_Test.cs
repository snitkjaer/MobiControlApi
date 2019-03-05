using NUnit.Framework;
using System;
using MobiControlApi;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace MobiControlApi.NunitTest
{
    [TestFixture()]
    public class Api_Test
    {
   

        [Test()]
        // using /devices
        public async Task DeviceCountSqlDb()
        {
            #region Setup conditions
            CancellationToken token;

            MobiControlApiConfig mobiControlApiConfig = MobiControlApiConfig.GetConfigFromJsonFile("MobiControlServerApiConfig.json");
            #endregion


            #region Execute code
            // SOTI Server API
            Api mcApi = new Api(mobiControlApiConfig);

            // Get list of devices
            List<Device> devices = await mcApi.GetDeviceListAsync("/Zebra TC56/Drift", token);

            #endregion


            #region Make assertion(s) on the result
            // This value must match the number of devices in at given group on the server
            Assert.AreEqual(112, devices.Count);

            #endregion

        }

        // using /devices/search
        [Test()]
        public async Task DeviceCountSearchDb()
        {
            #region Setup conditions
            CancellationToken token;

            MobiControlApiConfig mobiControlApiConfig = MobiControlApiConfig.GetConfigFromJsonFile("MobiControlServerApiConfig.json");
            #endregion


            #region Execute code
            // SOTI Server API
            Api mcApi = new Api(mobiControlApiConfig);
            mcApi.useSearchDbToGetDevices = true;

            // Get list of devices
            List<Device> devices = await mcApi.GetDeviceListAsync("/Zebra TC56/Drift", token);

            #endregion


            #region Make assertion(s) on the result
            // This value must match the number of devices in at given group on the server
            Assert.AreEqual(112, devices.Count);

            #endregion

        }














        /*
                 private static FluentMockServer _server;
         using WireMock.Matchers;
using WireMock.Server;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

                 [SetUp]
        public void StartMockServer()
        {
            _server = FluentMockServer.Start();
        }
         *         
        [Test()]
        public async Task DeviceCountStub()
        {
            #region Setup conditions
            CancellationToken token;

            // devices?path=%255C%255CZebra%2520TC56%255CDrift&skip=0&take=50
            // 

            _server
                .Given(Request.Create().WithPath("/devices").UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithBody(@"{ msg: ""Hello world!""}"));
            MobiControlApiConfig mobiControlApiConfig = MobiControlApiConfig.GetConfigFromJsonFile("MobiControlServerApiConfig.json");
            #endregion


            #region Execute code
            // SOTI Server API
            Api mcApi = new Api(mobiControlApiConfig);

            // Get list of devices
            List<Device> devices = await mcApi.GetDeviceListAsync("/Zebra TC56/Drift", token);

            #endregion


            #region Make assertion(s) on the result
            // This value must match the number of devices in at given group on the server
            Assert.AreEqual(123, devices.Count);

            #endregion

        }
        */

    }
}
