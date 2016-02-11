/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting.Internal;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using MyNetSensors.Gateways;
using MyNetSensors.Gateways.MySensors;
using MyNetSensors.Nodes;
using MyNetSensors.Repositories.Dapper;
using MyNetSensors.Repositories.EF.SQLite;
using MyNetSensors.Users;
using MyNetSensors.WebController.ViewModels.Config;
using Newtonsoft.Json;
using Node = MyNetSensors.Nodes.Node;
using System.Linq;

namespace MyNetSensors.WebController.Code
{
    public static class SystemController
    {
        //CONFIG
        public static GatewayConfig gatewayConfig;
        public static WebServerRules webServerRules;
        public static WebServerConfig webServerConfig;
        public static DataBaseConfig dataBaseConfig;
        public static NodesEngineConfig nodesEngineConfig;


        //VARIABLES
        public static IUsersRepository usersDb;

        public static IGatewayConnectionPort gatewayConnectionPort;
        public static Gateway gateway;

        public static IMySensorsRepository mySensorsDb;
        public static IMySensorsMessagesRepository mySensorsMessagesDb;

        public static NodesEngine nodesEngine;
        public static MySensorsNodesEngine mySensorsNodesEngine;
        public static UiNodesEngine uiNodesEngine;
        public static INodesRepository nodesDb;
        public static INodesStatesRepository nodesStatesDb;
        public static UITimerNodesEngine uiTimerNodesEngine;
        public static IUITimerNodesRepository uiTimerNodesDb;

        public static Logs logs = new Logs();

        public static event Action OnStarted;
        public static event Action OnGatewayConnected;
        public static event Action OnGatewayDisconnected;




        private static bool systemControllerStarted;
        private static bool firstRun;


        public static IServiceProvider services;
        public static IConfigurationRoot configuration;

