using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNetSensors.WebController.Code
{
    public class LogsConfig
    {
        public bool EnableGatewayStateLog { get; set; } = true;
        public bool EnableGatewayMessagesLog { get; set; } = true;
        public bool EnableDataBaseStateLog { get; set; } = true;
        public bool EnableNodesEngineStateLog { get; set; } = true;
        public bool EnableNodesEngineNodesLog { get; set; } = true;
        public bool EnableSystemStateLog { get; set; } = true;

        public int MaxGatewayStateRecords { get; set; } = 1000;
        public int MaxGatewayMessagesRecords { get; set; } = 1000;
        public int MaxDataBaseStateRecords { get; set; } = 1000;
        public int MaxNodesEngineStateRecords { get; set; } = 1000;
        public int MaxNodesEngineNodesRecords { get; set; } = 1000;
        public int MaxSystemStateRecords { get; set; } = 1000;

    }
}
