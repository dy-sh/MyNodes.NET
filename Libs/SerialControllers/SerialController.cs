/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MyNetSensors.Gateways;
using MyNetSensors.LogicalNodes;
using MyNetSensors.LogicalNodesMySensors;
using MyNetSensors.NodesTasks;
using MyNetSensors.Repositories.Dapper;
using MyNetSensors.Repositories.EF.SQLite;
using DebugMessageEventHandler = MyNetSensors.Gateways.DebugMessageEventHandler;

namespace MyNetSensors.SerialControllers
{
    public static class SerialController
    {

        //SETTINGS
        public static string serialPortName = "COM1";
        public static bool enableAutoAssignId = true;
        public static bool gatewayDebugTxRx = true;
        public static bool gatewayDebugRawTxRx = false;
        public static bool gatewayDebugState = true;

        public static bool dataBaseEnabled = true;
        public static bool dataBadeUseMSSQL = true;
        public static string dataBaseConnectionString;
        public static int dataBaseWriteInterval = 5000;
        public static bool dataBaseDebugState = true;
        public static bool dataBaseWriteTxRxMessages = true;

        public static bool nodesTasksEnabled = true;
        public static int nodesTasksUpdateInterval = 10;


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
        public static Gateway gateway = new Gateway(comPort);
        public static IGatewayRepository gatewayDb;
        public static INodesHistoryRepository historyDb;
        public static INodesTasksRepository nodesTasksDb;
        public static NodesTasksEngine nodesTasksEngine;
        //       public static ISoftNodesServer softNodesServer;
        //        public static SoftNodesController softNodesController;



        public static LogicalNodesEngine logicalNodesEngine;
        public static LogicalHardwareNodesEngine logicalHardwareNodesEngine;
        public static ILogicalNodesRepository logicalNodesRepository;


        public static event DebugMessageEventHandler OnDebugTxRxMessage;
        public static event DebugMessageEventHandler OnDebugStateMessage;

        public static event EventHandler OnStarted;

        private static bool isStarted;

        public static async void Start(string serialPortName)
        {
            if (isStarted)
            {
                OnDebugStateMessage?.Invoke("Can`t start. Gateway already started.");
            }

            SerialController.serialPortName = serialPortName;
            isStarted = true;


            await Task.Run(() =>
            {
                //waiting for starting web server
                Thread.Sleep(500);

                OnDebugStateMessage?.Invoke("-------------STARTING GATEWAY--------------");

                ConnectToDB();
                ConnectToGateway();
                ConnectNodesTasks();
                //ConnectToSoftNodesController();
                ConnectToLogicalNodesEngine();

                //reconnect if disconnected
                gateway.OnDisconnectedEvent += ReconnectToGateway;

                OnDebugStateMessage?.Invoke("-------------SARTUP COMPLETE--------------");

                OnStarted?.Invoke(null, EventArgs.Empty);

            });
        }




        private static void ConnectToDB()
        {

            //connecting to DB
            if (!dataBaseEnabled) return;

            OnDebugStateMessage?.Invoke("DATABASE: Connecting... ");


            if (dataBadeUseMSSQL)
            {
                if (dataBaseConnectionString == null)
                {
                     OnDebugStateMessage?.Invoke("DATABASE: Connection failed. Set ConnectionString in appsettings.json file.");
                    return;
                }

                gatewayDb = new GatewayRepositoryDapper(dataBaseConnectionString);
                historyDb = new NodesHistoryRepositoryDapper(dataBaseConnectionString);
                nodesTasksDb = new NodesTasksRepositoryDapper(dataBaseConnectionString);
                logicalNodesRepository = new LogicalNodesRepositoryDapper(dataBaseConnectionString);
            }
            else
            {
                //configure from SerialControllerConfigurator, 
                //because I don`t want to reference Entity Framework to SerialController
            }

            gatewayDb.SetWriteInterval(dataBaseWriteInterval);
            gatewayDb.SetStoreTxRxMessages(dataBaseWriteTxRxMessages);
            gatewayDb.ConnectToGateway(gateway);
            if (dataBaseDebugState)
                gatewayDb.OnDebugStateMessage += message =>  OnDebugStateMessage?.Invoke("DB: " + message);

            historyDb.SetWriteInterval(dataBaseWriteInterval);
            historyDb.ConnectToGateway(gateway);




             OnDebugStateMessage?.Invoke("DATABASE: Connected");
        }


