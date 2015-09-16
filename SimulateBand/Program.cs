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
        static int _interations = 10;
        static int _sendIntervalms = 2000;
        static string _bandName = "BandTest1";
        static int _randomSeed = 0;
        static int _stepSize = 1;

        static string sas = "";

        static void Main(string[] args)
        {
            // args

            foreach (var arg in args)
            {
                _bandName = arg;
            }


            // set up read to simulate band

            Random rnd = new Random();
            if (_randomSeed > 0)
                rnd = new Random(_randomSeed);

            DeviceTelemetry telemetry = new DeviceTelemetry();
            telemetry.DeviceId = _bandName;
            telemetry.UVIndex = 0;
            telemetry.SkinTemp = 0;
            telemetry.HeartRate = 70;


            // get SAS token 

            GetSasTokenAsync().Wait();



            // send data

            for (int i = 0; i < _interations; i++)
            {
                // update telemetry

                int step = rnd.Next(0 - _stepSize, _stepSize + 1);
                telemetry.HeartRate += step;
                telemetry.Timestamp = DateTime.Now.ToString();

                // transmit telemetry 

                Console.WriteLine("sending data #{0} for band {2} = '{1}'", i, JsonConvert.SerializeObject(telemetry), _bandName);
                PostTelemetryAsync(telemetry).Wait();

                // wait

                Thread.Sleep(_sendIntervalms);
            }
        }


        static async Task GetSasTokenAsync()
        {
            var http = new HttpClient();
            var resp = await http.GetAsync(new Uri("http://bandontherun.azurewebsites.net/api/getsastoken/" + _bandName));
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
            var url = string.Format("{0}/publishers/{1}/messages", hubName, _bandName);

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
