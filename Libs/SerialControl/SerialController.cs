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
using MyNetSensors.LogicalNodes;
using MyNetSensors.NodesLinks;
using MyNetSensors.NodeTasks;
using MyNetSensors.SensorsHistoryRepository;
using MyNetSensors.SoftNodes;
using DebugMessageEventHandler = MyNetSensors.Gateway.DebugMessageEventHandler;

namespace MyNetSensors.SerialControl
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
        public static ComPort comPort = new ComPort();
        public static SerialGateway gateway = new SerialGateway();
        public static IGatewayRepository gatewayDb;
        public static ISensorsHistoryRepository historyDb;
        public static ISensorsTasksRepository sensorsTasksDb;
        public static SensorsTasksEngine sensorsTasksEngine;
        public static ISensorsLinksRepository sensorsLinksDb;
        public static SensorsLinksEngine sensorsLinksEngine;
        public static ISoftNodesServer softNodesServer;
        public static SoftNodesController softNodesController;
        public static IGatewayServer gatewayWebServer;

        public static NodesEditorEngine nodesEditorEngine;


        public static event DebugMessageEventHandler OnDebugTxRxMessage;
        public static event DebugMessageEventHandler OnDebugStateMessage;



        public static async void Start(string serialPortName, string dbConnectionString = null)
        {
            SerialController.serialPortName = serialPortName;

            await Task.Run(() =>
            {
                OnDebugStateMessage("-------------STARTING GATEWAY--------------");


                ConnectToDB();
                ConnectToSerialPort();
                ConnectSensorsTasks();
                ConnectSensorsLinks();
                ConnectToSoftNodesController();

                //reconnect if disconnected
                gateway.OnDisconnectedEvent += ReconnectToSerialPort;

                OnDebugStateMessage("-------------SARTUP COMPLETE--------------");
            });
        }



        public static void ConnectToDB()
        {

            //connecting to DB
            if (!dataBaseEnabled) return;

            OnDebugStateMessage("DATABASE: Connecting... ");

            if (dataBaseConnectionString == null)
            {
                OnDebugStateMessage("DATABASE: Connection failed. Set ConnectionString in appsettings.json file.");
                return;
            }


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


        private static void ConnectSensorsTasks()
        {
            //connecting tasks
            if (!sensorsTasksEnabled) return;

            OnDebugStateMessage("TASK ENGINE: Starting...");

            sensorsTasksEngine = new SensorsTasksEngine(gateway, sensorsTasksDb);
            sensorsTasksEngine.SetUpdateInterval(sensorsTasksUpdateInterval);

            OnDebugStateMessage("TASK ENGINE: Started");
        }

        private static void ConnectSensorsLinks()
        {
            //connecting tasks
            if (!sensorsLinksEnabled) return;

            OnDebugStateMessage("LINKS ENGINE: Starting... ");

            sensorsLinksEngine = new SensorsLinksEngine(gateway, sensorsLinksDb);
            sensorsLinksEngine.GetLinksFromRepository();

            OnDebugStateMessage("LINKS ENGINE: Started");
        }



        public static void ConnectToSerialPort()
        {
            if (serialPortDebugState)
                comPort.OnDebugPortStateMessage += message => OnDebugStateMessage("SERIAL: " + message);

            if (serialPortDebugTxRx)
                comPort.OnDebugTxRxMessage += message => OnDebugTxRxMessage("SERIAL: " + message);

            ReconnectToSerialPort();
        }

        public static void ReconnectToSerialPort()
        {
            //connecting to serial port
            OnDebugStateMessage("SERIAL: Connecting...");

            bool connected = false;
            while (!connected)
            {
                comPort.Connect(serialPortName);
                connected = comPort.IsConnected();
                if (!connected)
                {
                    Thread.Sleep(5000);
                }
            }

            ConnectToGateway();
        }

        public static void ConnectToGateway()
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
                if (!connected)
                {
                    Thread.Sleep(5000);
                }
            }
        }

        private static void ConnectToSoftNodesController()
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
