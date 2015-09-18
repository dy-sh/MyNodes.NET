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
        private static ComPort comPort=new ComPort();
        private static Gateway gateway=new Gateway();
        //private static SqlDapperRepository db;
        private static INodesRepository db=new SqlDapperRepository();
        private static SignalRController signalR=new SignalRController(gateway);


        private static void Main(string[] args)
        {
            signalR.OnLogMessageEvent += message => Console.Write(message);

            //connecting to DB
            bool connected = false;
            while (!connected)
            {
                connected = ConnectToDb();
                if (!connected) Thread.Sleep(5000);
            }

            //connecting to serial port
            connected = false;
            while (!connected)
            {
                string serialPort = ConfigurationManager.AppSettings["SerilPort"];
                connected = ConnectToPort(serialPort);
                if (!connected) Thread.Sleep(5000);
            }

            //connecting to gateway
            connected = false;
            while (!connected)
            {
                connected = ConnectToGateway();
                if (!connected) Thread.Sleep(5000);
            }

            //connecting to webserver
            connected = false;
            while (!connected)
            {
                string server = ConfigurationManager.AppSettings["WebServer"];
                connected = signalR.Connect(server);
                if (!connected) Thread.Sleep(5000);

            }

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

            return comPorts[portIndex];
        }

   


    }
}
