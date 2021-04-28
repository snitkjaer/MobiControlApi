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
using System.Net;

namespace MobiControlApi
{
    public partial class Api : LogAbstraction, IWebApi
    {
        private readonly CancellationToken cancellationToken;
        private readonly MobiControlApiConfig config;

        private readonly HttpClient httpClientInstance;

        private readonly Authentication authentication;

        // SOTI API can accecss device from either the search DB og the traditional DB
        public bool useSearchDbToGetDevices = true;

        // 

        // Main constructor
        public Api(MobiControlApiConfig mobiControlApiConfig, TelemetryClient tc, CancellationToken cancellationToken, HttpClient httpClient)
        {
            // Save
            this.tc = tc;
            this.httpClientInstance = httpClient;
            this.cancellationToken = cancellationToken;

            // Create config object
            config = mobiControlApiConfig;

            // Create SOTI Authentication object
            authentication = new Authentication(config, cancellationToken, httpClient);

            // Initiate the HTTP Client
            Init_httpClient();

        }


        protected void Init_httpClient()
        {
            httpClientInstance.BaseAddress = config.baseUri;
            httpClientInstance.DefaultRequestHeaders.Clear();
            httpClientInstance.DefaultRequestHeaders.ConnectionClose = false;
            ServicePointManager.FindServicePoint(config.baseUri).ConnectionLeaseTimeout = 60 * 1000;
        }


        //
        // Alternative constructor overloads
        //
        /*
        public Api(string FQDN, string ClientId, string ClientSecret, string Username, string Password, TelemetryClient tc, CancellationToken ct, IHttpClientFactory httpClientInstance)
            : this(new MobiControlApiConfig(FQDN, ClientId, ClientSecret, Username, Password), tc, ct, httpClientInstance)
        { }

        public Api(string FQDN, string ClientId, string ClientSecret, string Username, string Password, CancellationToken ct, IHttpClientFactory httpClientInstance)
            : this(new MobiControlApiConfig(FQDN, ClientId, ClientSecret, Username, Password), null, ct, httpClientInstance)
        { }

        public Api(JObject jsonConfig, TelemetryClient tc, CancellationToken ct, IHttpClientFactory httpClientInstance)
            : this(MobiControlApiConfig.GetConfigFromJObject(jsonConfig), tc, ct, httpClientInstance)
        { }

        public Api(string jsonConfig, TelemetryClient tc, CancellationToken ct, IHttpClientFactory httpClientInstance)
            : this(MobiControlApiConfig.GetConfigFromJsonString(jsonConfig), tc, ct, httpClientInstance)
        { }
        */
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

        // Get json
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

        // Post json
        public async Task<bool> PostJsonAsync(string resourcePath, string body)
        {
            // Create http request 
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(config.baseUri, WebUtility.UrlEncode(resourcePath)),
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

        private async Task UpdateHeaders()
        {
            // Add SOTI Authorization header
            httpClientInstance.DefaultRequestHeaders.Clear();
            httpClientInstance.DefaultRequestHeaders.ConnectionClose = false;

            string Token = await authentication.GetAuthenticationToken();
            httpClientInstance.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
        }


        // Send request to SOTI API
        private async Task<HttpResponseMessage> SendSotiRequest(HttpRequestMessage request)
        {
            // Define response message
            HttpResponseMessage response;

            await UpdateHeaders();

            // Call SOTI MobiControl
            response = await httpClientInstance.SendAsync(request, cancellationToken);

            // If error log it
            if (!response.IsSuccessStatusCode)
            {
                Log("Http request error with status code " + response.StatusCode + " with reason" +
                    " " + response.ReasonPhrase, SeverityLevel.Error);
            }

            return response;

        }



        #endregion


    }
}
