using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;

using Microsoft.ServiceBus;




namespace BandontheRunWebApp.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values


        // create a SAS token
        [Route("api/GetSasToken/{deviceId}")]
        [HttpGet]
        public string GetSasToken(string deviceId)
        {

            string ns = WebApiApplication.ehWebConsumerGroup.eventHubNamespace;
            string hubName = WebApiApplication.ehWebConsumerGroup.eventHubPath;
            string keyName = WebApiApplication.ehWebConsumerGroup.SendKeyName;
            string key = WebApiApplication.ehWebConsumerGroup.SendKeyValue;

            int TTLmins = 60 * 24;

            if (deviceId == "")
                return "";


             TimeSpan ttl = new TimeSpan(0, TTLmins, 0);

            var sas = CreateForHttpSender(keyName, key, ns, hubName, deviceId, ttl);

            // add app insight
            var tc = new Microsoft.ApplicationInsights.TelemetryClient();
            tc.TrackEvent("SasToken dispensed");
            
            return sas;
        }


        [Route("api/GetDevicesInfo")]
        [HttpGet]
        public List<DeviceTelemetry> GetDevicesInfo()
        {
            List<DeviceTelemetry> r = new List<DeviceTelemetry>();

            foreach (KeyValuePair<string, DeviceTelemetry> kvp in WebsiteEventProcessor._devices)
            {
                // do we need to check anything before returning?
                r.Add(kvp.Value);
            }
            
            return r;
        }



        public static string CreateForHttpSender(string senderKeyName, string senderKey, string serviceNamespace, string hubName, string publisherName, TimeSpan tokenTimeToLive)
        {
            var serviceUri = ServiceBusEnvironment.CreateServiceUri(
                    "https", serviceNamespace, String.Format("{0}/publishers/{1}/messages", hubName, publisherName))
                .ToString()
                .Trim('/');
            
            var sas = SharedAccessSignatureTokenProvider.GetSharedAccessSignature(senderKeyName, senderKey, serviceUri, tokenTimeToLive);
            
            return sas;
        }



        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }



    }
}
