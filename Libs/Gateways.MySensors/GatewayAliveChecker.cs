/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Timers;

namespace MyNodes.Gateways.MySensors
{
    public class GatewayAliveChecker
    {

       // public bool checkGatewayIsAlive = true;
        public bool checkGatewayIsAlive = true;
        public int checkGatewayIsAliveInterval = 10000;
        private DateTime checkGatewayLastRequestTime;
        private DateTime checkGatewayLastResponseTime;

        private Timer checkGatewayTimer = new Timer();
        private Gateway gateway;

        public GatewayAliveChecker(Gateway gateway)
        {
            this.gateway = gateway;
            gateway.OnUnexpectedlyDisconnected += Stop;
            gateway.OnDisconnected += Stop;
            gateway.OnConnected += Start;
            gateway.OnMessageRecieved += OnMessageRecieved;
            checkGatewayTimer.Elapsed += CheckGatewayAlive;
        }




        private void CheckGatewayAlive(object sender, ElapsedEventArgs e)
        {
            if (!gateway.IsConnected)
                return;

            double lastResponseAgo = (DateTime.Now - checkGatewayLastResponseTime).TotalMilliseconds;
            if (lastResponseAgo > checkGatewayTimer.Interval * 2)
            {
                //gateway not responding
                gateway.LogError("Gateway not responding.");

                gateway.OnConnectionPortDisconnected();
            }
            else
            {
                //send new request
                checkGatewayLastRequestTime = DateTime.Now;
                gateway.SendGetwayVersionRequest();
            }
        }

        public void Start()
        {
            checkGatewayLastResponseTime = DateTime.Now;
            checkGatewayLastRequestTime = DateTime.Now;
            checkGatewayTimer.Interval = checkGatewayIsAliveInterval;
            checkGatewayTimer.Start();
        }

        public void Stop()
        {
            checkGatewayTimer.Stop();
        }


        private void OnMessageRecieved(Message message)
        {
            //Gateway vesrion (alive) respond
            if (message.nodeId == 0
                && message.messageType == MessageType.C_INTERNAL
                && message.subType == (int)InternalDataType.I_VERSION)
                ProceedAliveMessage(message);
        }

        private void ProceedAliveMessage(Message message)
        {
            checkGatewayLastResponseTime = DateTime.Now;
        }
    }
}
