/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Threading;
using System.Threading.Tasks;
using MyNetSensors.Gateways;
using MyNetSensors.Gateways.MySensors.Serial;
using MyNetSensors.Nodes;
using MyNetSensors.Repositories.Dapper;

namespace MyNetSensors.WebController.Code
{
    public static class NodesController
    {
        //SETTINGS
        public static string serialPortName = "COM1";
        public static bool enableAutoAssignId = true;

        public static bool dataBaseEnabled = true;
        public static bool dataBadeUseMSSQL = true;
        public static string dataBaseConnectionString;
        public static int dataBaseWriteInterval = 5000;
        public static bool writeNodesMessagesToDataBase = false;


        public static bool nodesEngineEnabled = true;
        public static int nodesEngineUpdateInterval = 10;

        //public static bool softNodesEnabled = true;
        //public static int softNodesPort = 13122;
        //public static bool softNodesLogMessages = true;
        //public static bool softNodesLogState = true;


        //VARIABLES
        public static ComPort comPort = new ComPort();
        public static Gateway gateway = new Gateway(comPort);

        public static IMySensorsRepository mySensorsDb;
        public static IMySensorsMessagesRepository messagesDb;


        public static NodesEngine nodesEngine;
        public static MySensorsNodesEngine mySensorsNodesEngine;
        public static UiNodesEngine uiNodesEngine;
        public static INodesRepository nodesDb;
        public static INodesStatesRepository nodesStatesDb;
        public static UITimerNodesEngine uiTimerNodesEngine;
        public static IUITimerNodesRepository uiTimerNodesDb;

        public static NodesControllerLogs logs = new NodesControllerLogs();

        //public static ISoftNodesServer softNodesServer;
        //public static SoftNodesController softNodesController;

        public static event EventHandler OnStarted;

        private static bool isStarted;
        public static bool serialGatewayEnabled;

        public static async void Start(string serialPortName)
        {
            if (isStarted)
            {
                logs.AddGatewayError("Can`t start. Gateway already started.");
            }

            NodesController.serialPortName = serialPortName;
            isStarted = true;


            await Task.Run(() =>
            {
                //waiting for starting web server
                Thread.Sleep(500);

                logs.AddNodesControllerInfo("-------------STARTING CONTROLLER--------------");

                ConnectToDB();
                ConnectToGateway();
                //ConnectToSoftNodesController();
                ConnectToNodesEngine();

                logs.AddNodesControllerInfo("-------------SARTUP COMPLETE--------------");

                OnStarted?.Invoke(null, EventArgs.Empty);

            });
        }




        private static void ConnectToDB()
        {

            //connecting to DB
            if (!dataBaseEnabled) return;

            logs.AddNodesControllerInfo("Connecting to database... ");


            if (dataBadeUseMSSQL)
            {
                if (dataBaseConnectionString == null)
                {
                    logs.AddNodesControllerError("Database connection failed. Set ConnectionString in appsettings.json file.");
                    return;
                }

                mySensorsDb = new MySensorsRepositoryDapper(dataBaseConnectionString);
                messagesDb = new MySensorsMessagesRepositoryDapper(dataBaseConnectionString);
                uiTimerNodesDb = new UITimerNodesRepositoryDapper(dataBaseConnectionString);
                nodesDb = new NodesRepositoryDapper(dataBaseConnectionString);
                nodesStatesDb = new NodesStatesRepositoryDapper(dataBaseConnectionString);
            }
            else
            {
                //configure from NodesControllerConfigurator, 
                //because I don`t want to reference Entity Framework to NodesController
            }

            mySensorsDb.SetWriteInterval(dataBaseWriteInterval);
            mySensorsDb.ConnectToGateway(gateway);
            mySensorsDb.OnLogInfo += logs.AddDataBaseInfo;
            mySensorsDb.OnLogError += logs.AddDataBaseError;

            messagesDb.SetWriteInterval(dataBaseWriteInterval);
            messagesDb.ConnectToGateway(gateway);
            messagesDb.OnLogInfo += logs.AddDataBaseInfo;
            messagesDb.OnLogError += logs.AddDataBaseError;
            messagesDb.Enable(writeNodesMessagesToDataBase);

            logs.AddNodesControllerInfo("Database connected.");
        }









        private static void ConnectToGateway()
        {
            gateway.enableAutoAssignId = enableAutoAssignId;

            gateway.OnLogMessage += logs.AddHardwareNodeInfo;
            gateway.OnLogInfo += logs.AddGatewayInfo;
            gateway.OnLogError += logs.AddGatewayError;
            gateway.serialPort.OnLogInfo += logs.AddGatewayInfo;
            // gateway.serialPort.OnLogMessage += logs.AddHardwareNodeInfo;
            gateway.endlessConnectionAttempts = true;

            if (serialGatewayEnabled)
            {
                //connecting to gateway
                logs.AddNodesControllerInfo("Connecting to gateway...");

                gateway.Connect(serialPortName).Wait();

                logs.AddNodesControllerInfo("Gateway connected.");
            }

        }

        public static async void ReconnectToGateway(string serialPortName)
        {
            await Task.Run(() =>
            {
                gateway.Disconnect();

                NodesController.serialPortName = serialPortName;

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


        private static void ConnectToNodesEngine()
        {
            //connecting tasks
            if (!nodesEngineEnabled) return;

            logs.AddNodesControllerInfo("Starting nodes engine... ");


            nodesEngine = new NodesEngine(nodesDb);

            nodesEngine.SetUpdateInterval(nodesEngineUpdateInterval);

            nodesEngine.OnLogEngineInfo += logs.AddNodesEngineInfo;
            nodesEngine.OnLogEngineError += logs.AddNodesEngineError;
            nodesEngine.OnLogNodeInfo += logs.AddNodeInfo;
            nodesEngine.OnLogNodeError += logs.AddNodeError;


            mySensorsNodesEngine = new MySensorsNodesEngine(gateway, nodesEngine);
            uiNodesEngine = new UiNodesEngine(nodesEngine, nodesStatesDb);
            uiTimerNodesEngine = new UITimerNodesEngine(nodesEngine, uiTimerNodesDb);

            nodesEngine.Start();


            logs.AddNodesControllerInfo("Nodes engine started.");
        }


    }
}
