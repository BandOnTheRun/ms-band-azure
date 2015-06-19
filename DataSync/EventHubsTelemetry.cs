﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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

        public Task<HttpResponseMessage> PostTelemetryAsync(DeviceTelemetry deviceTelemetry)
        {
            var sas = _sas;

            // Namespace info.
            var serviceNamespace = "bandontherun-ns";
            var hubName = "msbands";
            var url = string.Format("{0}/publishers/{1}/messages", hubName, deviceTelemetry.DeviceId);

            // Create client.
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(string.Format("https://{0}.servicebus.windows.net/", serviceNamespace))
            };

            var payload = JsonConvert.SerializeObject(deviceTelemetry);

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", sas);

            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            content.Headers.Add("ContentType", "application/atom+xml;type=entry;charset=utf-8");
            return httpClient.PostAsync(url, content);
        }
    }
}
