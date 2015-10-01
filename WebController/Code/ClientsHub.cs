/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using MyNetSensors.Gateway;

namespace MyNetSensors.WebController.Code
{
    public class ClientsHub : Hub
    {
        public ClientsHub()
        {
            GatewayClientStatic.ConnectToServer();
        }

        private bool IsGatewayServiceConnected()
        {
            return GatewayClientStatic.gatewayClient.IsConnected();
        }
        


        public override Task OnConnected()
        {
            string clientId = Context.ConnectionId;

            if (!GatewayClientStatic.connectedUsersId.Contains(clientId))
                GatewayClientStatic.connectedUsersId.Add(clientId);

            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            string clientId = Context.ConnectionId;
            if (!GatewayClientStatic.connectedUsersId.Contains(clientId))
                GatewayClientStatic.connectedUsersId.Add(clientId);

            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string clientId = Context.ConnectionId;

            if (GatewayClientStatic.connectedUsersId.Contains(clientId))
                GatewayClientStatic.connectedUsersId.Remove(clientId);

            return base.OnDisconnected(stopCalled);
        }







        #region Messages from clients to hub

        public void GetConnectedUsersCount()
        {
           Clients.Caller.returnConnectedUsersCount(GatewayClientStatic.connectedUsersId.Count);
        }

        public void GetGatewayServiceConnected()
        {
            Clients.Caller.returnGatewayServiceConnected(IsGatewayServiceConnected());
        }

        #endregion



        #region Messages from clients to gateway

        public void SendMessage(string message)
        {
            if (!IsGatewayServiceConnected())
                return;

            string clientId = Context.ConnectionId;
            GatewayClientStatic.gatewayClient.SendMessage(clientId, message);
        }


        public void GetGatewayHardwareConnected()
        {
            string clientId = Context.ConnectionId;

            if (!IsGatewayServiceConnected())
            {
                Clients.Caller.returnGatewayHardwareConnected(false);
                return;
            }

            GatewayClientStatic.gatewayClient.GetGatewayHardwareConnected(clientId);
        }

        public void GetNodes()
        {
            if (!IsGatewayServiceConnected())
                return;

            string clientId = Context.ConnectionId;
            GatewayClientStatic.gatewayClient.GetNodes(clientId);
        }

        public void GetLog()
        {
            if (!IsGatewayServiceConnected())
                return;

            string clientId = Context.ConnectionId;
            GatewayClientStatic.gatewayClient.GetLog(clientId);
        }

        public void ClearLog()
        {
            if (!IsGatewayServiceConnected())
                return;

            string clientId = Context.ConnectionId;
            GatewayClientStatic.gatewayClient.ClearLog(clientId);
        }

        public void GetGatewayInfo()
        {
            if (!IsGatewayServiceConnected())
                return;

            string clientId = Context.ConnectionId;
            GatewayClientStatic.gatewayClient.GetGatewayInfo(clientId);
        }

        #endregion

    }

}