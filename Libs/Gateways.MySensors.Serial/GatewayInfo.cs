/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Gateways.MySensors.Serial
{
    public class GatewayInfo
    {
        public GatewayState state;
        public bool isGatewayConnected;
        public int gatewayNodesRegistered;
        public int gatewaySensorsRegistered;
    }
}