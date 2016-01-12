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
using MyNetSensors.LogicalNodesUI;
using MyNetSensors.NodesTasks;
using MyNetSensors.Repositories.Dapper;
using MyNetSensors.Repositories.EF.SQLite;

namespace MyNetSensors.SerialControllers
{
    public static class SerialController
    {
        //SETTINGS
        public static string serialPortName = "COM1";
        public static bool enableAutoAssignId = true;

        public static bool dataBaseEnabled = true;
        public static bool dataBadeUseMSSQL = true;
        public static string dataBaseConnectionString;
        public static int dataBaseWriteInterval = 5000;
        public static bool writeNodesMessagesToDataBase = false;

        public static bool nodesTasksEnabled = true;
        public static int nodesTasksUpdateInterval = 10;

        public static bool logicalNodesEnabled = true;
        public static int logicalNodesUpdateInterval = 10;

        //public static bool softNodesEnabled = true;
        //public static int softNodesPort = 13122;
        //public static bool softNodesLogMessages = true;
        //public static bool softNodesLogState = true;


        //VARIABLES
        public static ComPort comPort = new ComPort();
        public static Gateway gateway = new Gateway(comPort);

        public static IGatewayRepository gatewayDb;
        public static INodesHistoryRepository historyDb;
        public static INodesMessagesRepository messagesDb;

        public static NodesTasksEngine nodesTasksEngine;
        public static INodesTasksRepository nodesTasksDb;

        public static LogicalNodesEngine logicalNodesEngine;
        public static LogicalHardwareNodesEngine logicalHardwareNodesEngine;
        public static LogicalNodesUIEngine logicalNodesUIEngine;
        public static ILogicalNodesRepository logicalNodesDb;

        public static SerialControllerLogs logs = new SerialControllerLogs();

        //public static ISoftNodesServer softNodesServer;
        //public static SoftNodesController softNodesController;

        public static event EventHandler OnStarted;

        private static bool isStarted;

        public static async void Start(string serialPortName)
        {
            if (isStarted)
            {
                logs.AddGatewayError("Can`t start. Gateway already started.");
            }

            SerialController.serialPortName = serialPortName;
            isStarted = true;


            await Task.Run(() =>
            {
                //waiting for starting web server
                Thread.Sleep(500);

                logs.AddSerialControllerInfo("-------------STARTING CONTROLLER--------------");

                ConnectToDB();
                ConnectToGateway();
                ConnectNodesTasks();
                //ConnectToSoftNodesController();
                ConnectToLogicalNodesEngine();

                logs.AddSerialControllerInfo("-------------SARTUP COMPLETE--------------");

                OnStarted?.Invoke(null, EventArgs.Empty);

            });
        }




        private static void ConnectToDB()
        {

            //connecting to DB
            if (!dataBaseEnabled) return;

            logs.AddSerialControllerInfo("Connecting to database... ");


            if (dataBadeUseMSSQL)
            {
                if (dataBaseConnectionString == null)
                {
                    logs.AddSerialControllerError("Database connection failed. Set ConnectionString in appsettings.json file.");
                    return;
                }

                gatewayDb = new GatewayRepositoryDapper(dataBaseConnectionString);
                historyDb = new NodesHistoryRepositoryDapper(dataBaseConnectionString);
                messagesDb = new NodesMessagesRepositoryDapper(dataBaseConnectionString);
                nodesTasksDb = new NodesTasksRepositoryDapper(dataBaseConnectionString);
                logicalNodesDb = new LogicalNodesRepositoryDapper(dataBaseConnectionString);
            }
            else
            {
                //configure from SerialControllerConfigurator, 
                //because I don`t want to reference Entity Framework to SerialController
            }

            gatewayDb.SetWriteInterval(dataBaseWriteInterval);
            gatewayDb.ConnectToGateway(gateway);
            gatewayDb.OnLogInfo += logs.AddDataBaseInfo;
            gatewayDb.OnLogError += logs.AddDataBaseError;

            historyDb.SetWriteInterval(dataBaseWriteInterval);
            historyDb.ConnectToGateway(gateway);

            messagesDb.SetWriteInterval(dataBaseWriteInterval);
            messagesDb.ConnectToGateway(gateway);
            messagesDb.OnLogInfo += logs.AddDataBaseInfo;
            messagesDb.OnLogError += logs.AddDataBaseError;
            messagesDb.Enable(writeNodesMessagesToDataBase);

            logs.AddSerialControllerInfo("Database connected.");
        }


