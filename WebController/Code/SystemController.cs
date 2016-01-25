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

namespace MyNetSensors.WebController.Code
{
    public static class SystemController
    {
        //SETTINGS
        public static string serialPortName = "COM1";
        public static bool enableAutoAssignId = true;

        public static bool dataBaseEnabled = true;
        public static bool useInternalDb = true;
        public static string dataBaseConnectionString;
        public static int dataBaseWriteInterval = 5000;
        public static bool serialGatewayMessagesLogEnabled = false;


        public static bool nodesEngineEnabled = true;
        public static int nodesEngineUpdateInterval = 10;

        //public static bool softNodesEnabled = true;
        //public static int softNodesPort = 13122;
        //public static bool softNodesLogMessages = true;
        //public static bool softNodesLogState = true;


        //VARIABLES
        public static ComPort comPort;
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

        //public static ISoftNodesServer softNodesServer;
        //public static SoftNodesController softNodesController;

        public static event EventHandler OnStarted;

        public static bool serialGatewayEnabled;


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
                ConnectToSerialGateway();
                StartNodesEngine();

                logs.AddSystemInfo("------------- SARTUP COMPLETE --------------");

                OnStarted?.Invoke(null, EventArgs.Empty);

            });
        }



        private static void ReadConfig(IConfigurationRoot configuration)
        {
            try
            {
                serialGatewayEnabled = Boolean.Parse(configuration["SerialGateway:Enable"]);
                enableAutoAssignId = Boolean.Parse(configuration["SerialGateway:EnableAutoAssignId"]);
                serialGatewayMessagesLogEnabled = Boolean.Parse(configuration["SerialGateway:EnableMessagesLog"]);

                logs.enableGatewayLog = Boolean.Parse(configuration["SerialGateway:LogState"]);
                logs.enableHardwareNodesLog = Boolean.Parse(configuration["SerialGateway:LogMessages"]);
                logs.enableNodesEngineLog = Boolean.Parse(configuration["NodesEngine:LogEngine"]);
                logs.enableNodesLog = Boolean.Parse(configuration["NodesEngine:LogNodes"]);
                logs.enableDataBaseLog = Boolean.Parse(configuration["DataBase:LogState"]);

                nodesEngineEnabled = Boolean.Parse(configuration["NodesEngine:Enable"]);
                nodesEngineUpdateInterval = Int32.Parse(configuration["NodesEngine:UpdateInterval"]);

                dataBaseEnabled = Boolean.Parse(configuration["DataBase:Enable"]);
                useInternalDb = Boolean.Parse(configuration["DataBase:UseInternalDb"]);
                dataBaseWriteInterval = Int32.Parse(configuration["DataBase:WriteInterval"]);
                dataBaseConnectionString = configuration["DataBase:ExternalDbConnectionString"];

                serialPortName = configuration["SerialGateway:SerialPort"];
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

                nodesDb = new NodesRepositoryEf(nodesDbContext);
                nodesStatesDb = new NodesStatesRepositoryEf(nodesStatesHistoryDbContext);
                mySensorsDb = new MySensorsRepositoryEf(mySensorsNodesDbContext);
                mySensorsMessagesDb = new MySensorsMessagesRepositoryEf(mySensorsMessagesDbContext);
                uiTimerNodesDb = new UITimerNodesRepositoryEf(uiTimerNodesDbContext);
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
            }


            mySensorsDb.SetWriteInterval(dataBaseWriteInterval);
            mySensorsDb.OnLogInfo += logs.AddDataBaseInfo;
            mySensorsDb.OnLogError += logs.AddDataBaseError;

            mySensorsMessagesDb.SetWriteInterval(dataBaseWriteInterval);
            mySensorsMessagesDb.OnLogInfo += logs.AddDataBaseInfo;
            mySensorsMessagesDb.OnLogError += logs.AddDataBaseError;

            nodesDb.SetWriteInterval(dataBaseWriteInterval);
            nodesDb.OnLogInfo += logs.AddDataBaseInfo;
            nodesDb.OnLogError+= logs.AddDataBaseError;

            logs.AddSystemInfo("Database connected.");
        }


        


        private static void ConnectToSerialGateway()
        {
            comPort=new ComPort();
            gateway=new Gateway(comPort, mySensorsDb,mySensorsMessagesDb);

            gateway.enableAutoAssignId = enableAutoAssignId;

            gateway.OnLogMessage += logs.AddHardwareNodeInfo;
            gateway.OnLogInfo += logs.AddGatewayInfo;
            gateway.OnLogError += logs.AddGatewayError;
            gateway.serialPort.OnLogInfo += logs.AddGatewayInfo;
            // gateway.serialPort.OnLogMessage += logs.AddHardwareNodeInfo;
            gateway.endlessConnectionAttempts = true;
            gateway.messagesLogEnabled = serialGatewayMessagesLogEnabled;

            if (!serialGatewayEnabled) return;

                //connecting to gateway
                logs.AddSystemInfo("Connecting to gateway...");

                gateway.Connect(serialPortName).Wait();

                logs.AddSystemInfo("Gateway connected.");

        }




        private static void StartNodesEngine()
        {
            nodesEngine = new NodesEngine(nodesDb);
            nodesEngine.SetUpdateInterval(nodesEngineUpdateInterval);
            nodesEngine.OnLogEngineInfo += logs.AddNodesEngineInfo;
            nodesEngine.OnLogEngineError += logs.AddNodesEngineError;
            nodesEngine.OnLogNodeInfo += logs.AddNodeInfo;
            nodesEngine.OnLogNodeError += logs.AddNodeError;

            mySensorsNodesEngine = new MySensorsNodesEngine(gateway, nodesEngine);
            uiNodesEngine = new UiNodesEngine(nodesEngine, nodesStatesDb);
            uiTimerNodesEngine = new UITimerNodesEngine(nodesEngine, uiTimerNodesDb);

            if (!nodesEngineEnabled) return;

            logs.AddSystemInfo("Starting nodes engine... ");
            nodesEngine.Start();

            logs.AddSystemInfo("Nodes engine started.");
        }
    }
}
