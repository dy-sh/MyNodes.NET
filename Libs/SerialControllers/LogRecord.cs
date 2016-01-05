using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.SerialControllers
{
    public enum LogRecordType
    {
        GatewayState,
        GatewayMessages,
        GatewayRawMessages,
        DataBaseState,
        LogicalNodesEngine,
        LogicalNodes,
        SerialController
    }

    public class LogRecord
    {
        public LogRecordType Type { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }

        public LogRecord(){}

        public LogRecord(LogRecordType type, string message)
        {
            Date = DateTime.Now;
            Message = message;
            Type = type;
        }

        public override string ToString()
        {
            return $"{Date}: {Message}";
        }

        public string ToStringWithType()
        {
            switch (Type)
            {
                case LogRecordType.GatewayState:
                case LogRecordType.GatewayMessages:
                    return $"{Date}: GATEWAY: {Message}";
                case LogRecordType.GatewayRawMessages:
                    return $"{Date}: GATEWAY: RAW: {Message}";
                case LogRecordType.DataBaseState:
                    return $"{Date}: DATABASE: {Message}";
                case LogRecordType.LogicalNodesEngine:
                case LogRecordType.LogicalNodes:
                    return $"{Date}: LOGICAL NODES: {Message}";
                case LogRecordType.SerialController:
                    return $"{Date}: CONTROLLER: {Message}";
            }
            return null;
        }
    }
}
