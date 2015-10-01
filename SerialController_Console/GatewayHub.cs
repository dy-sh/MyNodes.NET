using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using MyNetSensors.Gateway;

namespace MyNetSensors.SerialController_Console
{
    public class GatewayHub : Hub
    {
        public void ClearLog(string userId)
        {
            GatewayServer.gatewayServer.ClearLog(userId);
        }

        public void ClearNodes(string userId)
        {
            GatewayServer.gatewayServer.ClearNodes(userId);
        }

        public void GetLog(string userId)
        {
            List<Message> log = GatewayServer.gatewayServer.GetLog(userId);
            Clients.Caller.ReturnLogEvent(userId, log);

        }

        public void GetNodes(string userId)
        {
            List<Node> nodes = GatewayServer.gatewayServer.GetNodes(userId);
            Clients.Caller.ReturnNodesEvent(userId, nodes);

        }

        public void GetGatewayInfo(string userId)
        {
            GatewayInfo info = GatewayServer.gatewayServer.GetGatewayInfo(userId);
            Clients.Caller.ReturnGatewayInfoEvent(userId, info);
        }

        public void GetGatewayHardwareConnected(string userId)
        {
            bool connected= GatewayServer.gatewayServer.GetGatewayHardwareConnected(userId);
            Clients.Caller.ReturnGatewayHardwareConnectedEvent(userId, connected);
        }

        public void UpdateNodeSettings(Node node)
        {
            GatewayServer.gatewayServer.UpdateNodeSettings(node);
        }

        public void SendMessage( string userId, string message)
        {
            GatewayServer.gatewayServer.SendMessage(userId, message);
        }

    }
}
