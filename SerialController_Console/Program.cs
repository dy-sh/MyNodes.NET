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
        private static Gateway gateway = new Gateway(comPort);
        private static SignalRController signalR = new SignalRController(gateway);


        private static void Main(string[] args)
        {
            signalR.OnLogMessageEvent += message => Console.Write(message);

            bool connected = false;
            while (!connected)
            {
                string serialPort = ConfigurationManager.AppSettings["SerilPort"];
                Console.Write("Connecting to port {0}... ", serialPort);
                connected = ConnectToPort(serialPort);
            }

            connected = false;
            while (!connected)
            {
                string server = ConfigurationManager.AppSettings["WebServer"];                
                connected = signalR.Connect(server);
            }

            connected = false;
            while (!connected)
            {
                Console.Write("Connecting to gateway... ");

                connected = gateway.IsConnected();
                Console.WriteLine("OK");
            }

            Console.ReadKey();
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

        private static bool ConnectToPort(string port)
        {
            try
            {
                comPort.Connect(port);
            }
            catch { }
            

            bool result = comPort.IsConnected();

            if (result)
                Console.WriteLine("OK");
            else
                Console.WriteLine("FAILED");

            return result;
        }

      
    }
}
