using System;
using System.Timers;

namespace MyNetSensors.Gateways.MySensors.Serial
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
            gateway.OnUnexpectedlyDisconnectedEvent += Stop;
            gateway.OnDisconnectedEvent += Stop;
            gateway.OnConnectedEvent += Start;
            gateway.OnMessageRecievedEvent += OnMessageRecievedEvent;
            checkGatewayTimer.Elapsed += CheckGatewayAlive;
        }




        private void CheckGatewayAlive(object sender, ElapsedEventArgs e)
        {
            if (!gateway.IsConnected())
                return;

            double lastResponseAgo = (DateTime.Now - checkGatewayLastResponseTime).TotalMilliseconds;
            if (lastResponseAgo > checkGatewayTimer.Interval * 2)
            {
                //gateway not responding
                gateway.LogError("Gateway not responding.");

                gateway.OnSerialPortDisconnectedEvent();
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


        private void OnMessageRecievedEvent(Message message)
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
