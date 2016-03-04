/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNodes.WebController.Code
{
    public class LogsConfig
    {
        public bool ShowAllErrors { get; set; } = true;

        public bool ShowGatewayState { get; set; } = true;
        public bool ShowGatewayMessages { get; set; } = true;
        public bool ShowGatewayDecodedMessages { get; set; } = true;
        public bool ShowDataBaseState { get; set; } = true;
        public bool ShowNodesEngineState { get; set; } = true;
        public bool ShowNodesEngineNodes { get; set; } = true;
        public bool ShowSystemState { get; set; } = true;

        public bool StoreGatewayState { get; set; } = true;
        public bool StoreGatewayMessages { get; set; } = true;
        public bool StoreGatewayDecodedMessages { get; set; } = true;
        public bool StoreDataBaseState { get; set; } = true;
        public bool StoreNodesEngineState { get; set; } = true;
        public bool StoreNodesEngineNodes { get; set; } = true;
        public bool StoreSystemState { get; set; } = true;

        public int MaxGatewayState { get; set; } = 1000;
        public int MaxGatewayMessages { get; set; } = 1000;
        public int MaxGatewayDecodedMessages { get; set; } = 1000;
        public int MaxDataBaseState { get; set; } = 1000;
        public int MaxNodesEngineState { get; set; } = 1000;
        public int MaxNodesEngineNodes { get; set; } = 1000;
        public int MaxSystemState { get; set; } = 1000;

    }
}
