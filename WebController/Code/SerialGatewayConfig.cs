using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNetSensors.WebController.Code
{
    public class SerialGatewayConfig
    {
        public bool Enable { get; set; }
        public string SerialPortName{ get; set; }
        public int Boudrate{ get; set; }
    }
}
