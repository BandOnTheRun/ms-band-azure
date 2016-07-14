using System;
using System.Text;
using System.Threading.Tasks;
using MSBandAzure.Models;
using Microsoft.Azure.Devices.Client;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;
using System.Threading;

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
                    Debug.WriteLine(payload);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public async Task RefreshIotHubTokenAsync(string deviceId)
        {
#if EXCLUSIVE_DEVICE
            // Note. this check is made as we only want one device to send data to iot
            // hub currently as there is an 8000 message limit on the free tier - to 
            // remove this behaviour comment out the following line.
            if (!string.IsNullOrEmpty(_exclusiveDevice))
                return;

            _exclusiveDevice = deviceId;
#endif
            // Construct a uri to register this particular device with iot hub
            UriBuilder builder = new UriBuilder();
            builder.Scheme = "http";
            builder.Host = "bandontherunwebapp.azurewebsites.net";

            //TODO: fix this hack
            deviceId = deviceId.Replace(' ', '-');
            deviceId = deviceId.Replace(':', '-');

            builder.Path = "api/IoTRegisterDevice/" + WebUtility.UrlEncode(deviceId);

            HttpResponseMessage resp;

            using (var http = new HttpClient(new RetryHandler(new HttpClientHandler(), 3)))
            {
                resp = await http.GetAsync(builder.Uri);
            }

            resp.EnsureSuccessStatusCode();
            var deviceToken = await resp.Content.ReadAsStringAsync();
            deviceToken = deviceToken.Trim('"');

            try
            {
                _iotHubClient = DeviceClient.Create("BandOnTheRunHub.azure-devices.net",
                    new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceToken), TransportType.Amqp);
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

    // Really dumb retry handler - please replace me at some future time :-)
    internal class RetryHandler : DelegatingHandler
    {
        private int _numRetries;
        public RetryHandler(HttpClientHandler inner, int NumRetries)
        {
            InnerHandler = inner;
            _numRetries = NumRetries;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            while (_numRetries-- > 0)
            {
                try
                {
                    response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {

                }
                cancellationToken.ThrowIfCancellationRequested();
                if (response != null && response.IsSuccessStatusCode)
                    return response;
            }

            return response;
        }
    }
}