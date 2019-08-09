using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;

namespace MobiControlApi
{
    public partial class Api : LogAbstraction
    {
        /*
        Content-Type: multipart/related; 
        boundary=foo_bar_baz Content-Length: number_of_bytes_in_entire_request_body

        --foo_bar_baz Content-Type: application/vnd.soti.mobicontrol.package.metadata+json
        { "deviceFamily" : "AndroidPlus" }

        --foo_bar_baz Content-Type: application/vnd.soti.mobicontrol.package 
        Content-Transfer-Encoding: Base64 
        Content-Disposition: attachment; 
        filename="package_file_name.pcg"
        Base64-encoded package data 
        --foo_bar_baz--
         */
        public async Task<bool> UploadPackageAsync(string filePath)
        {
            // POST /packages

            // Generate resourcePath
            string resourcePath = "packages/";

            if (!File.Exists(filePath))
            {
                Log("File not fouund " + filePath, SeverityLevel.Error);
                return false;
            }

            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            string file = Convert.ToBase64String(bytes);


            JObject objDeviceFamily = new JObject(
                new JProperty("deviceFamily", "AndroidPlus"));

            string strDeviceFamily = JsonConvert.SerializeObject(objDeviceFamily);

            // we need to send a request with multipart/form-data
            MultipartFormDataContent multiForm = new MultipartFormDataContent();

            // add API method parameters
            multiForm.Add(new StringContent(strDeviceFamily), "application/vnd.soti.mobicontrol.package.metadata+json");
            multiForm.Add(new StringContent(Path.GetFileName(filePath)), "filename");
            multiForm.Add(new StringContent(file), "file", Path.GetFileName(filePath));

            var uri = new Uri(config.baseUri, resourcePath);

            HttpResponseMessage response;

            // Get httpclient for SOTI mobicontrol
            using (HttpClient httpClient = await GetSotiHttpClient(authentication))
            {
                response = await httpClient.PostAsync(uri, multiForm, cancellationToken);
            }

            // If error log it
            if (!response.IsSuccessStatusCode)
            {
                Log("Http request error with status code " + response.StatusCode + " with reason" +
                    " " + response.ReasonPhrase, SeverityLevel.Error);
            }


            if (response.IsSuccessStatusCode)
                return true;
            else
                return false;
        }

    }
}
