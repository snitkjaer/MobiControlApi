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
            Init_httpClient();

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


            HttpClient httpClient = await GetSotiHttpClient(authentication);
            
            // Get httpclient for SOTI mobicontrol

            response = await httpClient.SendAsync(request, cancellationToken);

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

            
            httpClientInstance.DefaultRequestHeaders.Clear();
            httpClientInstance.DefaultRequestHeaders.Add("Authorization", "Bearer " + sotiToken);

            return httpClientInstance;
        }



        internal static HttpClient httpClientInstance;

        protected void Init_httpClient()
        {

            

            httpClientInstance = new HttpClient();
            httpClientInstance.BaseAddress = config.baseUri;
            httpClientInstance.Timeout = httpTimeout;
            httpClientInstance.DefaultRequestHeaders.Clear();
            httpClientInstance.DefaultRequestHeaders.ConnectionClose = false;
            httpClientInstance.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            ServicePointManager.FindServicePoint(config.baseUri).ConnectionLeaseTimeout = 60 * 1000;
        }


        #endregion


    }
}
