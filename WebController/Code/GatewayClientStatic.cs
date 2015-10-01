/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using MyNetSensors.Gateway;

namespace MyNetSensors.WebController.Code
{
    static public class GatewayClientStatic
    {
        public static IGatewayClient gatewayClient;
        public static List<string> connectedUsersId = new List<string>();

        public static void ConnectToServer()
        {
            if (gatewayClient!=null) return;

            gatewayClient = new GatewayClient();
            gatewayClient.ConnectToServer();
        }
    }
}