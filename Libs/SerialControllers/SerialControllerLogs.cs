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


        public void AddGatewayStateMessage(string message)
        {
            if (!enableGatewayStateLog)
                return;

            LogMessage logMessage = new LogMessage(LogMessageType.GatewayState, message);
            gatewayStateLog.Add(logMessage);
                OnGatewayStateLog?.Invoke(logMessage);
        }

        public void AddGatewayTxRxMessage(string message)
        {
            if (!enableGatewayTxRxLog)
                return;

            LogMessage logMessage = new LogMessage(LogMessageType.GatewayTxRx, message);
            gatewayTxRxLog.Add(logMessage);
                OnGatewayTxRxLog?.Invoke(logMessage);
        }

        public void AddGatewayRawTxRxMessage(string message)
        {
            if (!enableGatewayRawTxRxLog)
                return;

            LogMessage logMessage = new LogMessage(LogMessageType.GatewayRawTxRx, message);
            gatewayRawTxRxLog.Add(logMessage);
                OnGatewayRawTxRxLog?.Invoke(logMessage);
        }



        public void AddDataBaseStateMessage(string message)
        {
            if (!enableDataBaseStateLog)
                return;

            LogMessage logMessage = new LogMessage(LogMessageType.DataBaseState, message);
            dataBaseStateLog.Add(logMessage);
                OnDataBaseStateLog?.Invoke(logMessage);
        }

        public void AddLogicalNodesMessage(string message)
        {
            if (!enableLogicalNodesLog)
                return;

            LogMessage logMessage = new LogMessage(LogMessageType.LogicalNodes, message);
            logicalNodesLog.Add(logMessage);
                OnLogicalNodesLog?.Invoke(logMessage);
        }

        public void AddLogicalNodesEngineMessage(string message)
        {
            if (!enableLogicalNodesEngineLog)
                return;

            LogMessage logMessage = new LogMessage(LogMessageType.LogicalNodesEngine, message);
            logicalNodesEngineLog.Add(logMessage);
                OnLogicalNodesEngineLog?.Invoke(logMessage);
        }

        public void AddSerialControllerMessage(string message)
        {
            if (!enableSerialControllerLog)
                return;

            LogMessage logMessage = new LogMessage(LogMessageType.SerialController, message);
            serialControllerLog.Add(logMessage);
                OnSerialControllerLog?.Invoke(logMessage);
        }



        public List<LogMessage> GetAllLogs()
        {
            List<LogMessage>list=new List<LogMessage>();
            list.AddRange(gatewayStateLog);
            list.AddRange(gatewayTxRxLog);
            list.AddRange(gatewayRawTxRxLog);
            list.AddRange(logicalNodesEngineLog);
            list.AddRange(logicalNodesLog);
            list.AddRange(dataBaseStateLog);
            list.AddRange(serialControllerLog);
            return list.OrderBy(x=>x.Date).ToList();
        }
    }
}
