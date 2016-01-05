using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.SerialControllers
{
    public delegate void LogMessageEventHandler(LogMessage message);
    public class SerialControllerLogs
    {
        public List<LogMessage> gatewayTxRxLog = new List<LogMessage>();
        public List<LogMessage> gatewayRawTxRxLog = new List<LogMessage>();
        public List<LogMessage> gatewayStateLog = new List<LogMessage>();
        public List<LogMessage> dataBaseStateLog = new List<LogMessage>();
        public List<LogMessage> logicalNodesLog = new List<LogMessage>();
        public List<LogMessage> logicalNodesEngineLog = new List<LogMessage>();
        public List<LogMessage> serialControllerLog = new List<LogMessage>();

        public event LogMessageEventHandler OnGatewayTxRxLog;
        public event LogMessageEventHandler OnGatewayRawTxRxLog;
        public event LogMessageEventHandler OnGatewayStateLog;
        public event LogMessageEventHandler OnDataBaseStateLog;
        public event LogMessageEventHandler OnLogicalNodesLog;
        public event LogMessageEventHandler OnLogicalNodesEngineLog;
        public event LogMessageEventHandler OnSerialControllerLog;

        public bool enableGatewayStateLog = true;
        public bool enableGatewayTxRxLog = true;
        public bool enableGatewayRawTxRxLog = false;
        public bool enableDataBaseStateLog = true;
        public bool enableLogicalNodesLog = true;
        public bool enableLogicalNodesEngineLog = true;
        public bool enableSerialControllerLog = true;

        public int maxGatewayStateRecords = 1000;
        public int maxGatewayTxRxRecords = 1000;
        public int maxGatewayRawTxRxRecords = 1000;
        public int maxDataBaseStateRecords = 1000;
        public int maxLogicalNodesRecords = 1000;
        public int maxLogicalNodesEngineRecords = 1000;
        public int maxSerialControllerRecords = 1000;


        public void AddGatewayStateMessage(string message)
        {
            if (!enableGatewayStateLog)
                return;

            LogMessage logMessage = new LogMessage(LogMessageType.GatewayState, message);
            gatewayStateLog.Add(logMessage);
            if(gatewayStateLog.Count> maxGatewayStateRecords)
                gatewayStateLog.RemoveAt(0);
            OnGatewayStateLog?.Invoke(logMessage);
        }

        public void AddGatewayTxRxMessage(string message)
        {
            if (!enableGatewayTxRxLog)
                return;

            LogMessage logMessage = new LogMessage(LogMessageType.GatewayTxRx, message);
            gatewayTxRxLog.Add(logMessage);
            if (gatewayTxRxLog.Count > maxGatewayTxRxRecords)
                gatewayTxRxLog.RemoveAt(0);
            OnGatewayTxRxLog?.Invoke(logMessage);
        }

        public void AddGatewayRawTxRxMessage(string message)
        {
            if (!enableGatewayRawTxRxLog)
                return;

            LogMessage logMessage = new LogMessage(LogMessageType.GatewayRawTxRx, message);
            gatewayRawTxRxLog.Add(logMessage);
            if (gatewayRawTxRxLog.Count > maxGatewayRawTxRxRecords)
                gatewayRawTxRxLog.RemoveAt(0);
            OnGatewayRawTxRxLog?.Invoke(logMessage);
        }



        public void AddDataBaseStateMessage(string message)
        {
            if (!enableDataBaseStateLog)
                return;

            LogMessage logMessage = new LogMessage(LogMessageType.DataBaseState, message);
            dataBaseStateLog.Add(logMessage);
            if (dataBaseStateLog.Count > maxDataBaseStateRecords)
                dataBaseStateLog.RemoveAt(0);
            OnDataBaseStateLog?.Invoke(logMessage);
        }

        public void AddLogicalNodesEngineMessage(string message)
        {
            if (!enableLogicalNodesEngineLog)
                return;

            LogMessage logMessage = new LogMessage(LogMessageType.LogicalNodesEngine, message);
            logicalNodesEngineLog.Add(logMessage);
            if (logicalNodesEngineLog.Count > maxLogicalNodesEngineRecords)
                logicalNodesEngineLog.RemoveAt(0);
            OnLogicalNodesEngineLog?.Invoke(logMessage);
        }

        public void AddLogicalNodesMessage(string message)
        {
            if (!enableLogicalNodesLog)
                return;

            LogMessage logMessage = new LogMessage(LogMessageType.LogicalNodes, message);
            logicalNodesLog.Add(logMessage);
            if (logicalNodesLog.Count > maxLogicalNodesRecords)
                logicalNodesLog.RemoveAt(0);
            OnLogicalNodesLog?.Invoke(logMessage);
        }



        public void AddSerialControllerMessage(string message)
        {
            if (!enableSerialControllerLog)
                return;

            LogMessage logMessage = new LogMessage(LogMessageType.SerialController, message);
            serialControllerLog.Add(logMessage);
            if (serialControllerLog.Count > maxSerialControllerRecords)
                serialControllerLog.RemoveAt(0);
            OnSerialControllerLog?.Invoke(logMessage);
        }



        public List<LogMessage> GetAllLogs()
        {
            List<LogMessage> list = new List<LogMessage>();
            list.AddRange(gatewayStateLog);
            list.AddRange(gatewayTxRxLog);
            list.AddRange(gatewayRawTxRxLog);
            list.AddRange(logicalNodesEngineLog);
            list.AddRange(logicalNodesLog);
            list.AddRange(dataBaseStateLog);
            list.AddRange(serialControllerLog);
            return list.OrderBy(x => x.Date).ToList();
        }

        public void ClearAllLogs()
        {
            gatewayStateLog.Clear();
            gatewayTxRxLog.Clear();
            gatewayRawTxRxLog.Clear();
            logicalNodesEngineLog.Clear();
            logicalNodesLog.Clear();
            dataBaseStateLog.Clear();
            serialControllerLog.Clear();
        }
    }
}
