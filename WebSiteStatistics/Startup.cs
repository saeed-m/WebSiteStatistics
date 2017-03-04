using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebSiteStatistics.Startup))]

namespace WebSiteStatistics
{
    public class Startup
    {
        public static void Configuration(IAppBuilder app)
        {
            
            app.MapSignalR();
        }
    }
}
