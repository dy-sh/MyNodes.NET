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
            if (!config.EnableGatewayStateLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.Gateway, LogRecordType.Info, message);
            gatewayLog.Add(logRecord);
            if(gatewayLog.Count> config.MaxGatewayStateRecords)
                gatewayLog.RemoveAt(0);
            OnGatewayLogInfo?.Invoke(logRecord);
        }

        public void AddGatewayError(string message)
        {
            if (!config.EnableGatewayStateLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.Gateway, LogRecordType.Error, message);
            gatewayLog.Add(logRecord);
            if (gatewayLog.Count > config.MaxGatewayStateRecords)
                gatewayLog.RemoveAt(0);
            OnGatewayLogError?.Invoke(logRecord);
        }

        public void AddHardwareNodeInfo(string message)
        {
            if (!config.EnableGatewayMessagesLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.HardwareNodes, LogRecordType.Info, message);
            hardwareNodesLog.Add(logRecord);
            if (hardwareNodesLog.Count > config.MaxGatewayMessagesRecords)
                hardwareNodesLog.RemoveAt(0);
            OnHardwareNodeLogInfo?.Invoke(logRecord);
        }

        public void AddHardwareNodeError(string message)
        {
            if (!config.EnableGatewayMessagesLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.HardwareNodes, LogRecordType.Error, message);
            hardwareNodesLog.Add(logRecord);
            if (hardwareNodesLog.Count > config.MaxGatewayMessagesRecords)
                hardwareNodesLog.RemoveAt(0);
            OnHardwareNodeLogError?.Invoke(logRecord);
        }

    


        public void AddDataBaseInfo(string message)
        {
            if (!config.EnableDataBaseStateLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.DataBase, LogRecordType.Info, message);
            dataBaseLog.Add(logRecord);
            if (dataBaseLog.Count > config.MaxDataBaseStateRecords)
                dataBaseLog.RemoveAt(0);
            OnDataBaseLogInfo?.Invoke(logRecord);
        }

        public void AddDataBaseError(string message)
        {
            if (!config.EnableDataBaseStateLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.DataBase, LogRecordType.Error, message);
            dataBaseLog.Add(logRecord);
            if (dataBaseLog.Count > config.MaxDataBaseStateRecords)
                dataBaseLog.RemoveAt(0);
            OnDataBaseLogError?.Invoke(logRecord);
        }

        public void AddNodesEngineInfo(string message)
        {
            if (!config.EnableNodesEngineStateLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.NodesEngine, LogRecordType.Info, message);
            nodesEngineLog.Add(logRecord);
            if (nodesEngineLog.Count > config.MaxNodesEngineStateRecords)
                nodesEngineLog.RemoveAt(0);
            OnNodesEngineLogInfo?.Invoke(logRecord);
        }

        public void AddNodesEngineError(string message)
        {
            if (!config.EnableNodesEngineStateLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.NodesEngine, LogRecordType.Error, message);
            nodesEngineLog.Add(logRecord);
            if (nodesEngineLog.Count > config.MaxNodesEngineStateRecords)
                nodesEngineLog.RemoveAt(0);
            OnNodesEngineLogError?.Invoke(logRecord);
        }

        public void AddNodeInfo(string message)
        {
            if (!config.EnableNodesEngineNodesLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.Nodes, LogRecordType.Info, message);
            nodesLog.Add(logRecord);
            if (nodesLog.Count > config.MaxNodesEngineNodesRecords)
                nodesLog.RemoveAt(0);
            OnNodeLogInfo?.Invoke(logRecord);
        }
        public void AddNodeError(string message)
        {
            if (!config.EnableNodesEngineNodesLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.Nodes, LogRecordType.Error, message);
            nodesLog.Add(logRecord);
            if (nodesLog.Count > config.MaxNodesEngineNodesRecords)
                nodesLog.RemoveAt(0);
            OnNodeLogError?.Invoke(logRecord);
        }


        public void AddSystemInfo(string message)
        {
            if (!config.EnableSystemStateLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.System, LogRecordType.Info, message);
            systemLog.Add(logRecord);
            if (systemLog.Count > config.MaxSystemStateRecords)
                systemLog.RemoveAt(0);
            OnSystemLogInfo?.Invoke(logRecord);
        }

        public void AddSystemError(string message)
        {
            if (!config.EnableSystemStateLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.System, LogRecordType.Error, message);
            systemLog.Add(logRecord);
            if (systemLog.Count > config.MaxSystemStateRecords)
                systemLog.RemoveAt(0);
            OnSystemLogError?.Invoke(logRecord);
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
    }
}
