/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using Microsoft.AspNet.SignalR;
using MyNetSensors.Gateway;

namespace MyNetSensors.SerialController
{
    public class SoftNodesHub:Hub
    {
        public void ReceiveMessage(Message message)
        {
            SoftNodesServer.softNodesServer.ReceiveMessage(message);
        }
    }
}
