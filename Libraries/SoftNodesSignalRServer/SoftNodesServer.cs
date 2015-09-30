using System;
using System.Runtime.CompilerServices;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using MyNetSensors.Gateway;
using MyNetSensors.SoftNodes;

namespace MyNetSensors.SoftNodesSignalRServer
{

    public class SoftNodesServer : ISoftNodesServer
    {
        private IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<SoftNodesHub>();

        public static SoftNodesServer softNodesServer;

        public event OnReceivedMessageHandler OnReceivedMessageEvent;


        string url;
        public SoftNodesServer()
        {
            softNodesServer = this;
        }

        public void StartServer(string url)
        {
            this.url = url;

            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine(string.Format("Soft nodes server started at {0}", url));
                Console.ReadLine();
            }
        }

        public void SendMessage(Message message)
        {
            hub.Clients.All.ReceiveMessage(message);
        }


        public void OnReceivedMessage(Message message)
        {
            Console.WriteLine(message.ToString());
            if(OnReceivedMessageEvent!=null)
                OnReceivedMessageEvent(message);

            message.payload = "ok";
            SendMessage(message);
        }

    }
}

