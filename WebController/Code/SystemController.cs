/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting.Internal;
using Microsoft.Extensions.Configuration;
using MyNetSensors.Gateways;
using MyNetSensors.Gateways.MySensors;
using MyNetSensors.Nodes;
using MyNetSensors.Repositories.Dapper;
using MyNetSensors.Repositories.EF.SQLite;
using MyNetSensors.Users;
using MyNetSensors.WebController.ViewModels.Config;
using Newtonsoft.Json;

namespace MyNetSensors.WebController.Code
{
    public static class SystemController
    {
        //CONFIG
        public static GatewayConfig gatewayConfig;
        public static WebServerRules webServerRules;
        public static DataBaseConfig dataBaseConfig;
        public static NodesEngineConfig nodesEngineConfig;


        //VARIABLES
        public static IUsersRepository usersRepository;

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

        public static Logs logs=new Logs();

        public static event Action OnStarted;
        public static event Action OnGatewayConnected;
        public static event Action OnGatewayDisconnected;




        private static bool systemControllerStarted;


        public static async void Start(IConfigurationRoot configuration, IServiceProvider services)
        {
            if (systemControllerStarted) return;
            systemControllerStarted = true;
            
            //logs config
            logs.OnGatewayLogInfo += (logMessage) => { Log(logMessage, ConsoleColor.Green); };
            logs.OnGatewayLogError += (logMessage) => { Log(logMessage, ConsoleColor.Red); };
            logs.OnHardwareNodeLogInfo += (logMessage) => { Log(logMessage, ConsoleColor.DarkGreen); };
            logs.OnHardwareNodeLogError += (logMessage) => { Log(logMessage, ConsoleColor.Red); };
            logs.OnDataBaseLogInfo += (logMessage) => { Log(logMessage, ConsoleColor.Gray); };
            logs.OnDataBaseLogError += (logMessage) => { Log(logMessage, ConsoleColor.Red); };
            logs.OnNodesEngineLogInfo += (logMessage) => { Log(logMessage, ConsoleColor.Cyan); };
            logs.OnNodesEngineLogError += (logMessage) => { Log(logMessage, ConsoleColor.Red); };
            logs.OnNodeLogInfo += (logMessage) => { Log(logMessage, ConsoleColor.DarkCyan); };
            logs.OnNodeLogError += (logMessage) => { Log(logMessage, ConsoleColor.Red); };
            logs.OnSystemLogInfo += (logMessage) => { Log(logMessage, ConsoleColor.White); };
            logs.OnSystemLogError += (logMessage) => { Log(logMessage, ConsoleColor.Red); };


            //bool firstRun = Boolean.Parse(Configuration["FirstRun"]);
            //if (firstRun)
            //else
            //{
            //    Console.ForegroundColor = ConsoleColor.White;
            //    Console.WriteLine("\nThis is the first run of the system. \nYou can configure MyNetSensors from the web interface.\n"); // <-- see note
            //    Console.ForegroundColor = ConsoleColor.Gray;
            //}


            //read settings
            ReadConfig(configuration);


            await Task.Run(() =>
            {
                //waiting for starting web server
                Thread.Sleep(500);

                logs.AddSystemInfo("---------------- STARTING ------------------");

                ConnectToDB(services);
                ConnectToGateway();
                StartNodesEngine();

                logs.AddSystemInfo("------------- SARTUP COMPLETE --------------");

                OnStarted?.Invoke();

            });
        }




        private static void ReadConfig(IConfigurationRoot configuration)
        {
            try
            {
                SerialGatewayConfig serialGatewayConfig = new SerialGatewayConfig
                {
                    Enable = Boolean.Parse(configuration["Gateway:SerialGateway:Enable"]),
                    SerialPortName = configuration["Gateway:SerialGateway:SerialPortName"]
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
                    EnableGatewayStateLog = Boolean.Parse(configuration["Logs:EnableGatewayStateLog"]),
                    EnableGatewayMessagesLog = Boolean.Parse(configuration["Logs:EnableGatewayMessagesLog"]),
                    EnableDataBaseStateLog = Boolean.Parse(configuration["Logs:EnableDataBaseStateLog"]),
                    EnableNodesEngineStateLog = Boolean.Parse(configuration["Logs:EnableNodesEngineStateLog"]),
                    EnableNodesEngineNodesLog = Boolean.Parse(configuration["Logs:EnableNodesEngineNodesLog"]),
                    EnableSystemStateLog = Boolean.Parse(configuration["Logs:EnableSystemStateLog"]),
                    MaxGatewayStateRecords = Int32.Parse(configuration["Logs:MaxGatewayStateRecords"]),
                    MaxGatewayMessagesRecords = Int32.Parse(configuration["Logs:MaxGatewayMessagesRecords"]),
                    MaxDataBaseStateRecords = Int32.Parse(configuration["Logs:MaxDataBaseStateRecords"]),
                    MaxNodesEngineStateRecords = Int32.Parse(configuration["Logs:MaxNodesEngineStateRecords"]),
                    MaxNodesEngineNodesRecords = Int32.Parse(configuration["Logs:MaxNodesEngineNodesRecords"]),
                    MaxSystemStateRecords = Int32.Parse(configuration["Logs:MaxSystemStateRecords"]),
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
                    AllowRegistrationOfNewUsers = Boolean.Parse(configuration["WebServer:Rules:AllowRegistrationOfNewUsers"])
                };

            }
            catch
            {
                logs.AddSystemError("ERROR: Bad configuration in appsettings.json file.");
                throw new Exception("Bad configuration in appsettings.json file.");
            }
        }




        public static void Log(LogRecord record, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(record.ToStringWithType());
            Console.ForegroundColor = ConsoleColor.Gray;
        }








        private static void ConnectToDB(IServiceProvider services)
        {
            if (!dataBaseConfig.Enable) return;

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
                usersRepository = new UsersRepositoryEf(usersDbContext);
            }
            else
            {
                if (String.IsNullOrEmpty(dataBaseConfig.ExternalDbConnectionString))
                {
                    logs.AddSystemError("Database connection failed. Set ConnectionString in appsettings.json file.");
                    return;
                }

                mySensorsDb = new MySensorsRepositoryDapper(dataBaseConfig.ExternalDbConnectionString);
                mySensorsMessagesDb = new MySensorsMessagesRepositoryDapper(dataBaseConfig.ExternalDbConnectionString);
                uiTimerNodesDb = new UITimerNodesRepositoryDapper(dataBaseConfig.ExternalDbConnectionString);
                nodesDb = new NodesRepositoryDapper(dataBaseConfig.ExternalDbConnectionString);
                nodesStatesDb = new NodesStatesRepositoryDapper(dataBaseConfig.ExternalDbConnectionString);
                usersRepository = new UsersRepositoryDapper(dataBaseConfig.ExternalDbConnectionString);
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







        private static void StartNodesEngine()
        {
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





        public static void ConnectToGateway()
        {
            mySensorsNodesEngine = null;

            if (gatewayConfig.SerialGatewayConfig.Enable)
            {
                gatewayConnectionPort = new SerialConnectionPort(
                    gatewayConfig.SerialGatewayConfig.SerialPortName);
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

            gateway.OnLogMessage += logs.AddHardwareNodeInfo;
            gateway.OnLogInfo += logs.AddGatewayInfo;
            gateway.OnLogError += logs.AddGatewayError;
            gateway.connectionPort.OnLogInfo += logs.AddGatewayInfo;
            // gateway.connectionPort.OnLogMessage += logs.AddHardwareNodeInfo;
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
    }
}
