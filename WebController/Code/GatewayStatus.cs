using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyNetSensors.WebController.Code
{
    public class GatewayStatus
    {
        public bool gatewayHardwareOnline;
        public bool gatewayServiceOnline;
        public int gatewayConectedNodes;
        public int gatewayConectedSensors;
    }
}