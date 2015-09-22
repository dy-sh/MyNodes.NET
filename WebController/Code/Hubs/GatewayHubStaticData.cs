using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyNetSensors.WebController.Code.Hubs
{
    public static class GatewayHubStaticData
    {
        public static List<string> connectedUsersId = new List<string>();
        public static string gatewayId;
    }
}