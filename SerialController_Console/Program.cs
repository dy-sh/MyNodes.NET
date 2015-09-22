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

using MyNetSensors.SerialGateway;


namespace MyNetSensors.SerialController_Console
{
    internal class Program
    {
        private static ComPort comPort = new ComPort();
        private static Gateway gateway = new Gateway();
        private static INodesRepository db = new SqlDapperRepository();
        private static SignalRController signalR = new SignalRController();

        private static string serialPortName;

        private static void Main(string[] args)
        {

            //connecting to DB
            bool connected = false;
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseDB"]))
            {
                Console.WriteLine("Connecting to database... ");

                db.SetStoreInterval(Convert.ToInt32(ConfigurationManager.AppSettings["WritingToDbInterwal"]));
                db.ShowDebugInConsole(Convert.ToBoolean(ConfigurationManager.AppSettings["ShowDBDebug"]));
                db.StoreTxRxMessages(Convert.ToBoolean(ConfigurationManager.AppSettings["StoreTxRxMessagesInDB"]));
                string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

                while (!connected)
                {
                    db.Connect(gateway, connectionString);
                    connected = db.IsConnected();
                    if (!connected) Thread.Sleep(5000);
                }
            }



            //connecting to serial port
            Console.WriteLine("Connecting to serial port...");

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShowSerialPortStateDebug"]))
                comPort.OnDebugPortStateMessage += message => Console.WriteLine("SERIAL: " + message);

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShowSerialTxRxDebug"]))
                comPort.OnDebugTxRxMessage += message => Console.WriteLine("SERIAL: " + message);



            serialPortName = ConfigurationManager.AppSettings["SerialPort"];

            connected = false;
            while (!connected)
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["SelectSerialPortOnStartup"]))
                    serialPortName = SelectPort();

                comPort.Connect(serialPortName);
                connected = comPort.IsConnected();
                if (!connected) Thread.Sleep(5000);
            }


            //connecting to gateway
            Console.WriteLine("Connecting to gateway... ");

            gateway.enableAutoAssignId = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableAutoAssignId"]);

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShowGatewayTxRxDebug"]))
                gateway.OnDebugTxRxMessage += message => Console.WriteLine("GATEWAY: " + message);

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShowGatewayStateDebug"]))
                gateway.OnDebugGatewayStateMessage += message => Console.WriteLine("GATEWAY: " + message);

            connected = false;
            while (!connected)
            {
                gateway.Connect(comPort);
                connected = gateway.IsConnected();
                if (!connected) Thread.Sleep(5000);
            }



            //connecting to webserver
            connected = false;
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
                    connected=signalR.Connect(gateway, webServerUrl, connectionPassword);
                    if (!connected) Thread.Sleep(5000);
                }

                bool authorized =false;
                while (!authorized)
                {
                    authorized = signalR.IsAuthorized();
                    if (!authorized) Thread.Sleep(5000);
                }
            }

            //reconnect if disconnected. THIS MUST BE AFTER connecting to webserver, to send signalR message before 
            gateway.OnDisconnectedEvent += OnDisconnectedEvent;

            Console.WriteLine("Startup complete");
            while (true)
                Console.ReadLine();
        }

        private static void OnDisconnectedEvent(object sender, EventArgs e)
        {
            //connecting to serial port
            bool connected = false;
            while (!connected)
            {
                comPort.Connect(serialPortName);
                connected = comPort.IsConnected();
                if (!connected) Thread.Sleep(5000);
            }

            //connecting to gateway
            connected = false;
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
