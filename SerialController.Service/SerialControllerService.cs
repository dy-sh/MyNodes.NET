using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using MyNetSensors.Gateway;
using MyNetSensors.GatewayRepository;
using MyNetSensors.NodesLinks;
using MyNetSensors.NodeTasks;
using MyNetSensors.SensorsHistoryRepository;
using MyNetSensors.SoftNodes;

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
            Log("Service started");
            SerialController.Start();
        }

        protected override void OnStop()
        {
            Log("Service stopped");
        }
        
        private static void Log(string message)
        {
            LogFile.WriteMessage(message);
        }
    }
}
