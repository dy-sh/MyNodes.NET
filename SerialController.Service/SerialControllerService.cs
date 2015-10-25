using System.ServiceProcess;
using System.Timers;

namespace MyNetSensors.SerialController.Service
{
    public partial class SerialControllerService : ServiceBase
    {

        public SerialControllerService()
        {
            InitializeComponent();

        }

        protected override void OnStart(string[] args)
        {
            LogFile.WriteMessage("Service started");
        }

        protected override void OnStop()
        {
            LogFile.WriteMessage("Service stopped");
        }
    }
}
