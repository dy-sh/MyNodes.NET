using System;
using Microsoft.AspNet.SignalR.Client;
using MyNetSensors.SoftNodes;

namespace MyNetSensors.SoftNodesSignalRClient
{
    public class SoftNode:ISoftNode
    {
        IHubProxy hub;
        string url;

        public SoftNode(string url = "http://localhost:8080/")
        {
            this.url = url;

            var connection = new HubConnection(url);
            hub = connection.CreateHubProxy("SoftNodesHub");
            connection.Start().Wait();

            hub.On("ReceiveLength", x => Console.WriteLine(x));
        }

        public void Send(string message)
        {
            hub.Invoke("DetermineLength", message).Wait();
        }
    }
}
