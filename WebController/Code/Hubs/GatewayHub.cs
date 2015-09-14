/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.AspNet.SignalR;
using MyNetSensors.SerialGateway;
using System.Threading.Tasks;

namespace MyNetSensors.WebController.Code.Hubs
{
    public class GatewayHub : Hub
    {


        public void OnMessageRecievedEvent(Message message)
        {
            Clients.All.onMessageRecievedEvent(message.ToString());
        }

        public void OnMessageSendEvent(Message message)
        {
            Clients.All.onMessageSendEvent(message.ToString());
        }

        public void OnClearMessages()
        {
            Clients.All.onClearMessages();
        }

        public void ReturnLog(List<Message> log)
        {
            string sLog="";
            foreach (var message in log)
            {
                sLog += message.ToString()+"<br/>";
            }
            Clients.All.returnLog(sLog);
        }

        public void ReturnNodes(List<Node> nodes)
        {
            Clients.All.returnNodes(nodes);
        }
    }

}