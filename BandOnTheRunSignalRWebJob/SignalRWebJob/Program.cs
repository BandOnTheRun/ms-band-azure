using System;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNet.SignalR.Client;
using System.Threading.Tasks;

namespace SignalRWebJob
{
    internal class Program
    {       
        private static void Main(string[] args)
        {
            //Set connection
            var connection = new HubConnection("http://localhost:55815/");
            //Make proxy to hub based on hub name on server
            var myHub = connection.CreateHubProxy("OnTheRun");
            //Start connection

            connection.Start().ContinueWith(task => {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error opening the connection:{0}",
                                      task.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine("Connected");
                }

            }).Wait();

            int i = 0;
            while (i < 10000)
            {
                Random random = new Random();
                var r = random.Next(1, 200);

                myHub.Invoke<string>("Send", "Band1038478478", r.ToString()).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Console.WriteLine("There was an error calling send: {0}",
                                          task.Exception.GetBaseException());
                    }
                    else
                    {
                        Console.WriteLine("Message Sent");
                    }
                }).Wait();
                Task.Delay(4000).Wait();
                i++;
            }
             

            Console.Read();
            connection.Stop();
        }
    }
}