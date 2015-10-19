using Microsoft.Owin;
using MyNetSensors.WebServer;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace MyNetSensors.WebServer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
