/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

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
        public event Action OnConnected;
        public event Action OnDisconnected;


        string url;
        public SoftNodesServer()
        {
            softNodesServer = this;
        }

        public void StartServer(string url)
        {
            this.url = url;

            try
            {
                WebApp.Start<Startup>(url);

                Console.WriteLine(string.Format("Soft nodes server started at {0}", url));
                if (OnConnected != null)
                    OnConnected();
            }
            catch(Exception e)
            {
                throw e;
            }

        }

        public void SendMessage(Message message)
        {
            hub.Clients.All.ReceiveMessage(message);
        }


        public void OnReceivedMessage(Message message)
        {
            //Console.WriteLine(message.ToString());
            if(OnReceivedMessageEvent!=null)
                OnReceivedMessageEvent(message);
        }

    }
}

