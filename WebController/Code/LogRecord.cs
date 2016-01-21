using System;

namespace MyNetSensors.WebController.Code
{
    public enum LogRecordOwner
    {
        Gateway,
        HardwareNodes,
        DataBase,
        NodesEngine,
        Nodes,
        System
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
                case LogRecordOwner.HardwareNodes:
                    return $"{Date}: GATEWAY: {Message}";
                case LogRecordOwner.DataBase:
                    return $"{Date}: DATABASE: {Message}";
                case LogRecordOwner.NodesEngine:
                    return $"{Date}: NODES ENGINE: {Message}";
                case LogRecordOwner.Nodes:
                    return $"{Date}: NODE: {Message}";
                case LogRecordOwner.System:
                    return $"{Date}: SYSTEM: {Message}";
            }
            return null;
        }
    }
}
