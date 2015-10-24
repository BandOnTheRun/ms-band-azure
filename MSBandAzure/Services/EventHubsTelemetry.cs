using Microsoft.ApplicationInsights.DataContracts;
using MSBandAzure.Models;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Web.Http;


namespace MSBandAzure.Services
{
    public class EventHubsTelemetry : ITelemetry
    {
        private string bandTokenServiceLocation;

        public EventHubsTelemetry()
        {
            bandTokenServiceLocation = "http://bandontherun.azurewebsites.net/api/getsastoken/dxband";

            RefreshTokenAsync().ContinueWith(t => { });
        }

        private string _sas;

        public async Task RefreshTokenAsync()
        {
            var http = new HttpClient();
            var resp = await http.GetAsync(new Uri(bandTokenServiceLocation));
            resp.EnsureSuccessStatusCode();
            _sas = await resp.Content.ReadAsStringAsync();
            _sas = _sas.Trim('"');
        }

        public async Task<HttpResponseMessage> PostTelemetryAsync(DeviceTelemetry deviceTelemetry)
        {
            var sas = _sas;

            // Namespace info.
            var serviceNamespace = "bandontherun-ns";
            var hubName = "msbands";

            var url = string.Format("{0}/publishers/{1}/messages", hubName, "dxband"/*deviceTelemetry.DeviceId*/);
            var uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = string.Format("{0}.servicebus.windows.net/", serviceNamespace);
            uriBuilder.Path = url;

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Authorization", sas);

            var postContent = new HttpStringContent(JsonConvert.SerializeObject(deviceTelemetry),
                Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

            Microsoft.ApplicationInsights.TelemetryClient client = new Microsoft.ApplicationInsights.TelemetryClient();
            client.TrackEvent(new EventTelemetry { Name = "Event Hub Post" });

            HttpResponseMessage resp = null;
            try
            {
                resp = await httpClient.PostAsync(uriBuilder.Uri, postContent);
            }
            catch (Exception ex)
            {
                client.TrackException(new ExceptionTelemetry { Exception = ex });
            }
            return resp;
        }
    }
}
