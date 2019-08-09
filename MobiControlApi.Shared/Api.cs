using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;

namespace MobiControlApi
{
    public partial class Api : LogAbstraction
    {
        private readonly CancellationToken cancellationToken;
        private readonly MobiControlApiConfig config;

        /*
        private readonly IHttpClientFactory httpClientFactory;

        ServiceCollection services = new ServiceCollection();
        ServiceProvider serviceProvider;
        */
        //private static readonly HttpClient httpClient = new HttpClient();
        private readonly Authentication authentication;

        private static TimeSpan httpTimeout = new TimeSpan(0, 0, 20);  // 20 sec

        // SOTI API can accecss device from either the search DB og the traditional DB
        public bool useSearchDbToGetDevices = true;

        // 

        // Main constructor
        public Api(MobiControlApiConfig mobiControlApiConfig, TelemetryClient tc, CancellationToken ct)
        {
            this.tc = tc;

            // Create config object
            config = mobiControlApiConfig;

            // Save CancellationToken
            cancellationToken = ct;

            // Create SOTI Authentication object
            authentication = new Authentication(config, cancellationToken);

            /*
            // Register a HTTP Client
            services.AddHttpClient<SotiHttpClient>();
            serviceProvider = services.BuildServiceProvider();
            */

            // Start the monitoring task
            StartMonitor().ConfigureAwait(false);
        }

        //
        // Alternative constructor overloads
        //
        public Api(string FQDN, string ClientId, string ClientSecret, string Username, string Password, TelemetryClient tc, CancellationToken ct)
            : this(new MobiControlApiConfig(FQDN, ClientId, ClientSecret, Username, Password), tc, ct)
        { }

        public Api(string FQDN, string ClientId, string ClientSecret, string Username, string Password, CancellationToken ct)
            : this(new MobiControlApiConfig(FQDN, ClientId, ClientSecret, Username, Password), null, ct)
        { }

        public Api(JObject jsonConfig, TelemetryClient tc, CancellationToken ct)
            : this(MobiControlApiConfig.GetConfigFromJObject(jsonConfig), tc, ct)
        { }

        public Api(string jsonConfig, TelemetryClient tc, CancellationToken ct)
            : this(MobiControlApiConfig.GetConfigFromJsonString(jsonConfig), tc, ct)
        { }

        List<Task> listTask = new List<Task>();

        public async Task StartMonitor()
        {
            try
            {
                // TODO add SOTI API state monitor

                // Cache
                if (CacheDevices)
                    listTask.Add(UpdateCachedDeviceListOnInterval());

                Log("Started SOTI API monitor", SeverityLevel.Information);

                // Wait for all tasks to complete
                await Task.WhenAll(listTask);

            }
            catch (Exception ex)
            {
                Log("Exception starting MobiControl API monitor", SeverityLevel.Error);
                TrackException(ex);
            }

        }


        // This is the lowest level of the API - accepting raw resource path and retruning a json string
        #region low level SOTI REST Web API 

        // Get
        public async Task<string> GetJsonAsync(string resourcePath)
        {

            // Create http request 
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(config.baseUri, resourcePath)
            };

            // Send http request
            HttpResponseMessage response = await SendSotiRequest(request);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
                return null;

        }

        // Post
        public async Task<bool> PostJsonAsync(string resourcePath, string body)
        {

            // Create http request 
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(config.baseUri, resourcePath),
                Content = new StringContent(body),


            };
            request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json") { CharSet = "UTF-8" };


