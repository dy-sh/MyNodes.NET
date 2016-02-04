/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using MyNetSensors.Gateways.MySensors;

namespace MyNetSensors.WebController.Code
{
    public enum GatewayType
    {
        Serial,
        Ethernet
    }

    public class GatewayInfo
    {
        public GatewayState state;
        public GatewayType type;
        public bool isGatewayConnected;
        public int gatewayNodesRegistered;
        public int gatewaySensorsRegistered;
    }
}