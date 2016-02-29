using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using System.Threading;

namespace SimulateSignalR
{
    class Program
    {
        static void Main(string[] args)
        {
            // connect to signalr hub

            var connection = new HubConnection("http://bandontheruntracker.azurewebsites.net/");
            var hub = connection.CreateHubProxy("BandOnTheRunHub");
            connection.Start().Wait();

            // start pumping heartrate data

            int rate = 70;
            Random rnd = new Random();

            for (int i = 0; i < 25; i++)
            {
                hub.Invoke<string>("heartrate", "signalrdevice1", rate);
                Console.WriteLine("Heartrate: {0}", rate.ToString());

                rate += rnd.Next(5);
                Thread.Sleep(2000);
            }
        }
    }
}