        private static void ConnectNodesTasks()
        {
            //connecting tasks
            if (!nodesTasksEnabled) return;

             OnDebugStateMessage?.Invoke("TASKS ENGINE: Starting...");

            nodesTasksEngine = new NodesTasksEngine(gateway, nodesTasksDb);
            nodesTasksEngine.SetUpdateInterval(nodesTasksUpdateInterval);

             OnDebugStateMessage?.Invoke("TASKS ENGINE: Started");
        }







        private static void ConnectToGateway()
        {
            //connecting to gateway
             OnDebugStateMessage?.Invoke("GATEWAY: Connecting...");

            gateway.enableAutoAssignId = enableAutoAssignId;

            if (gatewayDebugTxRx)
                gateway.OnDebugTxRxMessage += message => OnDebugTxRxMessage?.Invoke("GATEWAY: " + message);

            if (gatewayDebugState)
            {
                gateway.OnDebugStateMessage += message =>  OnDebugStateMessage?.Invoke("GATEWAY: " + message);
                gateway.serialPort.OnDebugStateMessage += message =>  OnDebugStateMessage?.Invoke("GATEWAY: " + message);
            }

            if (gatewayDebugRawTxRx)
                gateway.serialPort.OnDebugTxRxMessage += message => OnDebugTxRxMessage?.Invoke("GATEWAY: RAW MESSAGE: " + message);


            bool connected = false;
            while (!connected)
            {
                gateway.Connect(serialPortName);
                connected = gateway.IsConnected();
                if (!connected)
                {
                    Thread.Sleep(5000);
                }
            }
        }

        public static void ReconnectToGateway(string serialPortName)
        {
            gateway.Disconnect();

            SerialController.serialPortName = serialPortName;

            ReconnectToGateway();
        }

        private static async void ReconnectToGateway()
        {
            await Task.Run(() =>
            {
                bool connected = false;
                while (!connected)
                {
                    gateway.Connect(serialPortName);
                    connected = gateway.IsConnected();
                    if (!connected)
                    {
                        Thread.Sleep(5000);
                    }
                }
            });
        }

        //private static void ConnectToSoftNodesController()
        //{
        //    if (!softNodesEnabled) return;

        //     OnDebugStateMessage?.Invoke("SOFT NODES SERVER: Starting...");

        //    if (softNodesDebugState)
        //        softNodesServer.OnDebugStateMessage += message =>  OnDebugStateMessage?.Invoke("SOFT NODES SERVER: " + message);

        //    if (softNodesDebugTxRx)
        //        softNodesServer.OnDebugTxRxMessage += message => OnDebugTxRxMessage?.Invoke("SOFT NODES SERVER: " + message);

        //    softNodesController = new SoftNodesController(softNodesServer, gateway);
        //    softNodesController.StartServer($"http://*:{softNodesPort}/");
        //     OnDebugStateMessage?.Invoke("SOFT NODES SERVER: Started");

        //}


        private static void ConnectToLogicalNodesEngine()
        {
            //connecting tasks
            if (!logicalNodesEnabled) return;

             OnDebugStateMessage?.Invoke("LOGICAL NODES ENGINE: Starting... ");


            logicalNodesEngine = new LogicalNodesEngine(logicalNodesRepository);
            //logicalNodesEngine=new LogicalNodesEngine();

            logicalNodesEngine.SetUpdateInterval(logicalNodesUpdateInterval);

            if (logicalNodesDebugEngine)
                logicalNodesEngine.OnDebugEngineMessage += message =>  OnDebugStateMessage?.Invoke("LOGICAL NODES ENGINE: " + message);

            if (logicalNodesDebugNodes)
                logicalNodesEngine.OnDebugNodeMessage += message => OnDebugTxRxMessage?.Invoke("LOGICAL NODES ENGINE: " + message);


            logicalHardwareNodesEngine = new LogicalHardwareNodesEngine(gateway, logicalNodesEngine);

            logicalNodesEngine.Start();

            //demo
            //LogicalNodeMathPlus nodeMathPlus = new LogicalNodeMathPlus();
            //logicalNodesEngine.AddNode(nodeMathPlus);
            //LogicalNodeConsole logicalNodeConsole = new LogicalNodeConsole();
            //logicalNodesEngine.AddNode(logicalNodeConsole);
            //logicalNodesEngine.AddLink(nodeMathPlus[0].Outputs[0], logicalNodeConsole.Inputs[0]);



            //string json1 = logicalNodesEngine.SerializeNodes();
            //string json2 = logicalNodesEngine.SerializeLinks();
            //logicalNodesEngine.DeserializeNodes(json1);
            //logicalNodesEngine.DeserializeLinks(json2);


             OnDebugStateMessage?.Invoke("LOGICAL NODES ENGINE: Started");
        }


    }
}
