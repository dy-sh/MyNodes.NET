/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyNetSensors.Gateway;
using MyNetSensors.GatewayRepository;
using MyNetSensors.SensorsHistoryRepository;


namespace MyNetSensors.SerialController_Console
{
    internal class Program
    {
        private static ComPort comPort = new ComPort();
        private static SerialGateway gateway = new SerialGateway();
        private static IGatewayRepository gatewayDb;
        private static ISensorsHistoryRepository historyDb;
        private static GatewaySignalRController signalR = new GatewaySignalRController();

        private static string serialPortName;

        private static void Main(string[] args)
        {
            ConnectToDb();
            ConnectToSerialPort();
            ConnectToWebServer();

            //reconnect if disconnected. THIS MUST BE AFTER connecting to webserver, to send signalR message before 
            gateway.OnDisconnectedEvent += OnDisconnectedEvent;

            Console.WriteLine("Startup complete");
            while (true)
                Console.ReadLine();
        }

        private static void OnDisconnectedEvent(object sender, EventArgs e)
        {
            ConnectToSerialPort();
        }


        public async static Task ConnectToDb()
        {
            //connecting to DB
            bool connected = false;
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseDB"]))
            {
                Console.WriteLine("Connecting to database... ");

                string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
                gatewayDb = new GatewayRepositoryDapper(connectionString);
                historyDb = new SensorsHistoryRepositoryDapper(connectionString);

                gatewayDb.SetWriteInterval(Convert.ToInt32(ConfigurationManager.AppSettings["WritingToDbInterwal"]));
                gatewayDb.ShowDebugInConsole(Convert.ToBoolean(ConfigurationManager.AppSettings["ShowDBDebug"]));
                gatewayDb.SetStoreTxRxMessages(Convert.ToBoolean(ConfigurationManager.AppSettings["StoreTxRxMessagesInDB"]));


                while (!connected)
                {
                    gatewayDb.ConnectToGateway(gateway);
                    historyDb.ConnectToGateway(gateway);
                    connected = (gatewayDb.IsDbExist() && historyDb.IsDbExist());
                    if (!connected) await Task.Delay(5000);
                }
            }
        }


        public static async Task ConnectToSerialPort()
        {
            //connecting to serial port
            Console.WriteLine("Connecting to serial port...");

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShowSerialPortStateDebug"]))
                comPort.OnDebugPortStateMessage += message => Console.WriteLine("SERIAL: " + message);

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShowSerialTxRxDebug"]))
                comPort.OnDebugTxRxMessage += message => Console.WriteLine("SERIAL: " + message);



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

        public static async Task ConnectToWebServer()
        {
            //connecting to webserver
            bool connected = false;
            bool connectToWebServer = Convert.ToBoolean(ConfigurationManager.AppSettings["ConnectToWebServer"]);
            string connectionPassword = ConfigurationManager.AppSettings["GateToWebConnectionPassword"];
            if (connectToWebServer)
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShowWebServerTxRxDebug"]))
                    signalR.OnDebugTxRxMessage += message => Console.WriteLine("WEB SERVER: " + message);

                if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShowWebServerStateDebug"]))
                    signalR.OnDebugStateMessage += message => Console.WriteLine("WEB SERVER: " + message);

                while (!connected)
                {
                    string webServerUrl = ConfigurationManager.AppSettings["WebServerUrl"];
                    connected = signalR.Connect(gateway, webServerUrl, connectionPassword);
                    if (!connected) Thread.Sleep(5000);
                }

                bool authorized = false;
                while (!authorized)
                {
                    authorized = signalR.IsAuthorized();
                    if (!authorized) Thread.Sleep(5000);
                }
            }
        }



        public async static Task ConnectToGateway()
        {
            //connecting to gateway
            Console.WriteLine("Connecting to gateway... ");

            gateway.enableAutoAssignId = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableAutoAssignId"]);

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShowGatewayTxRxDebug"]))
                gateway.OnDebugTxRxMessage += message => Console.WriteLine("GATEWAY: " + message);

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShowGatewayStateDebug"]))
                gateway.OnDebugGatewayStateMessage += message => Console.WriteLine("GATEWAY: " + message);

            bool connected = false;
            while (!connected)
            {
                gateway.Connect(comPort);
                connected = gateway.IsConnected();
                if (!connected) Thread.Sleep(5000);
            }


        }

        private static string SelectPort()
        {
            var comPorts = comPort.GetPortsList();

            Console.WriteLine("Select port:");

            for (int i = 0; i < comPorts.Count; i++)
            {
                Console.WriteLine("{0}: {1}", i, comPorts[i]);
            }

            int portIndex = Int32.Parse(Console.ReadLine());

            string port = null;
            try { port = comPorts[portIndex]; }
            catch { }

            return port;
        }




    }
}
