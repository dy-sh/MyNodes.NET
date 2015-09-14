using Microsoft.Owin;
using MyNetSensors.WebController;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace MyNetSensors.WebController
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
