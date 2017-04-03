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

        /// <summary>
        /// Whether to use metric units for all nodes. Defaults to <c>true</c>. This config maps to 
        /// the value nodes retrieve with <code>getControllerConfig().isMetric</code>.
        /// </summary>
        public bool IsMetric { get; set; } = true;

        public SerialGatewayConfig SerialGateway { get; set; }
        public EthernetGatewayConfig EthernetGateway { get; set; }
    }
}
