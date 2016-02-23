/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNetSensors.WebController.Code
{
    public class GatewayConfig
    {
        public bool EnableAutoAssignId { get; set; }
        public bool EnableMessagesLog { get; set; }
        public bool LogState { get; set; }
        public bool LogMessages { get; set; }

        public SerialGatewayConfig SerialGatewayConfig { get; set; }
        public EthernetGatewayConfig EthernetGatewayConfig { get; set; }
    }
}