        public static async void Start(IConfigurationRoot configuration, IServiceProvider services)
        {
            SystemController.configuration = configuration;
            SystemController.services = services;

            if (Boolean.Parse(configuration["FirstRun"]))
            {
                if (!firstRun)
                {
                    firstRun = true;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\nThis is the first run of the system. \nYou can configure MyNetSensors from the web interface.\n");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }

                return;
            }

            if (systemControllerStarted) return;
            systemControllerStarted = true;



            //read settings
            ReadConfig();


            await Task.Run(() =>
            {
                //waiting for starting web server
                Thread.Sleep(500);

                logs.AddSystemInfo("---------------- STARTING ------------------");

                if (nodesDb == null)
                    ConnectToDB();

                if (gateway == null)
                    ConnectToGateway();

                if (nodesEngine == null)
                    StartNodesEngine();

                logs.AddSystemInfo("------------- SARTUP COMPLETE --------------");

                OnStarted?.Invoke();

            });
        }




        public static void ReadConfig()
        {
            try
            {
                SerialGatewayConfig serialGatewayConfig = new SerialGatewayConfig
                {
                    Enable = Boolean.Parse(configuration["Gateway:SerialGateway:Enable"]),
                    SerialPortName = configuration["Gateway:SerialGateway:SerialPortName"],
                    Boudrate = Int32.Parse(configuration["Gateway:SerialGateway:Boudrate"])
                };

                EthernetGatewayConfig ethernetGatewayConfig = new EthernetGatewayConfig
                {
                    Enable = Boolean.Parse(configuration["Gateway:EthernetGateway:Enable"]),
                    GatewayIP = configuration["Gateway:EthernetGateway:GatewayIP"],
                    GatewayPort = Int32.Parse(configuration["Gateway:EthernetGateway:GatewayPort"])
                };

                gatewayConfig = new GatewayConfig
                {
                    SerialGatewayConfig = serialGatewayConfig,
                    EthernetGatewayConfig = ethernetGatewayConfig,
                    EnableAutoAssignId = Boolean.Parse(configuration["Gateway:EnableAutoAssignId"]),
                    EnableMessagesLog = Boolean.Parse(configuration["Gateway:EnableMessagesLog"])
                };

                logs.config = new LogsConfig
                {
                    ShowGatewayState = Boolean.Parse(configuration["Logs:ShowGatewayState"]),
                    ShowGatewayMessages = Boolean.Parse(configuration["Logs:ShowGatewayMessages"]),
                    ShowGatewayDecodedMessages = Boolean.Parse(configuration["Logs:ShowGatewayDecodedMessages"]),
                    ShowDataBaseState = Boolean.Parse(configuration["Logs:ShowDataBaseState"]),
                    ShowNodesEngineState = Boolean.Parse(configuration["Logs:ShowNodesEngineState"]),
                    ShowNodesEngineNodes = Boolean.Parse(configuration["Logs:ShowNodesEngineNodes"]),
                    ShowSystemState = Boolean.Parse(configuration["Logs:ShowSystemState"]),

                    StoreGatewayState = Boolean.Parse(configuration["Logs:StoreGatewayState"]),
                    StoreGatewayMessages = Boolean.Parse(configuration["Logs:StoreGatewayMessages"]),
                    StoreGatewayDecodedMessages = Boolean.Parse(configuration["Logs:StoreGatewayDecodedMessages"]),
                    StoreDataBaseState = Boolean.Parse(configuration["Logs:StoreDataBaseState"]),
                    StoreNodesEngineState = Boolean.Parse(configuration["Logs:StoreNodesEngineState"]),
                    StoreNodesEngineNodes = Boolean.Parse(configuration["Logs:StoreNodesEngineNodes"]),
                    StoreSystemState = Boolean.Parse(configuration["Logs:StoreSystemState"]),

                    MaxGatewayState = Int32.Parse(configuration["Logs:MaxGatewayState"]),
                    MaxGatewayMessages = Int32.Parse(configuration["Logs:MaxGatewayMessages"]),
                    MaxGatewayDecodedMessages = Int32.Parse(configuration["Logs:MaxGatewayDecodedMessages"]),
                    MaxDataBaseState = Int32.Parse(configuration["Logs:MaxDataBaseState"]),
                    MaxNodesEngineState = Int32.Parse(configuration["Logs:MaxNodesEngineState"]),
                    MaxNodesEngineNodes = Int32.Parse(configuration["Logs:MaxNodesEngineNodes"]),
                    MaxSystemState = Int32.Parse(configuration["Logs:MaxSystemState"]),
                };


                nodesEngineConfig = new NodesEngineConfig
                {
                    Enable = Boolean.Parse(configuration["NodesEngine:Enable"]),
                    UpdateInterval = Int32.Parse(configuration["NodesEngine:UpdateInterval"])
                };

                dataBaseConfig = new DataBaseConfig
                {
                    Enable = Boolean.Parse(configuration["DataBase:Enable"]),
                    UseInternalDb = Boolean.Parse(configuration["DataBase:UseInternalDb"]),
                    WriteInterval = Int32.Parse(configuration["DataBase:WriteInterval"]),
                    ExternalDbConnectionString = configuration["DataBase:ExternalDbConnectionString"]
                };

                webServerRules = new WebServerRules
                {
                    AllowFullAccessWithoutAuthorization = Boolean.Parse(configuration["WebServer:Rules:AllowFullAccessWithoutAuthorization"]),
                    AllowRegistrationOfNewUsers = Boolean.Parse(configuration["WebServer:Rules:AllowRegistrationOfNewUsers"])
                };

                webServerConfig = new WebServerConfig
                {
                    Address = configuration["WebServer:Address"]
                };

            }
            catch
            {
                logs.AddSystemError("ERROR: Bad configuration in appsettings.json file.");
                throw new Exception("Bad configuration in appsettings.json file.");
            }
        }













        public static void ConnectToDB()
        {
            if (!dataBaseConfig.Enable)
            {
                nodesDb = null;
                nodesStatesDb = null;
                mySensorsDb = null;
                mySensorsMessagesDb = null;
                uiTimerNodesDb = null;
                usersDb = null;
                return;
            }

            logs.AddSystemInfo("Connecting to database... ");


            //db config
            if (dataBaseConfig.UseInternalDb)
            {
                NodesDbContext nodesDbContext = (NodesDbContext)services.GetService(typeof(NodesDbContext));
                NodesStatesHistoryDbContext nodesStatesHistoryDbContext = (NodesStatesHistoryDbContext)services.GetService(typeof(NodesStatesHistoryDbContext));
                MySensorsNodesDbContext mySensorsNodesDbContext = (MySensorsNodesDbContext)services.GetService(typeof(MySensorsNodesDbContext));
                MySensorsMessagesDbContext mySensorsMessagesDbContext = (MySensorsMessagesDbContext)services.GetService(typeof(MySensorsMessagesDbContext));
                UITimerNodesDbContext uiTimerNodesDbContext = (UITimerNodesDbContext)services.GetService(typeof(UITimerNodesDbContext));
                UsersDbContext usersDbContext = (UsersDbContext)services.GetService(typeof(UsersDbContext));

                nodesDb = new NodesRepositoryEf(nodesDbContext);
                nodesStatesDb = new NodesStatesRepositoryEf(nodesStatesHistoryDbContext);
                mySensorsDb = new MySensorsRepositoryEf(mySensorsNodesDbContext);
                mySensorsMessagesDb = new MySensorsMessagesRepositoryEf(mySensorsMessagesDbContext);
                uiTimerNodesDb = new UITimerNodesRepositoryEf(uiTimerNodesDbContext);
                usersDb = new UsersRepositoryEf(usersDbContext);
            }
            else
            {
                if (String.IsNullOrEmpty(dataBaseConfig.ExternalDbConnectionString))
                {
                    logs.AddSystemError("Database connection failed. Set ConnectionString in appsettings.json file.");
                    return;
                }

                nodesDb = new NodesRepositoryDapper(dataBaseConfig.ExternalDbConnectionString);
                nodesStatesDb = new NodesStatesRepositoryDapper(dataBaseConfig.ExternalDbConnectionString);
                mySensorsDb = new MySensorsRepositoryDapper(dataBaseConfig.ExternalDbConnectionString);
                mySensorsMessagesDb = new MySensorsMessagesRepositoryDapper(dataBaseConfig.ExternalDbConnectionString);
                uiTimerNodesDb = new UITimerNodesRepositoryDapper(dataBaseConfig.ExternalDbConnectionString);
                usersDb = new UsersRepositoryDapper(dataBaseConfig.ExternalDbConnectionString);
            }


            mySensorsDb.SetWriteInterval(dataBaseConfig.WriteInterval);
            mySensorsDb.OnLogInfo += logs.AddDataBaseInfo;
            mySensorsDb.OnLogError += logs.AddDataBaseError;

            mySensorsMessagesDb.SetWriteInterval(dataBaseConfig.WriteInterval);
            mySensorsMessagesDb.OnLogInfo += logs.AddDataBaseInfo;
            mySensorsMessagesDb.OnLogError += logs.AddDataBaseError;

            nodesDb.SetWriteInterval(dataBaseConfig.WriteInterval);
            nodesDb.OnLogInfo += logs.AddDataBaseInfo;
            nodesDb.OnLogError += logs.AddDataBaseError;

            logs.AddSystemInfo("Database connected.");
        }







        public static void StartNodesEngine()
        {
            if (Boolean.Parse(configuration["Develop:GenerateNodesJsListFileOnStart"]))
                GenerateNodesJsListFile();

            nodesEngine = new NodesEngine(nodesDb);
            nodesEngine.SetUpdateInterval(nodesEngineConfig.UpdateInterval);
            nodesEngine.OnLogEngineInfo += logs.AddNodesEngineInfo;
            nodesEngine.OnLogEngineError += logs.AddNodesEngineError;
            nodesEngine.OnLogNodeInfo += logs.AddNodeInfo;
            nodesEngine.OnLogNodeError += logs.AddNodeError;

            if (gateway != null && nodesEngine != null)
                mySensorsNodesEngine = new MySensorsNodesEngine(gateway, nodesEngine);
            else
                mySensorsNodesEngine = null;

            uiNodesEngine = new UiNodesEngine(nodesEngine, nodesStatesDb);
            uiTimerNodesEngine = new UITimerNodesEngine(nodesEngine, uiTimerNodesDb);

            if (!nodesEngineConfig.Enable) return;

            logs.AddSystemInfo("Starting nodes engine... ");
            nodesEngine.Start();

            logs.AddSystemInfo("Nodes engine started.");
        }




        public static void GenerateNodesJsListFile()
        {
            try
            {
                List<Node> nodes = typeof (Node)
                    .Assembly.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof (Node)) && !t.IsAbstract)
                    .Select(t => (Node) Activator.CreateInstance(t)).ToList();

                nodes.AddRange(typeof (UiNode)
                    .Assembly.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof (UiNode)) && !t.IsAbstract)
                    .Select(t => (UiNode) Activator.CreateInstance(t)).ToList());

