using System;
using System.Threading;
using System.Threading.Tasks;
using MobiControlApi;

namespace DotNetCoreTestConsole
{
    class Program
    {
		public static async Task Main(string[] args)
        {
            // Create the token source.
            CancellationTokenSource cts = new CancellationTokenSource();

            //
            // You need to change MobiControlServerApiConfig.json values to match your environment
            //


            var version = typeof(Program).Assembly.GetName().Version.ToString().Trim(new Char[] { '{', '}' });
            Console.WriteLine("Starting version " + version);


            try
			{
                MobiControlApiConfig mobiControlApiConfig = MobiControlApiConfig.GetConfigFromJsonFile("MobiControlServerApiConfig.json");

                // SOTI Server API
                Api mcApi = new Api(mobiControlApiConfig, null, cts.Token);


                while(true)
                {
                    // Get device groups json
                    string resultJson = await mcApi.GetJsonAsync("devicegroups");
                    Console.Write(resultJson);

                    Thread.Sleep(1000);
                }


			}
			catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }



		}
    }
}
