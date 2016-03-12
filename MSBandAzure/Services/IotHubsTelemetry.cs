using System;
using System.Text;
using System.Threading.Tasks;
using MSBandAzure.Models;
using Microsoft.Azure.Devices.Client;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;

namespace MSBandAzure.Services
{
    public class IotHubsTelemetry : ITelemetry
    {
        private static string _exclusiveDevice = string.Empty;

        private DeviceClient _iotHubClient;

        public async Task PostTelemetryAsync(DeviceTelemetry deviceTelemetry)
        {
            if (_iotHubClient != null)
            {
                var payload = JsonConvert.SerializeObject(deviceTelemetry);
                try
                {
                    await _iotHubClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes(payload)));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public async Task RefreshIotHubTokenAsync(string deviceId)
        {
            // Note. this check is made as we only want one device to send data to iot
            // hub currently as there is an 8000 message limit on the free tier - to 
            // remove this behaviour comment out the following line.
            if (!string.IsNullOrEmpty(_exclusiveDevice))
                return;

            _exclusiveDevice = deviceId;

            // Construct a uri to register this particular device with iot hub
            UriBuilder builder = new UriBuilder();
            builder.Scheme = "http";
            builder.Host = "bandontherunwebapp.azurewebsites.net";

            //TODO: fix this hack
            deviceId = deviceId.Replace(' ', '-');
            deviceId = deviceId.Replace(':', '-');

            builder.Path = "api/IoTRegisterDevice/" + WebUtility.UrlEncode(deviceId);

            HttpResponseMessage resp;

            using (var http = new HttpClient())
            {
                resp = await http.GetAsync(builder.Uri);
            }

            resp.EnsureSuccessStatusCode();
            var deviceToken = await resp.Content.ReadAsStringAsync();
            deviceToken = deviceToken.Trim('"');

            try
            {
                _iotHubClient = DeviceClient.Create("BandOnTheRun-IoTHub.azure-devices.net", 
                    new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceToken), TransportType.Http1);
            }
            catch (Exception ex)
            {

            }
        }

        public Task RefreshTokenAsync(string deviceId)
        {
            return RefreshIotHubTokenAsync(deviceId);
        }
    }

    internal class RetryHandler : HttpClientHandler
    {

    }
}
