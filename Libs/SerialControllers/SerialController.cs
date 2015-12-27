﻿/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyNetSensors.Gateways;
using MyNetSensors.GatewayRepository;
using MyNetSensors.LogicalNodes;
using MyNetSensors.NodesLinks;
using MyNetSensors.NodeTasks;
using MyNetSensors.SensorsHistoryRepository;
using DebugMessageEventHandler = MyNetSensors.Gateways.DebugMessageEventHandler;

namespace MyNetSensors.SerialControllers
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

        public static bool logicalNodesEnabled = true;
        public static int logicalNodesUpdateInterval = 10;
        public static bool logicalNodesDebugNodes = true;
        public static bool logicalNodesDebugEngine = true;

        //VARIABLES
        public static ComPort comPort = new ComPort();
        public static Gateway gateway = new Gateway();
        public static IGatewayRepository gatewayDb;
        public static ISensorsHistoryRepository historyDb;
        public static ISensorsTasksRepository sensorsTasksDb;
        public static SensorsTasksEngine sensorsTasksEngine;
        public static ISensorsLinksRepository sensorsLinksDb;
        public static SensorsLinksEngine sensorsLinksEngine;
 //       public static ISoftNodesServer softNodesServer;
//        public static SoftNodesController softNodesController;


        public static LogicalNodesEngine logicalNodesEngine;
        public static ILogicalNodesRepository logicalNodesRepository;


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
                //ConnectToSoftNodesController();
                ConnectToLogicalNodesEngine();

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
            //todo logicalNodesRepository = new logicalNodesRepositoryDapper(dataBaseConnectionString);

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

        //private static void ConnectToSoftNodesController()
        //{
        //    if (!softNodesEnabled) return;

        //    OnDebugStateMessage("SOFT NODES SERVER: Starting...");

        //    if (softNodesDebugState)
        //        softNodesServer.OnDebugStateMessage += message => OnDebugStateMessage("SOFT NODES SERVER: " + message);

        //    if (softNodesDebugTxRx)
        //        softNodesServer.OnDebugTxRxMessage += message => OnDebugTxRxMessage("SOFT NODES SERVER: " + message);

        //    softNodesController = new SoftNodesController(softNodesServer, gateway);
        //    softNodesController.StartServer($"http://*:{softNodesPort}/");
        //    OnDebugStateMessage("SOFT NODES SERVER: Started");

        //}


        private static void ConnectToLogicalNodesEngine()
        {
            //connecting tasks
            if (!logicalNodesEnabled) return;

            OnDebugStateMessage("LOGICAL NODES ENGINE: Starting... ");

            //todo logicalNodesEngine = new LogicalNodesEngine(gateway, logicalNodesRepository);
            logicalNodesEngine = new LogicalNodesEngine(gateway);
            logicalNodesEngine.SetUpdateInterval(logicalNodesUpdateInterval);

            if (logicalNodesDebugEngine)
                logicalNodesEngine.OnDebugEngineMessage += message => OnDebugStateMessage("LOGICAL NODES ENGINE: " + message);

            if (logicalNodesDebugNodes)
                logicalNodesEngine.OnDebugNodeMessage += message => OnDebugTxRxMessage("LOGICAL NODES ENGINE: " + message);

            logicalNodesEngine.CreateAndAddMySensorsNodes();

            LogicalNodeMathPlus nodeMathPlus = new LogicalNodeMathPlus();
            nodeMathPlus.Position=new Position {X=100,Y=331};
            //nodeMathPlus.Inputs[0].Value = "1";
            //nodeMathPlus.Inputs[1].Value = "0";
            logicalNodesEngine.AddNode(nodeMathPlus);

            LogicalNodeInvert logicalNodeInvert = new LogicalNodeInvert();
            logicalNodeInvert.Position=new Position {X=100,Y=251};
            logicalNodesEngine.AddNode(logicalNodeInvert);

            LogicalNodeConsole logicalNodeConsole = new LogicalNodeConsole();
            logicalNodesEngine.AddNode(logicalNodeConsole);

            //List<LogicalNodeMySensors> mySensorsesNodes
            //    = logicalNodesEngine.CreateAndAddMySensorsNodes();


            //logicalNodesEngine.AddLink(mySensorsesNodes[0].Outputs[0], logicalNodeInvert.Inputs[0]);
            //logicalNodesEngine.AddLink(logicalNodeInvert.Outputs[0], mySensorsesNodes[1].Inputs[0]);
            //logicalNodesEngine.AddLink(mySensorsesNodes[1].Outputs[0], logicalNodeConsole.Inputs[0]);


            //string json1 = logicalNodesEngine.SerializeNodes();
            //string json2 = logicalNodesEngine.SerializeLinks();
            //logicalNodesEngine.DeserializeNodes(json1);
            //logicalNodesEngine.DeserializeLinks(json2);


            OnDebugStateMessage("LOGICAL NODES ENGINE: Started");
        }


    }
}