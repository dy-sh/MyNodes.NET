/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using MyNetSensors.Gateway;
using MyNetSensors.GatewayRepository;
using MyNetSensors.NodesLinks;
using MyNetSensors.NodeTasks;
using MyNetSensors.SensorsHistoryRepository;
using MyNetSensors.SoftNodes;
using DebugMessageEventHandler = MyNetSensors.Gateway.DebugMessageEventHandler;

namespace MyNetSensors.SerialController
{
    static public class SerialController
    {

        //SETTINGS
        public static string serialPortName = "COM1";
        public static bool serialPortDebugTxRx = true;
        public static bool serialPortDebugState = true;
        public static bool enableAutoAssignId = true;
        public static bool gatewayDebugTxRx = true;
        public static bool gatewayDebugState = true;

        public static bool dataBaseEnabled = true;
        public static string dataBaseConnectionString;
        public static int dataBaseWriteInterval = 5000;
        public static bool dataBaseDebugState = true;
        public static bool dataBaseWriteTxRxMessages = true;

        public static bool sensorsTasksEnabled = true;
        public static int sensorsTasksUpdateInterval = 10;

        public static bool sensorsLinksEnabled = true;

        public static bool softNodesEnabled = true;
        public static int softNodesPort = 13122;
        public static bool softNodesDebugTxRx = true;
        public static bool softNodesDebugState = true;



        //VARIABLES
        private static ComPort comPort = new ComPort();
        public static SerialGateway gateway = new SerialGateway();
        private static IGatewayRepository gatewayDb;
        private static ISensorsHistoryRepository historyDb;
        private static ISensorsTasksRepository sensorsTasksDb;
        public static SensorsTasksEngine sensorsTasksEngine;
        private static ISensorsLinksRepository sensorsLinksDb;
        public static SensorsLinksEngine sensorsLinksEngine;
        private static ISoftNodesServer softNodesServer;
        private static SoftNodesController softNodesController;
        private static IGatewayServer gatewayWebServer;

        public static event DebugMessageEventHandler OnDebugTxRxMessage;
        public static event DebugMessageEventHandler OnDebugStateMessage;



        public static void Start(string serialPortName, string dbConnectionString = null)
        {
            SerialController.serialPortName = serialPortName;

            OnDebugStateMessage("-------------STARTING GATEWAY--------------");

            ConnectToDB();
            ConnectToSerialPort();
            ConnectSensorsTasks();
            ConnectSensorsLinks();
            ConnectToSoftNodesController();

            //reconnect if disconnected
            gateway.OnDisconnectedEvent += OnDisconnectedEvent;

            OnDebugStateMessage("-------------SARTUP COMPLETE--------------");

        }


        private static void OnDisconnectedEvent()
        {
            ConnectToSerialPort();
        }



        public async static Task ConnectToDB()
        {

            //connecting to DB
            bool connected = false;
            if (!dataBaseEnabled) return;

            OnDebugStateMessage("DATABASE: Connecting... ");

            gatewayDb = new GatewayRepositoryDapper(dataBaseConnectionString);
            historyDb = new SensorsHistoryRepositoryDapper(dataBaseConnectionString);
            sensorsTasksDb = new SensorsTasksRepositoryDapper(dataBaseConnectionString);
            sensorsLinksDb = new SensorsLinksRepositoryDapper(dataBaseConnectionString);

            gatewayDb.SetWriteInterval(dataBaseWriteInterval);
            gatewayDb.ShowDebugInConsole(dataBaseDebugState);
            gatewayDb.SetStoreTxRxMessages(dataBaseWriteTxRxMessages);
            gatewayDb.ConnectToGateway(gateway);

            historyDb.SetWriteInterval(dataBaseWriteInterval);
            historyDb.ConnectToGateway(gateway);

            OnDebugStateMessage("DATABASE: Connected");
        }


        private async static Task ConnectSensorsTasks()
        {
            //connecting tasks
            if (!sensorsTasksEnabled) return;

            OnDebugStateMessage("TASK ENGINE: Starting...");

            sensorsTasksEngine = new SensorsTasksEngine(gateway, sensorsTasksDb);
            sensorsTasksEngine.SetUpdateInterval(sensorsTasksUpdateInterval);

            OnDebugStateMessage("TASK ENGINE: Started");
        }

        private async static Task ConnectSensorsLinks()
        {
            //connecting tasks
            if (!sensorsLinksEnabled) return;

            OnDebugStateMessage("LINKS ENGINE: Starting... ");

            sensorsLinksEngine = new SensorsLinksEngine(gateway, sensorsLinksDb);
            sensorsLinksEngine.GetLinksFromRepository();

            OnDebugStateMessage("LINKS ENGINE: Started");
        }



        public static async Task ConnectToSerialPort()
        {
            //connecting to serial port
            OnDebugStateMessage("SERIAL: Connecting...");

            if (serialPortDebugTxRx)
                comPort.OnDebugPortStateMessage += message => OnDebugStateMessage("SERIAL: " + message);

            if (serialPortDebugState)
                comPort.OnDebugTxRxMessage += message => OnDebugTxRxMessage("SERIAL: " + message);

            bool connected = false;
            while (!connected)
            {
                comPort.Connect(serialPortName);
                connected = comPort.IsConnected();
                if (!connected) await Task.Delay(5000);
            }

            OnDebugStateMessage("SERIAL: Connected");
            ConnectToGateway();
        }



        public async static Task ConnectToGateway()
        {
            //connecting to gateway
            OnDebugStateMessage("GATEWAY: Connecting...");

            gateway.enableAutoAssignId = enableAutoAssignId;

            if (gatewayDebugTxRx)
                gateway.OnDebugTxRxMessage += message => OnDebugTxRxMessage("GATEWAY: " + message);

            if (gatewayDebugState)
                gateway.OnDebugGatewayStateMessage += message => OnDebugStateMessage("GATEWAY: " + message);

            bool connected = false;
            while (!connected)
            {
                gateway.Connect(comPort);
                connected = gateway.IsConnected();
                if (!connected) Thread.Sleep(5000);
            }
            OnDebugStateMessage("GATEWAY: Connected");
        }

        private async static Task ConnectToSoftNodesController()
        {
            if (!softNodesEnabled) return;

            OnDebugStateMessage("SOFT NODES SERVER: Starting...");

            if (softNodesDebugState)
                softNodesServer.OnDebugStateMessage += message => OnDebugStateMessage("SOFT NODES SERVER: " + message);

            if (softNodesDebugTxRx)
                softNodesServer.OnDebugTxRxMessage += message => OnDebugTxRxMessage("SOFT NODES SERVER: " + message);

            softNodesController = new SoftNodesController(softNodesServer, gateway);
            softNodesController.StartServer($"http://*:{softNodesPort}/");
            OnDebugStateMessage("SOFT NODES SERVER: Started");

        }



    }
}
