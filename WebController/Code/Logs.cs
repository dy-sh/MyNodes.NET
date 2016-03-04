/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using MyNodes.Gateways.MySensors;

namespace MyNodes.WebController.Code
{
    public delegate void LogMessageEventHandler(LogRecord record);
    public delegate void LogMySensorsMessageEventHandler(Message record);
    public class Logs
    {
        public List<LogRecord> gatewayLog = new List<LogRecord>();
        public List<LogRecord> gatewayMessagesLog = new List<LogRecord>();
        public List<Message> gatewayDecodedMessagesLog = new List<Message>();
        public List<LogRecord> dataBaseLog = new List<LogRecord>();
        public List<LogRecord> nodesEngineLog = new List<LogRecord>();
        public List<LogRecord> nodesLog = new List<LogRecord>();
        public List<LogRecord> systemLog = new List<LogRecord>();

        public event LogMessageEventHandler OnGatewayLogInfo;
        public event LogMessageEventHandler OnGatewayLogError;
        public event LogMessageEventHandler OnGatewayMessageLog;
        public event LogMySensorsMessageEventHandler OnGatewayDecodedMessageLog;
        public event LogMessageEventHandler OnDataBaseLogInfo;
        public event LogMessageEventHandler OnDataBaseLogError;
        public event LogMessageEventHandler OnNodesEngineLogInfo;
        public event LogMessageEventHandler OnNodesEngineLogError;
        public event LogMessageEventHandler OnNodeLogInfo;
        public event LogMessageEventHandler OnNodeLogError;
        public event LogMessageEventHandler OnSystemLogInfo;
        public event LogMessageEventHandler OnSystemLogError;

        public LogsConfig config;




        public void AddGatewayInfo(string message)
        {
            LogRecord logRecord = new LogRecord(LogRecordSource.Gateway, LogRecordType.Info, message);

            if (config.ShowGatewayState)
                Show(logRecord);

            OnGatewayLogInfo?.Invoke(logRecord);

            if (config.StoreGatewayState)
            {
                gatewayLog.Add(logRecord);
                if (gatewayLog.Count > config.MaxGatewayState)
                    gatewayLog.RemoveAt(0);
            }
        }

        public void AddGatewayError(string message)
        {
            LogRecord logRecord = new LogRecord(LogRecordSource.Gateway, LogRecordType.Error, message);

            if (config.ShowGatewayState)
                Show(logRecord);

            OnGatewayLogError?.Invoke(logRecord);

            if (config.StoreGatewayState)
            {
                gatewayLog.Add(logRecord);
                if (gatewayLog.Count > config.MaxGatewayState)
                    gatewayLog.RemoveAt(0);
            }
        }

        public void AddGatewayDecodedMessage(Message message)
        {
            LogRecord logRecord = new LogRecord(LogRecordSource.GatewayDecodedMessages, LogRecordType.Info, message.ToString());

            if (config.ShowGatewayDecodedMessages)
                Show(logRecord);

            OnGatewayDecodedMessageLog?.Invoke(message);

            if (config.StoreGatewayDecodedMessages)
            {
                gatewayDecodedMessagesLog.Add(message);
                if (gatewayDecodedMessagesLog.Count > config.MaxGatewayDecodedMessages)
                    gatewayDecodedMessagesLog.RemoveAt(0);
            }
        }

        public void AddGatewayMessage(string message)
        {
            LogRecord logRecord = new LogRecord(LogRecordSource.GatewayMessages, LogRecordType.Info, message);

            if (config.ShowGatewayMessages)
                Show(logRecord);

            OnGatewayMessageLog?.Invoke(logRecord);

            if (config.StoreGatewayMessages)
            {
                gatewayMessagesLog.Add(logRecord);
                if (gatewayMessagesLog.Count > config.MaxGatewayMessages)
                    gatewayMessagesLog.RemoveAt(0);
            }
        }

        public void AddDataBaseInfo(string message)
        {
            LogRecord logRecord = new LogRecord(LogRecordSource.DataBase, LogRecordType.Info, message);

            if (config.ShowDataBaseState)
                Show(logRecord);

            OnDataBaseLogInfo?.Invoke(logRecord);

            if (config.StoreDataBaseState)
            {
                dataBaseLog.Add(logRecord);
                if (dataBaseLog.Count > config.MaxDataBaseState)
                    dataBaseLog.RemoveAt(0);
            }
        }

        public void AddDataBaseError(string message)
        {
            LogRecord logRecord = new LogRecord(LogRecordSource.DataBase, LogRecordType.Error, message);

            if (config.ShowDataBaseState)
                Show(logRecord);

            OnDataBaseLogError?.Invoke(logRecord);

            if (config.StoreDataBaseState)
            {
                dataBaseLog.Add(logRecord);
                if (dataBaseLog.Count > config.MaxDataBaseState)
                    dataBaseLog.RemoveAt(0);
            }
        }

        public void AddNodesEngineInfo(string message)
        {
            LogRecord logRecord = new LogRecord(LogRecordSource.NodesEngine, LogRecordType.Info, message);

            if (config.ShowNodesEngineState)
                Show(logRecord);

            OnNodesEngineLogInfo?.Invoke(logRecord);

            if (config.StoreNodesEngineState)
            {
                nodesEngineLog.Add(logRecord);
                if (nodesEngineLog.Count > config.MaxNodesEngineState)
                    nodesEngineLog.RemoveAt(0);
            }
        }

        public void AddNodesEngineError(string message)
        {
            LogRecord logRecord = new LogRecord(LogRecordSource.NodesEngine, LogRecordType.Error, message);

            if (config.ShowNodesEngineState)
                Show(logRecord);

            OnNodesEngineLogError?.Invoke(logRecord);

            if (config.StoreNodesEngineState)
            {
                nodesEngineLog.Add(logRecord);
                if (nodesEngineLog.Count > config.MaxNodesEngineState)
                    nodesEngineLog.RemoveAt(0);
            }
        }

