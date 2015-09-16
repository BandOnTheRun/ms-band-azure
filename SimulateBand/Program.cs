using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulateBand
{
    using System.Threading;
    using System.Net.Http;
    using Newtonsoft.Json;


    public class DeviceTelemetry
    {
        public DeviceTelemetry()
        {
        }
        public string DeviceId { get; set; }

        public int HeartRate { get; set; }
        public double SkinTemp { get; set; }
        public int UVIndex { get; set; }

        public string Timestamp { get; set; }
    }


    class Program
    {
        static int interations = 10;
        static int sendIntervalms = 2000;
        static string bandName = "BandTest1";
        static int randomSeed = 0;

        static string sas = "";

        static void Main(string[] args)
        {
            // args

            foreach (var arg in args)
            {
                bandName = arg;
            }


            // set up read to simulate band

            Random rnd = new Random();
            if (randomSeed > 0)
                rnd = new Random(randomSeed);

            DeviceTelemetry telemetry = new DeviceTelemetry();
            telemetry.DeviceId = bandName;
            telemetry.UVIndex = 0;
            telemetry.SkinTemp = 0;
            telemetry.HeartRate = 70;


            // get SAS token 

            GetSasTokenAsync().Wait();



            // send data

            for (int i = 0; i < interations; i++)
            {
                // update telemetry

                int step = rnd.Next(-1, 1);
                telemetry.HeartRate += step;
                telemetry.Timestamp = DateTime.Now.ToString();

                // transmit telemetry 

                Console.WriteLine("sending data #{0} for band {2} = '{1}'", i, JsonConvert.SerializeObject(telemetry), bandName);
                PostTelemetryAsync(telemetry).Wait();

                // wait

                Thread.Sleep(sendIntervalms);
            }
        }


        static async Task GetSasTokenAsync()
        {
            var http = new HttpClient();
            var resp = await http.GetAsync(new Uri("http://bandontherun.azurewebsites.net/api/getsastoken/" + bandName));
            resp.EnsureSuccessStatusCode();
            sas = await resp.Content.ReadAsStringAsync();
            sas = sas.Trim('"');

            return;
        }


        static async Task PostTelemetryAsync(DeviceTelemetry deviceTelemetry)
        {
            // Namespace info.
            var serviceNamespace = "bandontherun-ns";
            var hubName = "msbands";
            var url = string.Format("{0}/publishers/{1}/messages", hubName, bandName);

            // Create client.
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(string.Format("https://{0}.servicebus.windows.net/", serviceNamespace))
            };

            var payload = JsonConvert.SerializeObject(deviceTelemetry);

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", sas);  // add sas token

            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            content.Headers.Add("ContentType", "application/atom+xml;type=entry;charset=utf-8");

            // post 

            var ret = await httpClient.PostAsync(url, content);

            return;
        }
    }
}
