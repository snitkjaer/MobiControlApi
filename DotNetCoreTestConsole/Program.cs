using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MobiControlApi;
using ConsoleTables;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using MobiControlApi.Devices.DeviceCertificates;

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

                BasicDeviceWithCertificatesList basicDevicsWithCerts = new BasicDeviceWithCertificatesList(mcApi);

                List<BasicDeviceWithCertificates> devices = await basicDevicsWithCerts.GetAllDevicesCertificatesAsync("/", true);

                Console.WriteLine("Got " + devices.Count + " devices from SOTI");


                // Display result
                var consoleTable = new ConsoleTable("Name", "ID", "IMEI", "Path", "Model", "Kind", "DeviceCertCount");
                foreach (BasicDeviceWithCertificates dwc in devices)
                {
                    var device = dwc.basicDevice;
                    consoleTable.AddRow(device.DeviceName, device.DeviceId, device.ImeiMeidEsn, device.Path, device.Model, device.Kind, dwc.deviceCertificates.Count);
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