        public void AddNodeInfo(string message)
        {
            LogRecord logRecord = new LogRecord(LogRecordSource.Nodes, LogRecordType.Info, message);

            if (config.ShowNodesEngineNodes)
                Show(logRecord);

            OnNodeLogInfo?.Invoke(logRecord);

            if (config.StoreNodesEngineNodes)
            {
                nodesLog.Add(logRecord);
                if (nodesLog.Count > config.MaxNodesEngineNodes)
                    nodesLog.RemoveAt(0);
            }
        }
        public void AddNodeError(string message)
        {
            LogRecord logRecord = new LogRecord(LogRecordSource.Nodes, LogRecordType.Error, message);

            if (config.ShowNodesEngineNodes)
                Show(logRecord);

            OnNodeLogError?.Invoke(logRecord);

            if (config.StoreNodesEngineNodes)
            {
                nodesLog.Add(logRecord);
                if (nodesLog.Count > config.MaxNodesEngineNodes)
                    nodesLog.RemoveAt(0);
            }
        }


        public void AddSystemInfo(string message)
        {
            LogRecord logRecord = new LogRecord(LogRecordSource.System, LogRecordType.Info, message);

            if (config.ShowSystemState)
                Show(logRecord);

            OnSystemLogInfo?.Invoke(logRecord);

            if (config.StoreSystemState)
            {
                systemLog.Add(logRecord);
                if (systemLog.Count > config.MaxSystemState)
                    systemLog.RemoveAt(0);
            }
        }

        public void AddSystemError(string message)
        {
            LogRecord logRecord = new LogRecord(LogRecordSource.System, LogRecordType.Error, message);

            if (config.ShowSystemState)
                Show(logRecord);

            OnSystemLogError?.Invoke(logRecord);

            if (config.StoreSystemState)
            {
                systemLog.Add(logRecord);
                if (systemLog.Count > config.MaxSystemState)
                    systemLog.RemoveAt(0);
            }
        }




        public List<LogRecord> GetAllLogs()
        {
            List<LogRecord> list = new List<LogRecord>();
            list.AddRange(gatewayLog);
            list.AddRange(gatewayMessagesLog);
            list.AddRange(nodesEngineLog);
            list.AddRange(nodesLog);
            list.AddRange(dataBaseLog);
            list.AddRange(systemLog);
            return list.OrderBy(x => x.Date).ToList();
        }

        public List<LogRecord> GetErrorsLogs()
        {
            List<LogRecord> list = new List<LogRecord>();
            list.AddRange(gatewayLog.ToArray().Where(x=>x.Type==LogRecordType.Error));
            list.AddRange(nodesEngineLog.ToArray().Where(x => x.Type == LogRecordType.Error));
            list.AddRange(nodesLog.ToArray().Where(x => x.Type == LogRecordType.Error));
            list.AddRange(dataBaseLog.ToArray().Where(x => x.Type == LogRecordType.Error));
            list.AddRange(systemLog.ToArray().Where(x => x.Type == LogRecordType.Error));
            return list.OrderBy(x => x.Date).ToList();
        }


        public List<LogRecord> GetLogsOfSource(LogRecordSource logRecordSource)
        {
            switch (logRecordSource)
            {
                case LogRecordSource.Gateway:
                    return gatewayLog;
                case LogRecordSource.GatewayMessages:
                    return gatewayMessagesLog;
                case LogRecordSource.GatewayDecodedMessages:
                    return gatewayDecodedMessagesLog.Select(log => new LogRecord(LogRecordSource.GatewayDecodedMessages, LogRecordType.Info, log.ToString())).ToList(); ;
                case LogRecordSource.DataBase:
                    return dataBaseLog;
                case LogRecordSource.NodesEngine:
                    return nodesEngineLog;
                case LogRecordSource.Nodes:
                    return nodesLog;
                case LogRecordSource.System:
                    return systemLog;
            }
            return null;
        }

        public void ClearLogsOfSource(LogRecordSource logRecordSource)
        {
            switch (logRecordSource)
            {
                case LogRecordSource.Gateway:
                    gatewayLog.Clear();
                    break;
                case LogRecordSource.GatewayMessages:
                    gatewayMessagesLog.Clear();
                    break;
                case LogRecordSource.GatewayDecodedMessages:
                    gatewayDecodedMessagesLog.Clear();
                    break;
                case LogRecordSource.DataBase:
                    dataBaseLog.Clear();
                    break;
                case LogRecordSource.NodesEngine:
                    nodesEngineLog.Clear();
                    break;
                case LogRecordSource.Nodes:
                    nodesLog.Clear();
                    break;
                case LogRecordSource.System:
                    systemLog.Clear();
                    break;
            }
        }

        public void ClearAllLogs()
        {
            gatewayLog.Clear();
            gatewayMessagesLog.Clear();
            gatewayDecodedMessagesLog.Clear();
            nodesEngineLog.Clear();
            nodesLog.Clear();
            dataBaseLog.Clear();
            systemLog.Clear();
        }


        public static void Show(LogRecord record)
        {
            if (record.Type == LogRecordType.Error)
                Console.ForegroundColor = ConsoleColor.Red;
            else
            {
                switch (record.Source)
                {
                    case LogRecordSource.Gateway:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case LogRecordSource.GatewayMessages:
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        break;
                    case LogRecordSource.GatewayDecodedMessages:
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        break;
                    case LogRecordSource.DataBase:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogRecordSource.NodesEngine:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case LogRecordSource.Nodes:
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case LogRecordSource.System:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
            }

            Console.WriteLine(record.ToStringWithType());
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
