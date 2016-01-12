using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.SerialControllers
{
    public enum LogRecordOwner
    {
        Gateway,
        Node,
        DataBase,
        LogicalNodesEngine,
        LogicalNode,
        SerialController
    }

    public enum LogRecordType
    {
        Info,
        Error
    }

    public class LogRecord
    {
        public LogRecordOwner Owner { get; set; }
        public LogRecordType Type { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }

        public LogRecord(){}

        public LogRecord(LogRecordOwner owner, LogRecordType type, string message)
        {
            Date = DateTime.Now;
            Owner = owner;
            Type = type;
            Message = message;
        }

        public override string ToString()
        {
            return $"{Date}: {Message}";
        }

        public string ToStringWithType()
        {
            switch (Owner)
            {
                case LogRecordOwner.Gateway:
                    return $"{Date}: GATEWAY: {Message}";
                    break;
                case LogRecordOwner.Node:
                    return $"{Date}: GATEWAY: {Message}";
                    break;
                case LogRecordOwner.DataBase:
                    return $"{Date}: DATABASE: {Message}";
                    break;
                case LogRecordOwner.LogicalNodesEngine:
                    return $"{Date}: LOGICAL NODES ENGINE: {Message}";
                    break;
                case LogRecordOwner.LogicalNode:
                    return $"{Date}: LOGICAL NODE: {Message}";
                    break;
                case LogRecordOwner.SerialController:
                    return $"{Date}: CONTROLLER: {Message}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return null;
        }
    }
}
