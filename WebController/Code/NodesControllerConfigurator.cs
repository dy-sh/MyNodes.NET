/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting.Internal;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyNetSensors.Gateways;
using MyNetSensors.Repositories.EF.SQLite;
using MyNetSensors.WebController.Controllers;

namespace MyNetSensors.WebController.Code
{
    public static class NodesControllerConfigurator
    {
        private static bool nodesControllerStarted;

        public static NodesDbContext nodesDbContext;
        public static NodesStatesHistoryDbContext nodesStatesHistoryDbContext;
        public static MySensorsNodesDbContext mySensorsNodesDbContext;
        public static MySensorsMessagesDbContext mySensorsMessagesDbContext;
        public static UITimerNodesDbContext uiTimerNodesDbContext;

        public static void Start(IConfigurationRoot Configuration)
        {
            if (nodesControllerStarted) return;
            nodesControllerStarted = true;


            //configure
            string portName = null;
            try
            {
                NodesController.serialGatewayEnabled = Boolean.Parse(Configuration["SerialGateway:Enable"]);

                NodesController.logs.enableGatewayLog = Boolean.Parse(Configuration["SerialGateway:LogState"]);
                NodesController.logs.enableHardwareNodesLog = Boolean.Parse(Configuration["SerialGateway:LogMessages"]);
                NodesController.logs.enableNodesEngineLog = Boolean.Parse(Configuration["NodesEngine:LogEngine"]);
                NodesController.logs.enableNodesLog = Boolean.Parse(Configuration["NodesEngine:LogNodes"]);
                NodesController.logs.enableDataBaseLog = Boolean.Parse(Configuration["DataBase:LogState"]);


                NodesController.enableAutoAssignId = Boolean.Parse(Configuration["SerialGateway:EnableAutoAssignId"]);

                NodesController.nodesEngineEnabled = Boolean.Parse(Configuration["NodesEngine:Enable"]);
                NodesController.nodesEngineUpdateInterval = Int32.Parse(Configuration["NodesEngine:UpdateInterval"]);
                NodesController.dataBaseEnabled = Boolean.Parse(Configuration["DataBase:Enable"]);
                NodesController.dataBadeUseMSSQL = Boolean.Parse(Configuration["DataBase:UseMSSQL"]);
                NodesController.dataBaseWriteInterval = Int32.Parse(Configuration["DataBase:WriteInterval"]);
                NodesController.writeNodesMessagesToDataBase = Boolean.Parse(Configuration["SerialGateway:WriteNodesMessagesToDataBase"]);

                if (NodesController.dataBadeUseMSSQL)
                    NodesController.dataBaseConnectionString = Configuration["DataBase:MSSQLConnectionString"];

                //NodesController.softNodesEnabled = Boolean.Parse(Configuration["SoftNodes:Enable"]);
                //NodesController.softNodesPort = Int32.Parse(Configuration["SoftNodes:Port"]);
                //NodesController.softNodesLogMessages = Boolean.Parse(Configuration["SoftNodes:LogMessages"]);
                //NodesController.softNodesLogState = Boolean.Parse(Configuration["SoftNodes:LogState"]);

                portName = Configuration["SerialGateway:SerialPort"];
            }
            catch
            {
                Log(new LogRecord(LogRecordOwner.NodesController, LogRecordType.Error, "ERROR: Bad configuration in appsettings.json file."), ConsoleColor.Red);
                throw new Exception("Bad configuration in appsettings.json file.");
            }

            if (!NodesController.dataBadeUseMSSQL)
            {
                //todo temporary
                NodesController.dataBaseConnectionString = Configuration["DataBase:MSSQLConnectionString"];

                NodesController.mySensorsDb = new MySensorsRepositoryEf(mySensorsNodesDbContext);
                NodesController.messagesDb = new MySensorsMessagesRepositoryEf(mySensorsMessagesDbContext);
                NodesController.uiTimerNodesDb = new UITimerNodesRepositoryEf(uiTimerNodesDbContext);
                NodesController.nodesDb = new NodesRepositoryEf(nodesDbContext);
                NodesController.nodesStatesDb = new NodesStatesRepositoryEf(nodesStatesHistoryDbContext);
            }

            if (portName != null)
            {
                NodesController.logs.OnGatewayLogInfo += (logMessage) => { Log(logMessage, ConsoleColor.Green); };
                NodesController.logs.OnGatewayLogError += (logMessage) => { Log(logMessage, ConsoleColor.Red); };
                NodesController.logs.OnHardwareNodeLogInfo += (logMessage) => { Log(logMessage, ConsoleColor.DarkGreen); };
                NodesController.logs.OnHardwareNodeLogError += (logMessage) => { Log(logMessage, ConsoleColor.Red); };
                NodesController.logs.OnDataBaseLogInfo += (logMessage) => { Log(logMessage, ConsoleColor.Gray); };
                NodesController.logs.OnDataBaseLogError += (logMessage) => { Log(logMessage, ConsoleColor.Red); };
                NodesController.logs.OnNodesEngineLogInfo += (logMessage) => { Log(logMessage, ConsoleColor.Cyan); };
                NodesController.logs.OnNodesEngineLogError += (logMessage) => { Log(logMessage, ConsoleColor.Red); };
                NodesController.logs.OnNodeLogInfo += (logMessage) => { Log(logMessage, ConsoleColor.DarkCyan); };
                NodesController.logs.OnNodeLogError += (logMessage) => { Log(logMessage, ConsoleColor.Red); };
                NodesController.logs.OnNodesControllerLogInfo += (logMessage) => { Log(logMessage, ConsoleColor.White); };
                NodesController.logs.OnNodesControllerLogError += (logMessage) => { Log(logMessage, ConsoleColor.Red); };



                NodesController.Start(portName);
            }
        }

        public static void Log(LogRecord record, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(record.ToStringWithType());
            Console.ForegroundColor = ConsoleColor.Gray;
        }


    }
}