        private static void ConnectNodesTasks()
        {
            //connecting tasks
            if (!nodesTasksEnabled) return;

            logs.AddSerialControllerInfo("Starting Task Engine...");

            nodesTasksEngine = new NodesTasksEngine(gateway, nodesTasksDb);
            nodesTasksEngine.SetUpdateInterval(nodesTasksUpdateInterval);

            logs.AddSerialControllerInfo("Task Engine started.");
        }







        private static void ConnectToGateway()
        {
            //connecting to gateway
            logs.AddSerialControllerInfo("Connecting to gateway...");

            gateway.enableAutoAssignId = enableAutoAssignId;

            gateway.OnLogMessage += logs.AddNodeInfo;
            gateway.OnLogInfo += logs.AddGatewayInfo;
            gateway.OnLogError += logs.AddGatewayError;
            gateway.serialPort.OnLogInfo += logs.AddGatewayInfo;
           // gateway.serialPort.OnLogMessage += logs.AddNodeInfo;
            gateway.endlessConnectionAttempts = true;

            gateway.Connect(serialPortName).Wait();

            logs.AddSerialControllerInfo("Gateway connected.");

        }

        public static async void ReconnectToGateway(string serialPortName)
        {
            await Task.Run(() =>
            {
                gateway.Disconnect();

                SerialController.serialPortName = serialPortName;

                gateway.Connect(serialPortName).Wait();

            });
        }



        //private static void ConnectToSoftNodesController()
        //{
        //    if (!softNodesEnabled) return;

        //     OnLogStateMessage?.Invoke("SOFT NODES SERVER: Starting...");

        //    if (softNodesLogState)
        //        softNodesServer.OnLogStateMessage += message =>  OnDebugStateMessage?.Invoke("SOFT NODES SERVER: " + message);

        //    if (softNodesLogMessages)
        //        softNodesServer.OnLogMessage += message => OnLogMessage?.Invoke("SOFT NODES SERVER: " + message);

        //    softNodesController = new SoftNodesController(softNodesServer, gateway);
        //    softNodesController.StartServer($"http://*:{softNodesPort}/");
        //     OnLogStateMessage?.Invoke("SOFT NODES SERVER: Started");

        //}


        private static void ConnectToLogicalNodesEngine()
        {
            //connecting tasks
            if (!logicalNodesEnabled) return;

            logs.AddSerialControllerInfo("Starting logical nodes engine... ");


            logicalNodesEngine = new LogicalNodesEngine(logicalNodesDb);
            //logicalNodesEngine=new LogicalNodesEngine();

            logicalNodesEngine.SetUpdateInterval(logicalNodesUpdateInterval);

            logicalNodesEngine.OnLogEngineInfo += logs.AddLogicalNodesEngineInfo;
            logicalNodesEngine.OnLogEngineError += logs.AddLogicalNodesEngineError;
            logicalNodesEngine.OnLogNodeInfo += logs.AddLogicalNodeInfo;
            logicalNodesEngine.OnLogNodeError += logs.AddLogicalNodeError;


            logicalHardwareNodesEngine = new LogicalHardwareNodesEngine(gateway, logicalNodesEngine);
            logicalNodesUIEngine = new LogicalNodesUIEngine(logicalNodesEngine);

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


            logs.AddSerialControllerInfo("Logical nodes engine started.");
        }


    }
}
