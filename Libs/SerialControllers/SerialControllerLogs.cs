using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.SerialControllers
{
    public delegate void LogMessageEventHandler(LogRecord record);
    public class SerialControllerLogs
    {
        public List<LogRecord> gatewayLog = new List<LogRecord>();
        public List<LogRecord> nodesLog = new List<LogRecord>();
        public List<LogRecord> dataBaseLog = new List<LogRecord>();
        public List<LogRecord> logicalNodesEngineLog = new List<LogRecord>();
        public List<LogRecord> logicalNodesLog = new List<LogRecord>();
        public List<LogRecord> serialControllerLog = new List<LogRecord>();

        public event LogMessageEventHandler OnGatewayLogInfo;
        public event LogMessageEventHandler OnGatewayLogError;
        public event LogMessageEventHandler OnNodeLogInfo;
        public event LogMessageEventHandler OnNodeLogError;
        public event LogMessageEventHandler OnDataBaseLogInfo;
        public event LogMessageEventHandler OnDataBaseLogError;
        public event LogMessageEventHandler OnLogicalNodesEngineLogInfo;
        public event LogMessageEventHandler OnLogicalNodesEngineLogError;
        public event LogMessageEventHandler OnLogicalNodeLogInfo;
        public event LogMessageEventHandler OnLogicalNodeLogError;
        public event LogMessageEventHandler OnSerialControllerLogInfo;
        public event LogMessageEventHandler OnSerialControllerLogError;

        public bool enableGatewayLog = true;
        public bool enableNodesLog = true;
        public bool enableDataBaseLog = true;
        public bool enableLogicalNodesEngineLog = true;
        public bool enableLogicalNodesLog = true;
        public bool enableSerialControllerLog = true;

        public int maxGatewayRecords = 1000;
        public int maxNodesRecords = 1000;
        public int maxDataBaseRecords = 1000;
        public int maxLogicalNodesEngineRecords = 1000;
        public int maxLogicalNodesRecords = 1000;
        public int maxSerialControllerRecords = 1000;


        public void AddGatewayInfo(string message)
        {
            if (!enableGatewayLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.Gateway, LogRecordType.Info, message);
            gatewayLog.Add(logRecord);
            if(gatewayLog.Count> maxGatewayRecords)
                gatewayLog.RemoveAt(0);
            OnGatewayLogInfo?.Invoke(logRecord);
        }

        public void AddGatewayError(string message)
        {
            if (!enableGatewayLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.Gateway, LogRecordType.Error, message);
            gatewayLog.Add(logRecord);
            if (gatewayLog.Count > maxGatewayRecords)
                gatewayLog.RemoveAt(0);
            OnGatewayLogError?.Invoke(logRecord);
        }

        public void AddNodeInfo(string message)
        {
            if (!enableNodesLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.Node, LogRecordType.Info, message);
            nodesLog.Add(logRecord);
            if (nodesLog.Count > maxNodesRecords)
                nodesLog.RemoveAt(0);
            OnNodeLogInfo?.Invoke(logRecord);
        }

        public void AddNodeError(string message)
        {
            if (!enableNodesLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.Node, LogRecordType.Error, message);
            nodesLog.Add(logRecord);
            if (nodesLog.Count > maxNodesRecords)
                nodesLog.RemoveAt(0);
            OnNodeLogError?.Invoke(logRecord);
        }

    


        public void AddDataBaseInfo(string message)
        {
            if (!enableDataBaseLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.DataBase, LogRecordType.Info, message);
            dataBaseLog.Add(logRecord);
            if (dataBaseLog.Count > maxDataBaseRecords)
                dataBaseLog.RemoveAt(0);
            OnDataBaseLogInfo?.Invoke(logRecord);
        }

        public void AddDataBaseError(string message)
        {
            if (!enableDataBaseLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.DataBase, LogRecordType.Error, message);
            dataBaseLog.Add(logRecord);
            if (dataBaseLog.Count > maxDataBaseRecords)
                dataBaseLog.RemoveAt(0);
            OnDataBaseLogError?.Invoke(logRecord);
        }

        public void AddLogicalNodesEngineInfo(string message)
        {
            if (!enableLogicalNodesEngineLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.LogicalNodesEngine, LogRecordType.Info, message);
            logicalNodesEngineLog.Add(logRecord);
            if (logicalNodesEngineLog.Count > maxLogicalNodesEngineRecords)
                logicalNodesEngineLog.RemoveAt(0);
            OnLogicalNodesEngineLogInfo?.Invoke(logRecord);
        }

        public void AddLogicalNodesEngineError(string message)
        {
            if (!enableLogicalNodesEngineLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.LogicalNodesEngine, LogRecordType.Error, message);
            logicalNodesEngineLog.Add(logRecord);
            if (logicalNodesEngineLog.Count > maxLogicalNodesEngineRecords)
                logicalNodesEngineLog.RemoveAt(0);
            OnLogicalNodesEngineLogError?.Invoke(logRecord);
        }

        public void AddLogicalNodeInfo(string message)
        {
            if (!enableLogicalNodesLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.LogicalNode, LogRecordType.Info, message);
            logicalNodesLog.Add(logRecord);
            if (logicalNodesLog.Count > maxLogicalNodesRecords)
                logicalNodesLog.RemoveAt(0);
            OnLogicalNodeLogInfo?.Invoke(logRecord);
        }
        public void AddLogicalNodeError(string message)
        {
            if (!enableLogicalNodesLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.LogicalNode, LogRecordType.Error, message);
            logicalNodesLog.Add(logRecord);
            if (logicalNodesLog.Count > maxLogicalNodesRecords)
                logicalNodesLog.RemoveAt(0);
            OnLogicalNodeLogError?.Invoke(logRecord);
        }


        public void AddSerialControllerInfo(string message)
        {
            if (!enableSerialControllerLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.SerialController, LogRecordType.Info, message);
            serialControllerLog.Add(logRecord);
            if (serialControllerLog.Count > maxSerialControllerRecords)
                serialControllerLog.RemoveAt(0);
            OnSerialControllerLogInfo?.Invoke(logRecord);
        }

        public void AddSerialControllerError(string message)
        {
            if (!enableSerialControllerLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordOwner.SerialController, LogRecordType.Error, message);
            serialControllerLog.Add(logRecord);
            if (serialControllerLog.Count > maxSerialControllerRecords)
                serialControllerLog.RemoveAt(0);
            OnSerialControllerLogError?.Invoke(logRecord);
        }

        public List<LogRecord> GetAllLogs()
        {
            List<LogRecord> list = new List<LogRecord>();
            list.AddRange(gatewayLog);
            list.AddRange(nodesLog);
            list.AddRange(logicalNodesEngineLog);
            list.AddRange(logicalNodesLog);
            list.AddRange(dataBaseLog);
            list.AddRange(serialControllerLog);
            return list.OrderBy(x => x.Date).ToList();
        }

        public List<LogRecord> GetAllErrorLogs()
        {
            List<LogRecord> list = new List<LogRecord>();
            list.AddRange(gatewayLog.Where(x=>x.Type==LogRecordType.Error));
            list.AddRange(nodesLog.Where(x => x.Type == LogRecordType.Error));
            list.AddRange(logicalNodesEngineLog.Where(x => x.Type == LogRecordType.Error));
            list.AddRange(logicalNodesLog.Where(x => x.Type == LogRecordType.Error));
            list.AddRange(dataBaseLog.Where(x => x.Type == LogRecordType.Error));
            list.AddRange(serialControllerLog.Where(x => x.Type == LogRecordType.Error));
            return list.OrderBy(x => x.Date).ToList();
        }

        public void ClearAllLogs()
        {
            gatewayLog.Clear();
            nodesLog.Clear();
            logicalNodesEngineLog.Clear();
            logicalNodesLog.Clear();
            dataBaseLog.Clear();
            serialControllerLog.Clear();
        }
    }
}
