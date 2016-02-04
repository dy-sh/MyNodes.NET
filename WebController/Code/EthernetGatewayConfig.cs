using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNetSensors.WebController.Code
{
    public class EthernetGatewayConfig
    {
        public bool Enable { get; set; }
        public string GatewayIP { get; set; }
        public int GatewayPort { get; set; }
    }
}
