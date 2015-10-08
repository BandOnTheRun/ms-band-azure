using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace DataSync
{
    public class EventHubsTelemetry : ITelemetry
    {
        public EventHubsTelemetry()
        {
            RefreshTokenAsync().ContinueWith(t => { });
        }

        private string _sas;

        public async Task RefreshTokenAsync()
            {
            var http = new HttpClient();
            var resp = await http.GetAsync(new Uri("http://bandontherun.azurewebsites.net/api/getsastoken/dxband"));
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
            postContent.Headers.Add("ContentType", "application/atom+xml;type=entry;charset=utf-8");
            var resp = await httpClient.PostAsync(uriBuilder.Uri, postContent);
            return resp;
        }
    }
}