            HttpResponseMessage response = await SendSotiRequest(request);
            if (response.IsSuccessStatusCode)
                return true;
            else
                return false;

        }

        // Put json
        public async Task<bool> PutJsonAsync(string resourcePath, string body)
        {
            return await PutAsync(resourcePath, body, "application/json");

        }

        // Put
        public async Task<bool> PutAsync(string resourcePath, string body, string ContentType)
        {
            // Create http request 
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(config.baseUri, resourcePath),
                Content = new StringContent(body),


            };
            request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue(ContentType) { CharSet = "UTF-8" };

            HttpResponseMessage response = await SendSotiRequest(request);
            if (response.IsSuccessStatusCode)
                return true;
            else
                return false;
        }

        // Put
        public async Task<bool> PostAsync(string resourcePath, string body, string ContentType)
        {
            // Create http request 
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(config.baseUri, resourcePath),
                Content = new StringContent(body),


            };
            request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue(ContentType) { CharSet = "UTF-8" };

            HttpResponseMessage response = await SendSotiRequest(request);
            if (response.IsSuccessStatusCode)
                return true;
            else
                return false;
        }

        // Send request to SOTI API
        private async Task<HttpResponseMessage> SendSotiRequest(HttpRequestMessage request)
        {
            /*
            // Get a HTTP Client and make a request
            var sotiClient = serviceProvider.GetRequiredService<SotiHttpClient>();
            return await sotiClient.Get(request, cancellationToken);
              */

            HttpResponseMessage response;

            // Get httpclient for SOTI mobicontrol
            using (HttpClient httpClient = await GetSotiHttpClient(authentication))
            {
                response = await httpClient.SendAsync(request, cancellationToken);
            }

            // If error log it
            if (!response.IsSuccessStatusCode)
            {
                Log("Http request error with status code " + response.StatusCode + " with reason" +
                    " " + response.ReasonPhrase, SeverityLevel.Error);
            }

            return response;

        }

        // Create new httpclient for SOTI mobicontrol
        // TODO can we reuse the httpclient??  https://medium.com/@nuno.caneco/c-httpclient-should-not-be-disposed-or-should-it-45d2a8f568bc
        //  Maybe use HttpClientFactory??  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-2.2
        private static async Task<HttpClient> GetSotiHttpClient(Authentication authentication)
        {
            string sotiToken = await authentication.GetAuthenticationToken();
            if (sotiToken == null)
                return null;

            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = httpTimeout;
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + sotiToken);

            return httpClient;
        }




        // Get device list for specific group using /device MC 13+ API (reads directly from SOTI SQL DB)
        private async Task<string> GetDeviceListJsonFromSotiDbAsync(string deviceGroupPath, int skip, int take)
        {
            deviceGroupPath = deviceGroupPath.Replace(" ", "%2520").Replace("/", "%255C");

            // Generate resourcePath
            string resourcePath = "devices?path=%255C" + deviceGroupPath; // "MARK%255CZebra%2520TC56";

            resourcePath +=
                "&skip=" + skip.ToString()
                + "&take=" + take.ToString();

            // Call GetJsonAsync
            return await GetJsonAsync(resourcePath);

        }

        // Get device list for specific group using /device/search MC 14+ API (reads elasticsearch DB)
        private async Task<string> GetDeviceListJsonSearchDbAsync(string deviceGroupPath, string filter, bool includeSubgroups, bool verifyAndSync, int skip, int take)
        {

            // Generate resourcePath
            //string resourcePath = "devices/search?path=%255C" + deviceGroupPath.Replace(" ", "%2520").Replace("/", "%255C");

            string resourcePath = "devices/search?path=" + deviceGroupPath;

            if (!String.IsNullOrEmpty(filter))
                resourcePath += "&filter=" + filter.Replace(" ", "%2520").Replace("/", "%255C");

            // if (includeSubgroups)

            resourcePath += "&includeSubgroups=" + includeSubgroups.ToString().ToLower();

            resourcePath +=
                "&skip=" + skip.ToString()
                + "&take=" + take.ToString();

            // Call GetJsonAsync
            return await GetJsonAsync(resourcePath);


        }

        #endregion

        // Get list of device id's
        public async Task<List<string>> GetDeviceIdListAsync(string deviceGroupPath, bool includeSubgroups)
        {
            if (CacheDevices)
                return await GetCacheDeviceIDListAsync(deviceGroupPath, includeSubgroups);
            else
                return await GetDeviceIdListFromSotiAsync(deviceGroupPath, includeSubgroups);
        }

        // Get list of device id's
        public async Task<List<string>> GetDeviceIdListFromSotiAsync(string deviceGroupPath, bool includeSubgroups)
        {
            List<string> listDeviceIds = new List<string>();

            int deviceOffset = 0;
            int deviceBatchSize = 50;
            string resultJson = null;

            while (true)
            {
                // Get devices in SOTI folder
                if (useSearchDbToGetDevices)
                    resultJson = await GetDeviceListJsonSearchDbAsync(deviceGroupPath, null, includeSubgroups, true, deviceOffset, deviceOffset + deviceBatchSize);
                else
                    resultJson = await GetDeviceListJsonFromSotiDbAsync(deviceGroupPath, deviceOffset, deviceOffset + deviceBatchSize);

                // If we got a result - parse it
                if (resultJson != null)
                {
                    // String to json array
                    JArray devices = JArray.Parse(resultJson);

                    // Itterate over device found
                    foreach (JObject device in devices)
                    {
                        // deviceId
                        string deviceId = (string)device["DeviceId"];

                        if (!String.IsNullOrEmpty(deviceId))
                            listDeviceIds.Add(deviceId);
                    }

                    // Do we expect more devices?
                    if (devices.Count == deviceBatchSize)
                        deviceOffset += deviceBatchSize;
                    else
                        break;
                }
                else
                    break;


            }

            return listDeviceIds;
        }

        // Get list of devices
        public async Task<List<Device>> GetDeviceListAsync(string deviceGroupPath, bool includeSubgroups)
        {
            if (CacheDevices)
                return await GetCacheDeviceListAsync(deviceGroupPath, includeSubgroups);
            else
                return await GetDeviceListFromSotiAsync(deviceGroupPath, includeSubgroups);
        }

        // Get list of devices
        public async Task<List<Device>> GetDeviceListFromSotiAsync(string deviceGroupPath, bool includeSubgroups)
        {
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

            List<Device> listDevices = new List<Device>();

            int deviceOffset = 0;
            int deviceBatchSize = 50;
            string resultJson = null;

            try
            {
                stopWatch.Start();

                // SOTI API only returns a batch of 50 devices - continue to itterate over device batche
                while (true)
                {
                    // Get devices in SOTI folder
                    if (useSearchDbToGetDevices)
                        resultJson = await GetDeviceListJsonSearchDbAsync(deviceGroupPath, null, false, true, deviceOffset, deviceOffset + deviceBatchSize);
                    else
                        resultJson = await GetDeviceListJsonFromSotiDbAsync(deviceGroupPath, deviceOffset, deviceOffset + deviceBatchSize);


                    // If we got a result - parse it
                    if (resultJson != null)
                    {
                        // String to json array
                        JArray devices = JArray.Parse(resultJson);

                        // Itterate over device found
                        foreach (JObject deviceJson in devices)
                        {
                            // parse device
                            Device device = Device.FromJson(deviceJson.ToString());

                            if (device != null)
                                listDevices.Add(device);
                        }

                        // Do we expect more devices?
                        if (devices.Count == deviceBatchSize)
                            deviceOffset += deviceBatchSize;
                        else
                            break;
                    }
                    else
                        break;


                }

                // Stop
                stopWatch.Stop();


                var properties = new Dictionary<string, string>
                             {
                                 { "GroupPath", deviceGroupPath },
                                 { "CurrentDeviceCount",listDevices.Count.ToString()}
                             };
                TrackEvent("SotiGetGroupDeviceList", stopWatch.Elapsed, properties);

            }
            catch (Exception ex)
            {
                // Stop
                stopWatch.Stop();

                Log("Exception getting device list for '" + deviceGroupPath + "' in " + stopWatch.Elapsed.ToString(), SeverityLevel.Error);
                TrackException(ex);
            }




            return listDevices;


        }



    }
}
