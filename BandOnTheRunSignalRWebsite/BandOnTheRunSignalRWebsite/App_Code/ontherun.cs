using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;


public class OnTheRun : Hub
{
    static List<Tuple<string, string>> data;

    public OnTheRun()
    {

        if (data == null)
        {
            data = new List<Tuple<string, string>>();
        }
    }
    public void Logon()
    {
        //Clients.Caller.loadHistory(data);
    }

    public void Send(string bandName, string rate)
    { 
       Clients.All.broadcastMessage(rate);
    }
}