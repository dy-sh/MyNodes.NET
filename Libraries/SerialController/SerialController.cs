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
        private static ComPort comPort = new ComPort();
        private static SerialGateway gateway = new SerialGateway();
        private static IGatewayRepository gatewayDb;
        private static ISensorsHistoryRepository historyDb;
        private static ISensorsTasksRepository sensorsTasksDb;
        public static SensorsTasksEngine sensorsTasksEngine;
        private static ISensorsLinksRepository sensorsLinksDb;
        public static SensorsLinksEngine sensorsLinksEngine;
        private static ISoftNodesServer softNodesServer;
        private static SoftNodesController softNodesController;
        private static IGatewayServer gatewayWebServer;

        private static string serialPortName;

        public static event DebugMessageEventHandler OnDebugTxRxMessage;
        public static event DebugMessageEventHandler OnDebugStateMessage;

        
        public static void Start()
        {
            OnDebugStateMessage("-------------STARTING GATEWAY--------------");

            ConnectToGatewayDb();
            ConnectToHistoryDb();
            ConnectToSerialPort();
            ConnectSensorsTasks();
            ConnectSensorsLinks();
            ConnectToSoftNodesController();
            StartWebServer();

            //reconnect if disconnected
            gateway.OnDisconnectedEvent += OnDisconnectedEvent;

            OnDebugStateMessage("-------------SARTUP COMPLETE--------------");
            while (true)
                Console.ReadLine();
        }

        private static void StartWebServer()
        {
            OnDebugStateMessage("WEB SERVER: Starting...");

            bool connectToWebServer = Convert.ToBoolean(ConfigurationManager.AppSettings["UseWebServer"]);
            string webServerURL = ConfigurationManager.AppSettings["WebServerGatewayServiceURL"];
            //string connectionPassword = ConfigurationManager.AppSettings["WebServerGateConnectionPassword"];
            if (connectToWebServer)
            {

                gatewayWebServer = new GatewayServer();

                if (Convert.ToBoolean(ConfigurationManager.AppSettings["WebServerTxRxDebug"]))
                    gatewayWebServer.OnDebugTxRxMessage += message => OnDebugTxRxMessage("WEB SERVER: " + message);

                if (Convert.ToBoolean(ConfigurationManager.AppSettings["WebServerStateDebug"]))
                    gatewayWebServer.OnDebugStateMessage += message => OnDebugStateMessage("WEB SERVER: " + message);

                gatewayWebServer.StartServer(gateway, webServerURL);
            }
        }


        private static void OnDisconnectedEvent()
        {
            ConnectToSerialPort();
        }


        public async static Task ConnectToGatewayDb()
        {

            //connecting to DB
            bool connected = false;
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseGatewayDB"]))
            {
                OnDebugStateMessage("GATEWAY DB: Connecting... ");

                string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
                gatewayDb = new GatewayRepositoryDapper(connectionString);

                gatewayDb.SetWriteInterval(Convert.ToInt32(ConfigurationManager.AppSettings["GatewayDBWriteInterval"]));
                gatewayDb.ShowDebugInConsole(Convert.ToBoolean(ConfigurationManager.AppSettings["GatewayDBShowDebug"]));
                gatewayDb.SetStoreTxRxMessages(Convert.ToBoolean(ConfigurationManager.AppSettings["GatewayDBStoreTxRxMessages"]));


                while (!connected)
                {
                    gatewayDb.ConnectToGateway(gateway);
                    connected = (gatewayDb.IsDbExist());
                    if (!connected) await Task.Delay(5000);
                }

                OnDebugStateMessage("GATEWAY DB: Connected");
            }
        }

        public async static Task ConnectToHistoryDb()
        {
            //connecting to DB
            bool connected = false;
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseHistory"]))
            {
                OnDebugStateMessage("HISTORY DB: Connecting... ");

                string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
                historyDb = new SensorsHistoryRepositoryDapper(connectionString);
                historyDb.SetWriteInterval(Convert.ToInt32(ConfigurationManager.AppSettings["HistoryDBWriteInterval"]));

                while (!connected)
                {
                    historyDb.ConnectToGateway(gateway);
                    connected = (historyDb.IsDbExist());
                    if (!connected) await Task.Delay(5000);
                }

                OnDebugStateMessage("HISTORY DB: Connected");

            }
        }

        private async static Task ConnectSensorsTasks()
        {
            //connecting tasks
            bool connected = false;
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseSensorsTasks"]))
            {
                OnDebugStateMessage("TASK ENGINE: Starting...");

                string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

                while (!connected)
                {
                    sensorsTasksDb = new SensorsTasksRepositoryDapper(connectionString);
                    sensorsTasksEngine = new SensorsTasksEngine(gateway, sensorsTasksDb);
                    sensorsTasksEngine.SetUpdateInterval(Convert.ToInt32(ConfigurationManager.AppSettings["SensorsTasksUpdateInterval"]));
                    connected = (sensorsTasksDb.IsDbExist());
                    if (!connected) await Task.Delay(5000);
                }

                OnDebugStateMessage("TASK ENGINE: Started");

            }
        }

        private async static Task ConnectSensorsLinks()
        {
            //connecting tasks
            bool connected = false;
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseSensorsLinks"]))
            {
                OnDebugStateMessage("LINKS ENGINE: Starting... ");

                string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

                while (!connected)
                {
                    sensorsLinksDb = new SensorsLinksRepositoryDapper(connectionString);
                    sensorsLinksEngine = new SensorsLinksEngine(gateway, sensorsLinksDb);
                    connected = (sensorsTasksDb.IsDbExist());
                    if (!connected) await Task.Delay(5000);
                }

                sensorsLinksEngine.GetLinksFromRepository();

                OnDebugStateMessage("LINKS ENGINE: Started");

            }

        }

        public static async Task ConnectToSerialPort()
        {
            //connecting to serial port
            OnDebugStateMessage("SERIAL: Connecting...");

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["SerialStateDebug"]))
                comPort.OnDebugPortStateMessage += message => OnDebugStateMessage("SERIAL: " + message);

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["SerialTxRxDebug"]))
                comPort.OnDebugTxRxMessage += message => OnDebugTxRxMessage("SERIAL: " + message);



            serialPortName = ConfigurationManager.AppSettings["SerialPort"];

            bool connected = false;
            while (!connected)
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["SelectSerialPortOnStartup"]))
                    serialPortName = SelectPort();

                comPort.Connect(serialPortName);
                connected = comPort.IsConnected();
                if (!connected) await Task.Delay(5000);
            }

            ConnectToGateway();
        }


        public async static Task ConnectToGateway()
        {
            //connecting to gateway
            OnDebugStateMessage("GATEWAY: Connecting...");

            gateway.enableAutoAssignId = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableAutoAssignId"]);

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["GatewayTxRxDebug"]))
                gateway.OnDebugTxRxMessage += message => OnDebugTxRxMessage("GATEWAY: " + message);

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["GatewayStateDebug"]))
                gateway.OnDebugGatewayStateMessage += message => OnDebugStateMessage("GATEWAY: " + message);

            bool connected = false;
            while (!connected)
            {
                gateway.Connect(comPort);
                connected = gateway.IsConnected();
                if (!connected) Thread.Sleep(5000);
            }
        }

        private async static Task ConnectToSoftNodesController()
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseSoftNodes"]))
            {
                OnDebugStateMessage("SOFT NODES SERVER: Starting...");

                string softNodesServerURL = ConfigurationManager.AppSettings["SoftNodesServerURL"];
                softNodesServer = new SoftNodesServer();

                if (Convert.ToBoolean(ConfigurationManager.AppSettings["SoftNodesStateDebug"]))
                    softNodesServer.OnDebugStateMessage += message => OnDebugStateMessage("SOFT NODES SERVER: " + message);

                if (Convert.ToBoolean(ConfigurationManager.AppSettings["SoftNodesTxRxDebug"]))
                    softNodesServer.OnDebugTxRxMessage += message => OnDebugTxRxMessage("SOFT NODES SERVER: " + message);



                softNodesController = new SoftNodesController(softNodesServer, gateway);
                softNodesController.StartServer(softNodesServerURL);
            }
        }

        private static string SelectPort()
        {
            var comPorts = comPort.GetPortsList();

            Console.WriteLine("Select port:");

            for (int i = 0; i < comPorts.Count; i++)
            {
                Console.WriteLine(String.Format("{0}: {1}", i, comPorts[i]));
            }

            int portIndex = Int32.Parse(Console.ReadLine());

            string port = null;
            try { port = comPorts[portIndex]; }
            catch { }

            return port;
        }


    }
}
