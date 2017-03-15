/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/


namespace MyNodes.WebController.Code
{
    public class GatewayConfig
    {
        public bool EnableAutoAssignId { get; set; }
        public bool LogState { get; set; }
        public bool LogMessages { get; set; }

        public SerialGatewayConfig SerialGateway { get; set; }
        public EthernetGatewayConfig EthernetGateway { get; set; }
    }
}
