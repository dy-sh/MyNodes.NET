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
using MyNetSensors.LogicalNodes;
using MyNetSensors.Repositories.EF.SQLite;
using MyNetSensors.SerialControllers;
using MyNetSensors.WebController.Controllers;

namespace MyNetSensors.WebController.Code
{
    public static class SerialControllerConfigurator
    {
        private static bool serialControllerStarted;

        public static LogicalNodesDbContext logicalNodesDbContext;
        public static NodesDbContext nodesDbContext;
        public static NodesHistoryDbContext nodesHistoryDbContext;
        public static NodesMessagesDbContext nodesMessagesDbContext;
        public static NodesTasksDbContext nodesTasksDbContext;

        public static void Start(IConfigurationRoot Configuration)
        {
            if (serialControllerStarted) return;
            serialControllerStarted = true;


            //configure
            string portName = null;
            try
            {
                SerialController.logs.enableGatewayLog = Boolean.Parse(Configuration["Gateway:LogState"]);
                SerialController.logs.enableNodesLog = Boolean.Parse(Configuration["Gateway:LogMessages"]);
                SerialController.logs.enableLogicalNodesEngineLog = Boolean.Parse(Configuration["LogicalNodes:LogEngine"]);
                SerialController.logs.enableLogicalNodesLog = Boolean.Parse(Configuration["LogicalNodes:LogNodes"]);
                SerialController.logs.enableDataBaseLog = Boolean.Parse(Configuration["DataBase:LogState"]);


                SerialController.enableAutoAssignId = Boolean.Parse(Configuration["Gateway:EnableAutoAssignId"]);

                SerialController.nodesTasksEnabled = Boolean.Parse(Configuration["NodesTasks:Enable"]);
                SerialController.nodesTasksUpdateInterval = Int32.Parse(Configuration["NodesTasks:UpdateInterval"]);
                SerialController.logicalNodesEnabled = Boolean.Parse(Configuration["LogicalNodes:Enable"]);
                SerialController.logicalNodesUpdateInterval = Int32.Parse(Configuration["LogicalNodes:UpdateInterval"]);
                SerialController.dataBaseEnabled = Boolean.Parse(Configuration["DataBase:Enable"]);
                SerialController.dataBadeUseMSSQL = Boolean.Parse(Configuration["DataBase:UseMSSQL"]);
                SerialController.dataBaseWriteInterval = Int32.Parse(Configuration["DataBase:WriteInterval"]);
                SerialController.writeNodesMessagesToDataBase = Boolean.Parse(Configuration["Gateway:WriteNodesMessagesToDataBase"]);

                if (SerialController.dataBadeUseMSSQL)
                    SerialController.dataBaseConnectionString = Configuration["DataBase:MSSQLConnectionString"];

                //SerialController.softNodesEnabled = Boolean.Parse(Configuration["SoftNodes:Enable"]);
                //SerialController.softNodesPort = Int32.Parse(Configuration["SoftNodes:Port"]);
                //SerialController.softNodesLogMessages = Boolean.Parse(Configuration["SoftNodes:LogMessages"]);
                //SerialController.softNodesLogState = Boolean.Parse(Configuration["SoftNodes:LogState"]);

                portName = Configuration["Gateway:SerialPort"];
            }
            catch
            {
                Log(new LogRecord(LogRecordOwner.SerialController,LogRecordType.Error,  "ERROR: Bad configuration in appsettings.json file."),ConsoleColor.Red);
                throw new Exception("Bad configuration in appsettings.json file.");
            }

            if (!SerialController.dataBadeUseMSSQL)
            {
                //todo temporary
                SerialController.dataBaseConnectionString = Configuration["DataBase:MSSQLConnectionString"];

                SerialController.gatewayDb = new GatewayRepositoryEF(nodesDbContext);
                SerialController.historyDb = new NodesHistoryRepositoryEf(nodesHistoryDbContext);
                SerialController.messagesDb = new NodesMessagesRepositoryEF(nodesMessagesDbContext);
                SerialController.nodesTasksDb = new NodesTasksRepositoryEF(nodesTasksDbContext);
                SerialController.logicalNodesDb = new LogicalNodesRepositoryEF(logicalNodesDbContext);
            }

            if (portName != null)
            {
                SerialController.logs.OnGatewayLogInfo += (logMessage) => { Log(logMessage, ConsoleColor.Green); };
                SerialController.logs.OnGatewayLogError += (logMessage) => { Log(logMessage, ConsoleColor.Red); };
                SerialController.logs.OnNodeLogInfo += (logMessage) => { Log(logMessage, ConsoleColor.DarkGreen); };
                SerialController.logs.OnNodeLogError += (logMessage) => { Log(logMessage, ConsoleColor.Red); };
                SerialController.logs.OnDataBaseLogInfo += (logMessage) => { Log(logMessage, ConsoleColor.Gray); };
                SerialController.logs.OnDataBaseLogError += (logMessage) => { Log(logMessage, ConsoleColor.Red); };
                SerialController.logs.OnLogicalNodesEngineLogInfo += (logMessage) => { Log(logMessage, ConsoleColor.Cyan); };
                SerialController.logs.OnLogicalNodesEngineLogError += (logMessage) => { Log(logMessage, ConsoleColor.Red); };
                SerialController.logs.OnLogicalNodeLogInfo += (logMessage) => { Log(logMessage, ConsoleColor.DarkCyan); };
                SerialController.logs.OnLogicalNodeLogError += (logMessage) => { Log(logMessage, ConsoleColor.Red); };
                SerialController.logs.OnSerialControllerLogInfo += (logMessage) => { Log(logMessage, ConsoleColor.White); };
                SerialController.logs.OnSerialControllerLogError += (logMessage) => { Log(logMessage, ConsoleColor.Red); };



                SerialController.Start(portName);
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
