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

        static int expectedNoOfDeviceOnTestServer = 150;


        static CancellationToken token;

        static MobiControlApiConfig mobiControlApiConfig = MobiControlApiConfig.GetConfigFromJsonFile("MobiControlServerApiConfig.json");

        // SOTI Server API
        Api mcApi = new Api(mobiControlApiConfig, null, token);



        [Test()]
        // using /devices (the version 13+ methode)
        public async Task DeviceCountSqlDb()
        {
            #region Setup conditions
            #endregion


            #region Execute code
            mcApi.useSearchDbToGetDevices = false;

            // Get list of devices
            List<Device> devices = await mcApi.GetDeviceListAsync("/Zebra TC56/Drift", false);

            #endregion


            #region Make assertion(s) on the result
            // This value must match the number of devices in at given group on the server
            Assert.AreEqual(expectedNoOfDeviceOnTestServer, devices.Count);

            #endregion

        }

        // using /devices/search (the version 14+ methode)
        [Test()]
        public async Task DeviceCountSearchDb()
        {
            #region Setup conditions
            #endregion


            #region Execute code
            mcApi.useSearchDbToGetDevices = true;

            // Get list of devices
            List<Device> devices = await mcApi.GetDeviceListAsync("/Zebra TC56/Drift", false);

            #endregion


            #region Make assertion(s) on the result
            // This value must match the number of devices in at given group on the server
            Assert.AreEqual(expectedNoOfDeviceOnTestServer, devices.Count);

            #endregion

        }


        // Get / set device custom attribute
        [Test()]
        public async Task GetSetDeviceCustomAttibute()
        {
            #region Setup conditions
            string deviceId = "353857081083640";
            string CustomAttributeName = "RkCertStatus";
            string NewValue = "test" + DateTime.Now.ToFileTime();
            #endregion


            #region Execute code
            // Get attrubute
            string beforeValue = await mcApi.GetCustomAttributeValueAsync(deviceId, CustomAttributeName);
            

            // Set attribute
            bool setResult = await mcApi.SetCustomAttributeAsync(deviceId, CustomAttributeName, NewValue);

            // Get attrubute
            string afterValue = await mcApi.GetCustomAttributeValueAsync(deviceId, CustomAttributeName);



            #endregion


            #region Make assertion(s) on the result
            // This value must match the number of devices in at given group on the server
            Assert.AreEqual(NewValue, afterValue);

            #endregion

        }



        // Test action
        [Test()]
        public async Task SetGetDeviceCustomAttibute()
        {
            #region Setup conditions
            string deviceId = "353857081083640";
            #endregion


            #region Execute code

            // Set attribute
            bool result = await mcApi.SendActionToDevicesAsync(deviceId, Api.ActionInfo.CheckIn);

            #endregion


            #region Make assertion(s) on the result
            // This value must match the number of devices in at given group on the server
            Assert.AreEqual(true, result);

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
