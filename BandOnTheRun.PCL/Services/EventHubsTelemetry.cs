using MSBandAzure.Models;
using MSBandAzure.Mvvm;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MSBandAzure.Services
{
    public class EventHubsTelemetry : ITelemetry
    {
        private string bandTokenServiceLocation;

        public EventHubsTelemetry()
        {
            bandTokenServiceLocation = VMLocator.Instance.Settings.SasTokenUrl;
            RefreshSasTokenAsync().ContinueWith(t => { });
        }

        private string _sas;
        private HttpClient _httpClient = new HttpClient();
        private Uri _postUri;

        public async Task RefreshSasTokenAsync()
        {
            var http = new HttpClient();
            var resp = await http.GetAsync(new Uri(bandTokenServiceLocation));
            resp.EnsureSuccessStatusCode();
            _sas = await resp.Content.ReadAsStringAsync();
            _sas = _sas.Trim('"');

            // Namespace info.
            var serviceNamespace = VMLocator.Instance.Settings.ServiceNamespace;
            var hubName = VMLocator.Instance.Settings.HubName;

            var url = string.Format("{0}/publishers/{1}/messages", hubName, "dxband"/*deviceTelemetry.DeviceId*/);
            var uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = string.Format("{0}.servicebus.windows.net/", serviceNamespace);
            uriBuilder.Path = url;
            _postUri = uriBuilder.Uri;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", _sas);
        }

        public async Task PostTelemetryAsync(DeviceTelemetry deviceTelemetry)
        {
            var payload = JsonConvert.SerializeObject(deviceTelemetry);

            var postContent = new StringContent(payload, Encoding.UTF8, "application/json");

            //Microsoft.ApplicationInsights.TelemetryClient client = new Microsoft.ApplicationInsights.TelemetryClient();
            //client.TrackEvent(new EventTelemetry { Name = "Event Hub Post" });

            HttpResponseMessage resp = null;
            try
            {
                resp = await _httpClient.PostAsync(_postUri, postContent);
            }
            catch (Exception ex)
            {
                //client.TrackException(new ExceptionTelemetry { Exception = ex });
            }
            return;
        }

        public Task RefreshTokenAsync(string deviceId)
        {
            return RefreshSasTokenAsync();
        }
    }
}
