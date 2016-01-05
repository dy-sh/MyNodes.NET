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

        public bool enableGatewayStateEvent = true;
        public bool enableGatewayTxRxEvent = true;
        public bool enableGatewayRawTxRxEvent = false;
        public bool enableDataBaseStateEvent = true;
        public bool enableLogicalNodesEvent = true;
        public bool enableLogicalNodesEngineEvent = true;
        public bool enableSerialControllerEvent = true;


        public void AddGatewayStateMessage(string message)
        {
            LogMessage logMessage = new LogMessage(LogMessageType.GatewayState, message);
            gatewayStateLog.Add(logMessage);
            if (enableGatewayStateEvent)
                OnGatewayStateLog?.Invoke(logMessage);
        }

        public void AddGatewayTxRxMessage(string message)
        {
            LogMessage logMessage = new LogMessage(LogMessageType.GatewayTxRx, message);
            gatewayTxRxLog.Add(logMessage);
            if (enableGatewayTxRxEvent)
                OnGatewayTxRxLog?.Invoke(logMessage);
        }

        public void AddGatewayRawTxRxMessage(string message)
        {
            LogMessage logMessage = new LogMessage(LogMessageType.GatewayRawTxRx, message);
            gatewayRawTxRxLog.Add(logMessage);
            if (enableGatewayRawTxRxEvent)
                OnGatewayRawTxRxLog?.Invoke(logMessage);
        }



        public void AddDataBaseStateMessage(string message)
        {
            LogMessage logMessage = new LogMessage(LogMessageType.DataBaseState, message);
            dataBaseStateLog.Add(logMessage);
            if (enableDataBaseStateEvent)
                OnDataBaseStateLog?.Invoke(logMessage);
        }

        public void AddLogicalNodesMessage(string message)
        {
            LogMessage logMessage = new LogMessage(LogMessageType.LogicalNodes, message);
            logicalNodesLog.Add(logMessage);
            if (enableLogicalNodesEvent)
                OnLogicalNodesLog?.Invoke(logMessage);
        }

        public void AddLogicalNodesEngineMessage(string message)
        {
            LogMessage logMessage = new LogMessage(LogMessageType.LogicalNodesEngine, message);
            logicalNodesEngineLog.Add(logMessage);
            if (enableLogicalNodesEngineEvent)
                OnLogicalNodesEngineLog?.Invoke(logMessage);
        }

        public void AddSerialControllerMessage(string message)
        {
            LogMessage logMessage = new LogMessage(LogMessageType.SerialController, message);
            serialControllerLog.Add(logMessage);
            if (enableSerialControllerEvent)
                OnSerialControllerLog?.Invoke(logMessage);
        }
    }
}
