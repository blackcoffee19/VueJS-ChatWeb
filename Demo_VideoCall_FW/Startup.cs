using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
[assembly: OwinStartup(typeof(Demo_VideoCall_FW.Startup))]
namespace Demo_VideoCall_FW
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Map SignalR
            app.MapSignalR();
        }
    }
}