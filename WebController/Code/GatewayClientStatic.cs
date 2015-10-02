/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MyNetSensors.Gateway;

namespace MyNetSensors.WebController.Code
{

    static public class GatewayClientStatic
    {
        private static Object lock1 = new Object();

        public static IGatewayClient gatewayClient;
        public static List<string> connectedUsersId = new List<string>();

        public static void ConnectToServer()
        {
            lock (lock1)
            {
                if (gatewayClient != null) return;
                gatewayClient = new GatewayClient();

                gatewayClient.ConnectToServer();
            }
        }
    }
}