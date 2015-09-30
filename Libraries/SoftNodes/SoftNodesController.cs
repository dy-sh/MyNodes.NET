/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MyNetSensors.Gateway;

namespace MyNetSensors.SoftNodes
{
    public class SoftNodesController
    {
        private ISoftNodesServer server;
        private SerialGateway gateway;

        public SoftNodesController(ISoftNodesServer server, SerialGateway gateway)
        {
            this.server = server;
            this.gateway = gateway;
            server.OnReceivedMessageEvent+= OnReceivedSoftNodeMessage;
            gateway.OnMessageSendEvent+= OnSendGatewayMessage;
        }

        private void OnSendGatewayMessage(Message message)
        {
            server.SendMessage(message);
        }

        private void OnReceivedSoftNodeMessage(Message message)
        {
            gateway.RecieveMessage(message);
        }

        public void StartServer(string url = "http://localhost:13122/")
        {
            server.StartServer(url);
        }
    }
}
