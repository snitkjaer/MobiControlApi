using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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


        private static string boundary = "----CustomBoundary" + DateTime.Now.Ticks.ToString("x");

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

            // base64 encoded file stream
            //Stream fs = File.Create(filePath).ConvertToBase64();


            byte[] bytes = File.ReadAllBytes(filePath);
            //string file = Convert.ToBase64String(bytes);

            // var file_content = new ByteArrayContent(new StreamContent(fs).ReadAsByteArrayAsync().Result);


            ByteArrayContent file_content = new ByteArrayContent(bytes);
            file_content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.soti.mobicontrol.package");
            file_content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = Path.GetFileName(filePath)
            };
            file_content.Headers.ContentEncoding.Add("Base64");


            JObject objDeviceFamily = new JObject(
                new JProperty("deviceFamily", "AndroidPlus"));

            string strDeviceFamily = JsonConvert.SerializeObject(objDeviceFamily);

            StringContent Device_content = new StringContent(strDeviceFamily);
            Device_content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.soti.mobicontrol.package.metadata+json");

            // Update authentication header
            await UpdateHeaders();


            using (var formData = new MultipartFormDataContent(boundary))
            {
                formData.Add(Device_content);
                formData.Add(file_content);

                formData.Headers.Remove("Content-Type");
                formData.Headers.TryAddWithoutValidation("Content-Type", "multipart/related; boundary=" + boundary);

                HttpResponseMessage response = await httpClientInstance.PostAsync(resourcePath, formData);
                if (response.IsSuccessStatusCode)
                    return true;
                else
                    return false;

            }






        }

        /*

        public async Task<bool> UploadPackageAsync2(string filePath)
        {
            // POST /packages

            // Generate resourcePath
            string resourcePath = "packages/";

            if (!File.Exists(filePath))
            {
                Log("File not fouund " + filePath, SeverityLevel.Error);
                return false;
            }

            // base64 encoded file stream
            Stream fs = File.Create(filePath).ConvertToBase64();


            //byte[] bytes = File.ReadAllBytes(filePath);
            //string file = Convert.ToBase64String(bytes);


            JObject objDeviceFamily = new JObject(
                new JProperty("deviceFamily", "AndroidPlus"));

            string strDeviceFamily = JsonConvert.SerializeObject(objDeviceFamily);

            // we need to send a request with multipart/form-data
            MultipartFormDataContent multiForm = new MultipartFormDataContent();

            // add API method parameters
            multiForm.Add(new StringContent(strDeviceFamily), "application/vnd.soti.mobicontrol.package.metadata+json");
            multiForm.Add(new StreamContent(fs), "application/vnd.soti.mobicontrol.package");



            // Create http request 
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(config.baseUri, resourcePath),
            };
            request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("multipart/related") { CharSet = "UTF-8" };
             = multiForm;

            HttpResponseMessage response = await SendSotiRequest(request);
            if (response.IsSuccessStatusCode)
                return true;
            else
                return false;

        }
        */





        public async Task UploadPackageAsync3(string path)
        {
            Console.WriteLine("Uploading {0}", path);
            try
            {
                using (var client = new HttpClient())
                {
                    using (var stream = File.OpenRead(path))
                    {
                        var content = new MultipartFormDataContent();
                        var file_content = new ByteArrayContent(new StreamContent(stream).ReadAsByteArrayAsync().Result);
                        file_content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                        file_content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = "screenshot.png",
                            Name = "foo",
                        };
                        content.Add(file_content);
                        client.BaseAddress = new Uri("https://pajlada.se/poe/imgup/");
                        var response = await client.PostAsync("upload.php", content);
                        response.EnsureSuccessStatusCode();
                        Console.WriteLine("Done");
                    }
                }

            }
            catch (Exception)
            {
                Console.WriteLine("Something went wrong while uploading the image");
            }
        }

    }
}
