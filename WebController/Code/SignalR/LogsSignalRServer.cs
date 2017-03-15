/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using MyNodes.Gateways.MySensors;


namespace MyNodes.WebController.Code
{
    public static class LogsSignalRServer
    {
        private static IHubContext hub;

        public static void Start(IConnectionManager connectionManager)
        {
            hub = connectionManager.GetHubContext<LogsHub>();

            SystemController.OnGatewayConnected += OnGatewayConnected;
            SystemController.OnGatewayDisconnected += OnGatewayDisconnected;

            SystemController.logs.OnGatewayLogInfo += OnLogRecord;
            SystemController.logs.OnGatewayLogError += OnLogRecord;
            SystemController.logs.OnGatewayMessageLog += OnLogRecord;
            SystemController.logs.OnGatewayDecodedMessageLog += OnLogRecord;
            SystemController.logs.OnDataBaseLogInfo += OnLogRecord;
            SystemController.logs.OnDataBaseLogError += OnLogRecord;
            SystemController.logs.OnNodesEngineLogInfo += OnLogRecord;
            SystemController.logs.OnNodesEngineLogError += OnLogRecord;
            SystemController.logs.OnNodeLogInfo += OnLogRecord;
            SystemController.logs.OnNodeLogError += OnLogRecord;
            SystemController.logs.OnSystemLogInfo += OnLogRecord;
            SystemController.logs.OnSystemLogError += OnLogRecord;
        }




        private static void OnLogRecord(LogRecord record)
        {
            hub.Clients.Group(record.Source.ToString()).OnLogRecord(record);

            if (record.Type == LogRecordType.Error)
                hub.Clients.Group("Errors").OnLogRecord(record);

            hub.Clients.Group("All").OnLogRecord(record);
        }

        private static void OnLogRecord(Message message)
        {
            hub.Clients.Group(LogRecordSource.GatewayDecodedMessages.ToString())
                .OnLogRecord(new LogRecord(
                    LogRecordSource.GatewayDecodedMessages, 
                    LogRecordType.Info, 
                    message.ToString()));
        }


        private static void OnGatewayConnected()
        {
            hub.Clients.All.OnGatewayConnected();
        }

        private static void OnGatewayDisconnected()
        {
            hub.Clients.All.OnGatewayDisconnected();
        }
    }
}
