using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiControlApi.Devices;

namespace MobiControlApi.UnitTestMs.Devices
{
    [TestClass]
    public class CollectedDataTest
    {

        string DeviceId = "111111111111111";
        string response1 = @"[
                                              {
                                                ""$type"": ""CollectedInteger"",
                                                ""Kind"": ""Integer"",
                                                ""Value"": 86,
                                                ""DeviceId"": ""357539090249448"",
                                                ""Timestamp"": ""2019-11-15T10:30:02.653Z"",
                                                ""StatTypeId"": ""BatteryStatus""
                                              },
                                              {
                                                ""$type"": ""CollectedInteger"",
                                                ""Kind"": ""Integer"",
                                                ""Value"": 94,
                                                ""DeviceId"": ""357539090249448"",
                                                ""Timestamp"": ""2019-11-15T11:00:00.05Z"",
                                                ""StatTypeId"": ""BatteryStatus""
                                              },
                                              {
                                                ""$type"": ""CollectedInteger"",
                                                ""Kind"": ""Integer"",
                                                ""Value"": 100,
                                                ""DeviceId"": ""357539090249448"",
                                                ""Timestamp"": ""2019-11-15T11:30:01.787Z"",
                                                ""StatTypeId"": ""BatteryStatus""
                                              },
                                              {
                                                ""$type"": ""CollectedInteger"",
                                                ""Kind"": ""Integer"",
                                                ""Value"": 99,
                                                ""DeviceId"": ""357539090249448"",
                                                ""Timestamp"": ""2019-11-15T13:00:37.053Z"",
                                                ""StatTypeId"": ""BatteryStatus""
                                              },
                                              {
                                                ""$type"": ""CollectedInteger"",
                                                ""Kind"": ""Integer"",
                                                ""Value"": 98,
                                                ""DeviceId"": ""357539090249448"",
                                                ""Timestamp"": ""2019-11-15T13:30:00.017Z"",
                                                ""StatTypeId"": ""BatteryStatus""
                                              }
                                            ]";


        [TestMethod]
        public async Task GetCollectedDataForDeviceAsync()
        {
            #region Arrange
            MockApi mockApi = new MockApi();
            mockApi.GetJsonAsyncResult = response1;
            DateTimeOffset startDate = new DateTimeOffset(new DateTime(2019, 11, 01, 00, 00, 00));
            DateTimeOffset stopDate = new DateTimeOffset(new DateTime(2019, 11, 10, 00, 00, 00));
            #endregion

            #region Act
            List<CollectedDataModel> result = await CollectedData.GetCollectedDataForDeviceAsync(mockApi, DeviceId, startDate, stopDate, "BatteryStatus", null);
            #endregion

            #region Assert
            Assert.AreEqual(5, result.Count);
            #endregion
        }



    }

    public class MockApi : IWebApi
    {
        public string GetJsonAsyncResult;

        public Task<string> GetJsonAsync(string resourcePath)
        {
            Console.WriteLine("Encoded resourcePath is " + WebUtility.UrlEncode(resourcePath));

            return Task.FromResult(GetJsonAsyncResult);
           
        }

        public Task<bool> PostAsync(string resourcePath, string body, string ContentType)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PostJsonAsync(string resourcePath, string body)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PutAsync(string resourcePath, string body, string ContentType)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PutJsonAsync(string resourcePath, string body)
        {
            throw new NotImplementedException();
        }
    }

}
