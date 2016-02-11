﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedDevice
{
    using Microsoft.Azure.Devices.Client;
    using Newtonsoft.Json;
    using System.Threading;


    // telemetry payload 
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
        static DeviceClient deviceClient;

        static string iotHubUri = "xxxxxxxxxxxxxxx.azure-devices.net";
        static string deviceKey = "xxxxxxx=";
        static string deviceName = "Device1";

        static int _interations = 10;
        static int _sendIntervalms = 2000;
        static int _randomSeed = 0;
        static int _stepSize = 1;


        static void Main(string[] args)
        {
            Console.WriteLine("Simulate Band device {0}\n", deviceName);

            // connect to IoT Hub

            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceName, deviceKey));

            // set up simulated msband data

            Random rnd = new Random();
            if (_randomSeed > 0)
                rnd = new Random(_randomSeed);

            DeviceTelemetry telemetry = new DeviceTelemetry();
            telemetry.DeviceId = deviceName;
            telemetry.UVIndex = 0;
            telemetry.SkinTemp = 0;
            telemetry.HeartRate = 70;

            // send data to IoT Hub 

            for (int i = 0; i < _interations; i++)
            {
                // update random telemetry
                int step = rnd.Next(0 - _stepSize, _stepSize + 1);
                telemetry.HeartRate += step;
                telemetry.Timestamp = DateTime.Now.ToString();

                // transmit telemetry 
                Console.WriteLine("{3}> sending data #{0} for band {2} = '{1}'", i, JsonConvert.SerializeObject(telemetry), deviceName, DateTime.Now);
                SendTelemetryAsync(telemetry);

                // wait
                Thread.Sleep(_sendIntervalms);
            }

            Console.ReadLine();
        }



        // send telemetry to IoTHub
        private static async void SendTelemetryAsync(DeviceTelemetry telemetry)
        {
            var messageString = JsonConvert.SerializeObject(telemetry);
            var message = new Message(Encoding.ASCII.GetBytes(messageString));
            await deviceClient.SendEventAsync(message);
        }




        // send a new device-to-cloud message every second, contaning a JSON-serialized object with the deviceId and a randomly generated number, 
        //  representing a simulated wind speed sensor
        private static async void SendDeviceToCloudMessagesAsync()
        {
            double avgWindSpeed = 10; // m/s
            Random rand = new Random();

            while (true)
            {
                double currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;

                var telemetryDataPoint = new
                {
                    deviceId = deviceName,
                    windSpeed = currentWindSpeed
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message from device to IoT Hub: {1}", DateTime.Now, messageString);

                Thread.Sleep(1000);
            }
        }


        private static async void ReceiveC2dAsync()
        {
            Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                Message receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null)
                    continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                Console.ResetColor();

                // notify IoT Hub that the message has been successfully processed and can be safely removed from the device queue
                await deviceClient.CompleteAsync(receivedMessage);
            }
        }


        private static async void SendDeviceToCloudInteractiveMessagesAsync()
        {
            while (true)
            {
                var interactiveMessageString = "Alert message!";

                // create message
                var interactiveMessage = new Message(Encoding.ASCII.GetBytes(interactiveMessageString));
                interactiveMessage.Properties["messageType"] = "interactive";  // user property 
                interactiveMessage.MessageId = Guid.NewGuid().ToString();  // system property

                // send message 
                await deviceClient.SendEventAsync(interactiveMessage);
                Console.WriteLine("{0} > Sending interactive message: {1}", DateTime.Now, interactiveMessageString);

                // snooze
                Thread.Sleep(10000);
            }
        }
    }
}
