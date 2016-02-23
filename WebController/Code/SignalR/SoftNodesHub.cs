/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using MyNetSensors.Nodes;
using System.Linq;

namespace MyNetSensors.WebController.Code
{
    public class SoftNodesHub : Hub
    {
        public async Task<int> SetValue(string value,string channel, string password)
        {
            NodesEngine engine = SystemController.nodesEngine;

            if (password == "")
                password = null;

            return await Task.Run(() =>
            {
                if (engine == null)
                    return 2;

                List<ConnectionRemoteReceiverNode> receivers = engine.GetNodes()
                    .OfType<ConnectionRemoteReceiverNode>()
                    .Where(x => x.GetChannel().ToString() == channel)
                    .ToList();

                if (!receivers.Any())
                {
                    engine.LogNodesError(
                        $"Received a value for Remote Receiver, but no receivers with channel [{channel}]");
                    return 2;
                }

                var ip = Context.Request.HttpContext.Connection.RemoteIpAddress;
                string address = ip?.ToString();

                bool received = false;

                foreach (var receiver in receivers)
                {
                    if (receiver.ReceiveValue(value, channel, password, address))
                        received = true;
                }

                return received ? 0 : 1;
            });
        }
    }

}