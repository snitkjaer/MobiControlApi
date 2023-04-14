using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MobiControlApi;
using ConsoleTables;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace DotNetCoreTestConsole
{
    class Program
    {
		public static async Task Main(string[] args)
        {
            // Create the token source.
            CancellationTokenSource cts = new CancellationTokenSource();
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(0, 0, 20);   // 20 sec


            //
            // You need to change MobiControlServerApiConfig.json values to match your environment
            //


            var version = typeof(Program).Assembly.GetName().Version.ToString().Trim(new Char[] { '{', '}' });
                Console.WriteLine("Starting version " + version);


            try
			{
                MobiControlApiConfig mobiControlApiConfig = MobiControlApiConfig.GetConfigFromJsonFile("MobiControlServerApiConfig.json");



                // SOTI Server API
                Api mcApi = new Api(mobiControlApiConfig, null, cts.Token, httpClient);


                List<BasicDevice> devices = await mcApi.GetBasicDeviceListJsonSearchDbAsync("/", null, true, false, 0, 1000);

                Console.WriteLine("Got " + devices.Count + " devices from SOTI");


                // Display result
                var consoleTable = new ConsoleTable("Name", "ID", "IMEI", "Path", "Model", "Kind");
                foreach (var device in devices)
                {
                    consoleTable.AddRow(device.DeviceName, device.DeviceId, device.ImeiMeidEsn, device.Path, device.Model, device.Kind);
                }
                consoleTable.Write();


                Console.ReadLine();


            }
			catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }



		}
    }
}
