using System;
using System.Threading.Tasks;
using MobiControlApi.Shared;


namespace DotNetCoreTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
			Task t = new Task(BasicTest);
            t.Start();
			Console.ReadLine();
        }

		static async void BasicTest()
		{
            //
            // You need to change these values to match your environment
            //

			// The is fully qualified domain name for the server
			string FQDN = "server.domain.tld";

			// Needs to be generated on SOTI MobiControl server using MCadmin.exe
            string ClientId = "";
            string ClientSecret = "";
            // Create in the SOTI MobiControl user management - must be admin
            string User = "";
            string Password = "";
                     
			try
			{
				Api mcApi = new Api(FQDN, ClientId, ClientSecret, User, Password);
                // Get device groups json
				string resultJson = await mcApi.GetJsonAsync("devicegroups");
				Console.Write(resultJson);
			}
			catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }


		}
    }
}
