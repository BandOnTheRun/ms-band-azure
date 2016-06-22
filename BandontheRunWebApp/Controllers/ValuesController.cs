using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.ServiceBus;


namespace BandontheRunWebApp.Controllers
{
    using Microsoft.Azure.Devices;
    using Microsoft.Azure.Devices.Common.Exceptions;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class ValuesController : ApiController
    {
        // track known devices 
        public static Dictionary<string, string> _IoTRegisteredDevices = new Dictionary<string, string>();


        // GET api/values


        // IoT Hub device register code

        [Route("api/IoTRegisterDevice/{deviceId}")]
        [HttpGet]
        public async Task<string> IoTRegisterDevice(string deviceId)
        {
            // register device with IoT Hub
            string connectionString = WebApiApplication.ehWebConsumerGroup.iotHubConnectionString;

            RegistryManager registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            string key = await AddDeviceAsync(registryManager, deviceId);

            // track list of devices for reporting
            _IoTRegisteredDevices[deviceId] = DateTime.Now.ToString();

            return key;
        }


        // internal helper function 
        private async static Task<string> AddDeviceAsync(RegistryManager registryManager, string deviceId)
        {
            Device device;

            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }

            return device.Authentication.SymmetricKey.PrimaryKey;
        }




        // list out known devices that we manage 
        [Route("api/GetDevicesInfo")]
        [HttpGet]
        public string GetDevicesInfo()
        {
            //List<DeviceTelemetry> r = new List<DeviceTelemetry>();
            //foreach (KeyValuePair<string, DeviceTelemetry> kvp in WebsiteEventProcessor._devices)
            //{
            //    // do we need to check anything before returning?
            //   r.Add(kvp.Value);
            //

            var json = JsonConvert.SerializeObject(_IoTRegisteredDevices);           
            return json;
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
