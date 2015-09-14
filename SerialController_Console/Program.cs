using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using MyNetSensors.SerialGateway;


namespace MyNetSensors.SerialController_Console
{
    internal class Program
    {
        private static ComPort comPort = new ComPort();
        private static Gateway gateway = new Gateway(comPort);


        private static void Main(string[] args)
        {
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
                Console.Write("Connecting to server {0}... ", server);
                connected = ConnectSignalR(server);
            }

            connected = false;
            while (!connected)
            {
                Console.Write("Connecting to gateway... ");
                gateway.OnMessageRecievedEvent += OnMessageRecievedEvent;
                gateway.OnMessageSendEvent += OnMessageSendEvent;
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

        private static HubConnection hubConnection;
        private static IHubProxy hubProxy;

        private static bool ConnectSignalR(string serverUrl)
        {
            // Подключаемся к веб-приложению
            hubConnection = new HubConnection(serverUrl);
            hubProxy = hubConnection.CreateHubProxy("gatewayHub");

            try
            {
                hubConnection.Start().Wait();
                hubProxy.On("clearLog", ClearLog);
            }
            catch { }

            bool result = hubConnection.State == ConnectionState.Connected;

            if (result)
                Console.WriteLine("OK");
            else
                Console.WriteLine("FAILED");

            return result;
        }



        private static void OnMessageRecievedEvent(Message message)
        {
            hubProxy.Invoke("OnMessageRecievedEvent", message);
        }

        private static void OnMessageSendEvent(Message message)
        {
            hubProxy.Invoke("OnMessageSendEvent", message);
        }

        private static void ClearLog()
        {
            gateway.messagesLog.ClearLog();
            Console.WriteLine("Clear log... OK");
        }
    }
}
