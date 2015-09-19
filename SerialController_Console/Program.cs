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
        private static SignalRController signalR= new SignalRController();

        //settings



        private static void Main(string[] args)
        {

            //connecting to DB
            bool connected = false;
            bool useDB = Convert.ToBoolean(ConfigurationManager.AppSettings["UseDB"]);
            if (useDB)
            {
                db.SetStoreInterval(Convert.ToInt32(ConfigurationManager.AppSettings["StoreNodesAndMessagesInDbInterval"]));
                db.ShowDebugInConsole(Convert.ToBoolean(ConfigurationManager.AppSettings["ShowDBDebugMessages"]));
                db.StoreTxRxMessages(Convert.ToBoolean(ConfigurationManager.AppSettings["StoreTxRxMessagesInDB"]));
                while (!connected)
                {
                    connected = ConnectToDb();
                    if (!connected) Thread.Sleep(5000);
                }
            }

            //connecting to webserver
            connected = false;
            bool connectToWebServer = Convert.ToBoolean(ConfigurationManager.AppSettings["ConnectToWebServer"]);
            if (connectToWebServer)
            {
                signalR.OnLogMessageEvent += message => Console.Write(message);

                while (!connected)
                {
                    string webServerUrl = ConfigurationManager.AppSettings["WebServerUrl"];
                    connected = signalR.Connect(gateway, webServerUrl);
                    if (!connected) Thread.Sleep(5000);
                }
            }

            //connecting to serial port
            connected = false;
            while (!connected)
            {
                bool selectSerialPortOnStartup = Convert.ToBoolean(ConfigurationManager.AppSettings["SelectSerialPortOnStartup"]);

                string serialPort;
                if (selectSerialPortOnStartup)
                    serialPort = SelectPort();
                else
                    serialPort = ConfigurationManager.AppSettings["SerialPort"];

                connected = ConnectToPort(serialPort);
                if (!connected) Thread.Sleep(5000);
            }

            //connecting to gateway
            connected = false;
            gateway.enableAutoAssignId = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableAutoAssignId"]);
            if(Convert.ToBoolean(ConfigurationManager.AppSettings["ShowTxRxMessagesMessages"]))
                gateway.OnLogMessageEvent += message => Console.Write(message);
            
            while (!connected)
            {
                connected = ConnectToGateway();
                if (!connected) Thread.Sleep(5000);
            }



            Console.WriteLine("Startup complete");
            while (true)
                Console.ReadLine();
        }

        private static bool ConnectToGateway()
        {
            Console.Write("Connecting to gateway... ");

            gateway.Connect(comPort);
            bool connected = gateway.IsConnected();

            if (connected)
                Console.WriteLine("OK");
            else
                Console.WriteLine("FAILED");

            return connected;
        }

        private static bool ConnectToDb()
        {
            Console.Write("Connecting to database... ");

            string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
            db.Connect(gateway, connectionString);

            bool connected = db.IsConnected();

            if (connected)
                Console.WriteLine("OK");
            else
                Console.WriteLine("FAILED");

            return connected;
        }

        private static bool ConnectToPort(string port)
        {
            Console.Write("Connecting to port {0}... ", port);

            try
            {
                comPort.Connect(port);
            }
            catch { }


            bool connected = comPort.IsConnected();

            if (connected)
                Console.WriteLine("OK");
            else
                Console.WriteLine("FAILED");

            return connected;
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

            string port=null;
            try {port = comPorts[portIndex];}
            catch { }

            return port;
        }




    }
}
