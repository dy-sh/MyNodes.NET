using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using MyNetSensors.Gateway;


namespace MyNetSensors.SoftNodesSignalRServer
{
    public class SoftNodesHub:Hub
    {
        public void SendMessage(Message message)
        {
            Console.WriteLine(message.ToString());

            //temporary callback
           // Clients.Caller.SendMessage(message);
        }
    }
}
