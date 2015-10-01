using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;

namespace MyNetSensors.SerialController_Console
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
}
