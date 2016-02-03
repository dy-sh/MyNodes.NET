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
using MyNetSensors.Gateways.MySensors.Serial;
using MyNetSensors.Nodes;
using MyNetSensors.Repositories.Dapper;
using MyNetSensors.Repositories.EF.SQLite;
using MyNetSensors.Users;

namespace MyNetSensors.WebController.Code
{
    public static class SystemController
    {
        //SETTINGS
        public static string serialGatewayPortName = "COM1";

        public static string ethernetGatewayIp = "192.168.88.20";
        public static int ethernetGatewayPort = 5003;

        public static bool gatewayAutoAssignId = true;
        public static bool gatewayMessagesLogEnabled = false;

        public static bool dataBaseEnabled = true;
        public static bool useInternalDb = true;
        public static string dataBaseConnectionString;
        public static int dataBaseWriteInterval = 5000;


        public static bool nodesEngineEnabled = true;
        public static int nodesEngineUpdateInterval = 10;


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

        public static Logs logs = new Logs();

        public static event Action OnStarted;
        public static event Action OnGatewayConnected;
        public static event Action OnGatewayDisconnected;

        public static bool serialGatewayEnabled;
        public static bool ethernetGatewayEnabled;


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
                serialGatewayEnabled = Boolean.Parse(configuration["Gateway:SerialGateway:Enable"]);
                serialGatewayPortName = configuration["Gateway:SerialGateway:SerialPort"];

                ethernetGatewayEnabled = Boolean.Parse(configuration["Gateway:EthernetGateway:Enable"]);
                ethernetGatewayIp = configuration["Gateway:EthernetGateway:GatewayIP"];
                ethernetGatewayPort = Int32.Parse(configuration["Gateway:EthernetGateway:GatewayPort"]);

                gatewayAutoAssignId = Boolean.Parse(configuration["Gateway:EnableAutoAssignId"]);
                gatewayMessagesLogEnabled = Boolean.Parse(configuration["Gateway:EnableMessagesLog"]);

                logs.enableGatewayLog = Boolean.Parse(configuration["Gateway:LogState"]);
                logs.enableHardwareNodesLog = Boolean.Parse(configuration["Gateway:LogMessages"]);
                logs.enableNodesEngineLog = Boolean.Parse(configuration["NodesEngine:LogEngine"]);
                logs.enableNodesLog = Boolean.Parse(configuration["NodesEngine:LogNodes"]);
                logs.enableDataBaseLog = Boolean.Parse(configuration["DataBase:LogState"]);

                nodesEngineEnabled = Boolean.Parse(configuration["NodesEngine:Enable"]);
                nodesEngineUpdateInterval = Int32.Parse(configuration["NodesEngine:UpdateInterval"]);

                dataBaseEnabled = Boolean.Parse(configuration["DataBase:Enable"]);
                useInternalDb = Boolean.Parse(configuration["DataBase:UseInternalDb"]);
                dataBaseWriteInterval = Int32.Parse(configuration["DataBase:WriteInterval"]);
                dataBaseConnectionString = configuration["DataBase:ExternalDbConnectionString"];

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
            if (!dataBaseEnabled) return;

            logs.AddSystemInfo("Connecting to database... ");


            //db config
            if (useInternalDb)
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
                if (String.IsNullOrEmpty(dataBaseConnectionString))
                {
                    logs.AddSystemError("Database connection failed. Set ConnectionString in appsettings.json file.");
                    return;
                }

                mySensorsDb = new MySensorsRepositoryDapper(dataBaseConnectionString);
                mySensorsMessagesDb = new MySensorsMessagesRepositoryDapper(dataBaseConnectionString);
                uiTimerNodesDb = new UITimerNodesRepositoryDapper(dataBaseConnectionString);
                nodesDb = new NodesRepositoryDapper(dataBaseConnectionString);
                nodesStatesDb = new NodesStatesRepositoryDapper(dataBaseConnectionString);
                usersRepository = new UsersRepositoryDapper(dataBaseConnectionString);
            }


            mySensorsDb.SetWriteInterval(dataBaseWriteInterval);
            mySensorsDb.OnLogInfo += logs.AddDataBaseInfo;
            mySensorsDb.OnLogError += logs.AddDataBaseError;

            mySensorsMessagesDb.SetWriteInterval(dataBaseWriteInterval);
            mySensorsMessagesDb.OnLogInfo += logs.AddDataBaseInfo;
            mySensorsMessagesDb.OnLogError += logs.AddDataBaseError;

            nodesDb.SetWriteInterval(dataBaseWriteInterval);
            nodesDb.OnLogInfo += logs.AddDataBaseInfo;
            nodesDb.OnLogError += logs.AddDataBaseError;

            logs.AddSystemInfo("Database connected.");
        }







        private static void StartNodesEngine()
        {
            nodesEngine = new NodesEngine(nodesDb);
            nodesEngine.SetUpdateInterval(nodesEngineUpdateInterval);
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

            if (!nodesEngineEnabled) return;

            logs.AddSystemInfo("Starting nodes engine... ");
            nodesEngine.Start();

            logs.AddSystemInfo("Nodes engine started.");
        }





        public static void ConnectToGateway()
        {
            mySensorsNodesEngine = null;

            if (serialGatewayEnabled)
            {
                gatewayConnectionPort = new SerialConnectionPort(serialGatewayPortName);
            }
            else if (ethernetGatewayEnabled)
            {
                gatewayConnectionPort = new EthernetConnectionPort(ethernetGatewayIp, ethernetGatewayPort);
            }
            else return;

            //connecting to gateway
            logs.AddSystemInfo("Connecting to gateway...");

            gateway = new Gateway(gatewayConnectionPort, mySensorsDb, mySensorsMessagesDb);

            gateway.enableAutoAssignId = gatewayAutoAssignId;

            gateway.OnLogMessage += logs.AddHardwareNodeInfo;
            gateway.OnLogInfo += logs.AddGatewayInfo;
            gateway.OnLogError += logs.AddGatewayError;
            gateway.connectionPort.OnLogInfo += logs.AddGatewayInfo;
            // gateway.connectionPort.OnLogMessage += logs.AddHardwareNodeInfo;
            gateway.endlessConnectionAttempts = true;
            gateway.messagesLogEnabled = gatewayMessagesLogEnabled;
            gateway.OnConnected += GatewayConnected;
            gateway.OnDisconnected += GatewayDisconnected;

            gateway.Connect().Wait();

            if (gateway != null && nodesEngine != null)
                mySensorsNodesEngine = new MySensorsNodesEngine(gateway, nodesEngine);

            if (gateway!= null && gateway.IsConnected())
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
            if (gateway==null)
                return;

            if (gateway.GetGatewayState() != GatewayState.Disconnected)
            gateway.Disconnect();

            gateway = null;
            mySensorsNodesEngine = null;
        }
    }
}
