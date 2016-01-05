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
        public List<LogRecord> gatewayMessagesLog = new List<LogRecord>();
        public List<LogRecord> gatewayRawMessagesLog = new List<LogRecord>();
        public List<LogRecord> gatewayStateLog = new List<LogRecord>();
        public List<LogRecord> dataBaseStateLog = new List<LogRecord>();
        public List<LogRecord> logicalNodesLog = new List<LogRecord>();
        public List<LogRecord> logicalNodesEngineLog = new List<LogRecord>();
        public List<LogRecord> serialControllerLog = new List<LogRecord>();

        public event LogMessageEventHandler OnGatewayMessagesLog;
        public event LogMessageEventHandler OnGatewayRawMessagesLog;
        public event LogMessageEventHandler OnGatewayStateLog;
        public event LogMessageEventHandler OnDataBaseStateLog;
        public event LogMessageEventHandler OnLogicalNodesLog;
        public event LogMessageEventHandler OnLogicalNodesEngineLog;
        public event LogMessageEventHandler OnSerialControllerLog;

        public bool enableGatewayStateLog = true;
        public bool enableGatewayMessagesLog = true;
        public bool enableGatewayRawMessagesLog = false;
        public bool enableDataBaseStateLog = true;
        public bool enableLogicalNodesLog = true;
        public bool enableLogicalNodesEngineLog = true;
        public bool enableSerialControllerLog = true;

        public int maxGatewayStateRecords = 1000;
        public int maxGatewayMessagesRecords = 1000;
        public int maxGatewayRawMessagesRecords = 1000;
        public int maxDataBaseStateRecords = 1000;
        public int maxLogicalNodesRecords = 1000;
        public int maxLogicalNodesEngineRecords = 1000;
        public int maxSerialControllerRecords = 1000;


        public void AddGatewayState(string message)
        {
            if (!enableGatewayStateLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordType.GatewayState, message);
            gatewayStateLog.Add(logRecord);
            if(gatewayStateLog.Count> maxGatewayStateRecords)
                gatewayStateLog.RemoveAt(0);
            OnGatewayStateLog?.Invoke(logRecord);
        }

        public void AddGatewayMessage(string message)
        {
            if (!enableGatewayMessagesLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordType.GatewayMessages, message);
            gatewayMessagesLog.Add(logRecord);
            if (gatewayMessagesLog.Count > maxGatewayMessagesRecords)
                gatewayMessagesLog.RemoveAt(0);
            OnGatewayMessagesLog?.Invoke(logRecord);
        }

        public void AddGatewayRawMessage(string message)
        {
            if (!enableGatewayRawMessagesLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordType.GatewayRawMessages, message);
            gatewayRawMessagesLog.Add(logRecord);
            if (gatewayRawMessagesLog.Count > maxGatewayRawMessagesRecords)
                gatewayRawMessagesLog.RemoveAt(0);
            OnGatewayRawMessagesLog?.Invoke(logRecord);
        }



        public void AddDataBaseStateMessage(string message)
        {
            if (!enableDataBaseStateLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordType.DataBaseState, message);
            dataBaseStateLog.Add(logRecord);
            if (dataBaseStateLog.Count > maxDataBaseStateRecords)
                dataBaseStateLog.RemoveAt(0);
            OnDataBaseStateLog?.Invoke(logRecord);
        }

        public void AddLogicalNodesEngineMessage(string message)
        {
            if (!enableLogicalNodesEngineLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordType.LogicalNodesEngine, message);
            logicalNodesEngineLog.Add(logRecord);
            if (logicalNodesEngineLog.Count > maxLogicalNodesEngineRecords)
                logicalNodesEngineLog.RemoveAt(0);
            OnLogicalNodesEngineLog?.Invoke(logRecord);
        }

        public void AddLogicalNodesMessage(string message)
        {
            if (!enableLogicalNodesLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordType.LogicalNodes, message);
            logicalNodesLog.Add(logRecord);
            if (logicalNodesLog.Count > maxLogicalNodesRecords)
                logicalNodesLog.RemoveAt(0);
            OnLogicalNodesLog?.Invoke(logRecord);
        }



        public void AddSerialControllerMessage(string message)
        {
            if (!enableSerialControllerLog)
                return;

            LogRecord logRecord = new LogRecord(LogRecordType.SerialController, message);
            serialControllerLog.Add(logRecord);
            if (serialControllerLog.Count > maxSerialControllerRecords)
                serialControllerLog.RemoveAt(0);
            OnSerialControllerLog?.Invoke(logRecord);
        }



        public List<LogRecord> GetAllLogs()
        {
            List<LogRecord> list = new List<LogRecord>();
            list.AddRange(gatewayStateLog);
            list.AddRange(gatewayMessagesLog);
            list.AddRange(gatewayRawMessagesLog);
            list.AddRange(logicalNodesEngineLog);
            list.AddRange(logicalNodesLog);
            list.AddRange(dataBaseStateLog);
            list.AddRange(serialControllerLog);
            return list.OrderBy(x => x.Date).ToList();
        }

        public void ClearAllLogs()
        {
            gatewayStateLog.Clear();
            gatewayMessagesLog.Clear();
            gatewayRawMessagesLog.Clear();
            logicalNodesEngineLog.Clear();
            logicalNodesLog.Clear();
            dataBaseStateLog.Clear();
            serialControllerLog.Clear();
        }
    }
}
