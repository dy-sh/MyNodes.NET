using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNetSensors.WebController.Code
{
    public delegate void LogMessageEventHandler(LogRecord record);
    public class Logs
    {
        public List<LogRecord> gatewayLog = new List<LogRecord>();
        public List<LogRecord> hardwareNodesLog = new List<LogRecord>();
        public List<LogRecord> dataBaseLog = new List<LogRecord>();
        public List<LogRecord> nodesEngineLog = new List<LogRecord>();
        public List<LogRecord> nodesLog = new List<LogRecord>();
        public List<LogRecord> systemLog = new List<LogRecord>();

        public event LogMessageEventHandler OnGatewayLogInfo;
        public event LogMessageEventHandler OnGatewayLogError;
        public event LogMessageEventHandler OnHardwareNodeLogInfo;
        public event LogMessageEventHandler OnHardwareNodeLogError;
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
            LogRecord logRecord = new LogRecord(LogRecordOwner.Gateway, LogRecordType.Info, message);

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
            LogRecord logRecord = new LogRecord(LogRecordOwner.Gateway, LogRecordType.Error, message);

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

        public void AddHardwareNodeInfo(string message)
        {
            LogRecord logRecord = new LogRecord(LogRecordOwner.HardwareNodes, LogRecordType.Info, message);

            if (config.ShowGatewayMessages)
                Show(logRecord);

            OnHardwareNodeLogInfo?.Invoke(logRecord);

            if (config.StoreGatewayMessages)
            {
                hardwareNodesLog.Add(logRecord);
                if (hardwareNodesLog.Count > config.MaxGatewayMessages)
                    hardwareNodesLog.RemoveAt(0);
            }
        }

        public void AddHardwareNodeError(string message)
        {
            LogRecord logRecord = new LogRecord(LogRecordOwner.HardwareNodes, LogRecordType.Error, message);

            if (config.ShowGatewayMessages)
                Show(logRecord);

            OnHardwareNodeLogError?.Invoke(logRecord);

            if (config.StoreGatewayMessages)
            {
                hardwareNodesLog.Add(logRecord);
                if (hardwareNodesLog.Count > config.MaxGatewayMessages)
                    hardwareNodesLog.RemoveAt(0);
            }
        }
        
        public void AddDataBaseInfo(string message)
        {
            LogRecord logRecord = new LogRecord(LogRecordOwner.DataBase, LogRecordType.Info, message);

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
            LogRecord logRecord = new LogRecord(LogRecordOwner.DataBase, LogRecordType.Error, message);

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
            LogRecord logRecord = new LogRecord(LogRecordOwner.NodesEngine, LogRecordType.Info, message);

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
            LogRecord logRecord = new LogRecord(LogRecordOwner.NodesEngine, LogRecordType.Error, message);

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
            LogRecord logRecord = new LogRecord(LogRecordOwner.Nodes, LogRecordType.Info, message);

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
            LogRecord logRecord = new LogRecord(LogRecordOwner.Nodes, LogRecordType.Error, message);

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
            LogRecord logRecord = new LogRecord(LogRecordOwner.System, LogRecordType.Info, message);

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
            LogRecord logRecord = new LogRecord(LogRecordOwner.System, LogRecordType.Error, message);

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
            list.AddRange(hardwareNodesLog);
            list.AddRange(nodesEngineLog);
            list.AddRange(nodesLog);
            list.AddRange(dataBaseLog);
            list.AddRange(systemLog);
            return list.OrderBy(x => x.Date).ToList();
        }

        public List<LogRecord> GetErrorsLogs()
        {
            List<LogRecord> list = new List<LogRecord>();
            list.AddRange(gatewayLog.Where(x=>x.Type==LogRecordType.Error));
            list.AddRange(hardwareNodesLog.Where(x => x.Type == LogRecordType.Error));
            list.AddRange(nodesEngineLog.Where(x => x.Type == LogRecordType.Error));
            list.AddRange(nodesLog.Where(x => x.Type == LogRecordType.Error));
            list.AddRange(dataBaseLog.Where(x => x.Type == LogRecordType.Error));
            list.AddRange(systemLog.Where(x => x.Type == LogRecordType.Error));
            return list.OrderBy(x => x.Date).ToList();
        }

        public void ClearAllLogs()
        {
            gatewayLog.Clear();
            hardwareNodesLog.Clear();
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
                switch (record.Owner)
                {
                    case LogRecordOwner.Gateway:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case LogRecordOwner.HardwareNodes:
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        break;
                    case LogRecordOwner.DataBase:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogRecordOwner.NodesEngine:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case LogRecordOwner.Nodes:
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case LogRecordOwner.System:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
            }

            Console.WriteLine(record.ToStringWithType());
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
