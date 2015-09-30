using System;

using Microsoft.Owin.Hosting;
using MyNetSensors.Gateway;
using MyNetSensors.SoftNodes;

namespace MyNetSensors.SoftNodesSignalRServer
{

    public class SoftNodesController : ISoftNodesController
    {
        string url;
        public SoftNodesController(string url = "http://localhost:8080/")
        {
            this.url = url;

            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine(string.Format("Soft nodes server running at {0}", url));
                Console.ReadLine();
            }
        }

        public void SendMessage(Message message)
        {
            throw new NotImplementedException();
        }

        public void OnReceivedMessage(Message message)
        {
            throw new NotImplementedException();
        }

    }
}

