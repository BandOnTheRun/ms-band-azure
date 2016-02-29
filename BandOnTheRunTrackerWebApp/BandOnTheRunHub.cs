using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;

namespace BandOnTheRunTrackerWebApp
{
    public class BandOnTheRunHub : Hub
    {
        static List<Tuple<string, string>> data = new List<Tuple<string, string>>();

        public void Heartrate(string bandName, string rate)
        {
            Clients.All.broadcastMessage(rate);
        }
    }
}