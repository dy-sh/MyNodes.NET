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
    public class ConsoleConfig
    {
        public bool ShowAllErrors { get; set; } = true;

        public bool ShowGatewayState { get; set; } = true;
        public bool ShowGatewayMessages { get; set; } = false;
        public bool ShowGatewayDecodedMessages { get; set; } = false;
        public bool ShowDataBaseState { get; set; } = true;
        public bool ShowNodesEngineState { get; set; } = true;
        public bool ShowNodesEngineNodes { get; set; } = false;
        public bool ShowSystemState { get; set; } = true;

    }
}
