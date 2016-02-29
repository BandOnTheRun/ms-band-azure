using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(BandOnTheRunTrackerWebApp.Startup))]

namespace BandOnTheRunTrackerWebApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
