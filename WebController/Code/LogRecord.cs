/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.WebController.Code
{
    public enum LogRecordSource
    {
        Gateway,
        GatewayMessage,
        GatewayDecodedMessage,
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
        public LogRecordSource Source { get; set; }
        public LogRecordType Type { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }

        public LogRecord(){}

        public LogRecord(LogRecordSource source, LogRecordType type, string message)
        {
            Date = DateTime.Now;
            Source = source;
            Type = type;
            Message = message;
        }

        public override string ToString()
        {
            return $"{Date}: {Message}";
        }

        public string ToStringWithType()
        {
            switch (Source)
            {
                case LogRecordSource.Gateway:
                    return $"{Date}: GATEWAY: {Message}";
                case LogRecordSource.GatewayMessage:
                    return $"{Date}: GATEWAY: {Message}";
                case LogRecordSource.GatewayDecodedMessage:
                    return $"{Date}: GATEWAY: {Message}";
                case LogRecordSource.DataBase:
                    return $"{Date}: DATABASE: {Message}";
                case LogRecordSource.NodesEngine:
                    return $"{Date}: NODES ENGINE: {Message}";
                case LogRecordSource.Nodes:
                    return $"{Date}: NODE: {Message}";
                case LogRecordSource.System:
                    return $"{Date}: SYSTEM: {Message}";
            }
            return null;
        }
    }
}
