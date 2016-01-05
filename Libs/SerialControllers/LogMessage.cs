using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.SerialControllers
{
    public enum LogMessageType
    {
        GatewayState,
        GatewayTxRx,
        GatewayRawTxRx,
        DataBaseState,
        LogicalNodesEngine,
        LogicalNodes,
        SerialController
    }

    public class LogMessage
    {
        public LogMessageType Type { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }

        public LogMessage(){}

        public LogMessage(LogMessageType type, string message)
        {
            Date = DateTime.Now;
            Message = message;
        }

        public override string ToString()
        {
            return $"{Date}: {Message}";
        }

        public string ToStringWithType()
        {
            switch (Type)
            {
                case LogMessageType.GatewayState:
                case LogMessageType.GatewayTxRx:
                    return $"{Date}: GATEWAY: {Message}";
                case LogMessageType.GatewayRawTxRx:
                    return $"{Date}: GATEWAY: RAW: {Message}";
                case LogMessageType.DataBaseState:
                    return $"{Date}: DATABASE: {Message}";
                case LogMessageType.LogicalNodesEngine:
                    return $"{Date}: LOGICAL NODES ENGINE: {Message}";
                case LogMessageType.LogicalNodes:
                    return $"{Date}: LOGICAL NODES: {Message}";
                case LogMessageType.SerialController:
                    return $"{Date}: SERIAL CONTROLLER: {Message}";
            }
            return null;
        }
    }
}