                nodes.Add((UiTimerNode) Activator.CreateInstance(typeof (UiTimerNode)));



                nodes = nodes.OrderBy(x => x.Type).ToList();

                string file = "(function () {\n";

                foreach (var node in nodes)
                    file += node.GetJsListGenerationScript();

                file += "\n})();";

                System.IO.File.WriteAllText("wwwroot/js/nodes-editor/nodes-editor-list.js", file);

                logs.AddSystemInfo($"Generated nodes editor script with {nodes.Count} nodes");
            }
            catch (Exception ex)
            {
                logs.AddSystemError($"Failed to generate nodes editor script. "+ex.Message);
            }

        }



        public static void ConnectToGateway()
        {
            mySensorsNodesEngine = null;

            if (gatewayConfig.SerialGatewayConfig.Enable)
            {
                gatewayConnectionPort = new SerialConnectionPort(
                    gatewayConfig.SerialGatewayConfig.SerialPortName,
                    gatewayConfig.SerialGatewayConfig.Boudrate);
            }
            else if (gatewayConfig.EthernetGatewayConfig.Enable)
            {
                gatewayConnectionPort = new EthernetConnectionPort(
                    gatewayConfig.EthernetGatewayConfig.GatewayIP,
                    gatewayConfig.EthernetGatewayConfig.GatewayPort);
            }
            else return;

            //connecting to gateway
            logs.AddSystemInfo("Connecting to gateway...");

            gateway = new Gateway(gatewayConnectionPort, mySensorsDb, mySensorsMessagesDb);

            gateway.enableAutoAssignId = gatewayConfig.EnableAutoAssignId;

            gateway.OnLogDecodedMessage += logs.AddGatewayDecodedMessage;
            gateway.OnLogMessage += logs.AddGatewayMessage;
            gateway.OnLogInfo += logs.AddGatewayInfo;
            gateway.OnLogError += logs.AddGatewayError;
            gateway.endlessConnectionAttempts = true;
            gateway.messagesLogEnabled = gatewayConfig.EnableMessagesLog;
            gateway.OnConnected += GatewayConnected;
            gateway.OnDisconnected += GatewayDisconnected;

            gateway.Connect().Wait();

            if (gateway != null && nodesEngine != null)
                mySensorsNodesEngine = new MySensorsNodesEngine(gateway, nodesEngine);

            if (gateway != null && gateway.IsConnected())
            {
                logs.AddSystemInfo("Gateway connected.");
            }
            else
                logs.AddSystemInfo("Gateway is not connected.");

        }

        private static void GatewayConnected()
        {
            OnGatewayConnected?.Invoke();
        }

        private static void GatewayDisconnected()
        {
            OnGatewayDisconnected?.Invoke();
        }


        public static void DisconnectGateway()
        {
            if (gateway == null)
                return;

            if (gateway.GetGatewayState() != GatewayState.Disconnected)
                gateway.Disconnect();

            gateway = null;
            mySensorsNodesEngine = null;
        }

        public static void ClearAllDatabases()
        {
            mySensorsDb.RemoveAllNodesAndSensors();
            mySensorsMessagesDb.RemoveAllMessages();
            nodesDb.RemoveAllLinks();
            nodesDb.RemoveAllNodes();
            nodesStatesDb.RemoveAllStates();
            uiTimerNodesDb.RemoveAllTasks();
            usersDb.RemoveAllUsers();
        }
    }
}
