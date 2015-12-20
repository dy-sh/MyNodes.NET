/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using MyNetSensors.Gateway;
using MyNetSensors.SoftNodes;
using DebugMessageEventHandler = MyNetSensors.SoftNodes.DebugMessageEventHandler;
using OnReceivedMessageHandler = MyNetSensors.SoftNodes.OnReceivedMessageHandler;

namespace MyNetSensors.SerialController
{

    public class SoftNodesServer : ISoftNodesServer
    {
        private IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<SoftNodesHub>();

        public static SoftNodesServer softNodesServer;

        public event OnReceivedMessageHandler OnReceivedMessage;
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event DebugMessageEventHandler OnDebugTxRxMessage;
        public event DebugMessageEventHandler OnDebugStateMessage;

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

                if (OnDebugStateMessage!=null)
                    OnDebugStateMessage(string.Format("Server started at {0}", url));

                if (OnConnected != null)
                    OnConnected();
            }
            catch(Exception e)
            {
                if (OnDebugStateMessage != null)
                    OnDebugStateMessage(string.Format("Failed to start server: {0}",e.Message ));
            }

        }

        public void SendMessage(Message message)
        {

            if (OnDebugTxRxMessage != null)
                OnDebugTxRxMessage(string.Format("TX: {0}", message.ToString()));

            hub.Clients.All.ReceiveMessage(message);
        }


        public void ReceiveMessage(Message message)
        {
            if(OnReceivedMessage!=null)
                OnReceivedMessage(message);

            if (OnDebugTxRxMessage != null)
                OnDebugTxRxMessage(string.Format("RX: {0}", message.ToString()));
        }

    }
}

