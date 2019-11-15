using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MobiControlApi
{
    public class GetObjectListFromWebApi
    {
        public static async Task<List<T>> GetObjectListFromSotiAsync<T>(IWebApi webApi, string resourcePath)
        {

            List<T> tList = new System.Collections.Generic.List<T>();

            int deviceOffset = 0;
            int deviceBatchSize = 50;
            string responseJson = null;


            // SOTI API only returns a batch of 50 devices - continue to itterate over device batche
            while (true)
            {

                responseJson = await GetBatchAsync(webApi, resourcePath, deviceOffset, deviceBatchSize);

                // If we got a result - parse it
                if (responseJson != null)
                {
                    // String to json array
                    JArray jArray = JArray.Parse(responseJson);

                    // Itterate over device found
                    foreach (JObject json in jArray)
                    {
                        // parse
                        var t = JsonConvert.DeserializeObject<T>(json.ToString(), Helpers.Converter.Settings);

                        if (t != null)
                            tList.Add(t);
                    }


                    // Do we expect more devices?
                    if (tList.Count == deviceBatchSize)
                        deviceOffset += deviceBatchSize;
                    else
                        break;

                }
            }

            return tList;

        }


        private static async Task<string> GetBatchAsync(IWebApi webApi, string resourcePath, int skip, int take)
        {
  

            resourcePath +=
                "&skip=" + skip.ToString()
                + "&take=" + take.ToString();

            // Call GetJsonAsync
            return await webApi.GetJsonAsync(resourcePath);

        }

    }
}
