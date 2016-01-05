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

        public static NodesDbContext nodesDbContext;

        public static void Start(IConfigurationRoot Configuration)
        {
            if (serialControllerStarted) return;
            serialControllerStarted = true;


            //configure
            string portName = null;
            try
            {
                SerialController.logs.enableGatewayStateLog = Boolean.Parse(Configuration["Gateway:LogState"]);
                SerialController.logs.enableGatewayMessagesLog = Boolean.Parse(Configuration["Gateway:LogMessages"]);
                SerialController.logs.enableGatewayRawMessagesLog = Boolean.Parse(Configuration["Gateway:LogRawMessages"]);
                SerialController.logs.enableLogicalNodesEngineLog = Boolean.Parse(Configuration["LogicalNodes:LogEngine"]);
                SerialController.logs.enableLogicalNodesLog = Boolean.Parse(Configuration["LogicalNodes:LogNodes"]);
                SerialController.logs.enableDataBaseStateLog = Boolean.Parse(Configuration["DataBase:LogState"]);


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
                else
                    SerialController.dataBaseConnectionString = Configuration["DataBase:SqliteConnectionString"];

                //SerialController.softNodesEnabled = Boolean.Parse(Configuration["SoftNodes:Enable"]);
                //SerialController.softNodesPort = Int32.Parse(Configuration["SoftNodes:Port"]);
                //SerialController.softNodesLogMessages = Boolean.Parse(Configuration["SoftNodes:LogMessages"]);
                //SerialController.softNodesLogState = Boolean.Parse(Configuration["SoftNodes:LogState"]);

                portName = Configuration["Gateway:SerialPort"];
            }
            catch
            {
                Log(new LogRecord(LogRecordType.SerialController, "ERROR: Bad configuration in appsettings.json file."),ConsoleColor.Red);
                throw new Exception("Bad configuration in appsettings.json file.");
            }

            if (!SerialController.dataBadeUseMSSQL)
            {
                //todo temporary
                SerialController.dataBaseConnectionString = Configuration["DataBase:MSSQLConnectionString"];

                SerialController.gatewayDb = new GatewayRepositoryEF(nodesDbContext);
                SerialController.historyDb = new NodesHistoryRepositoryEf(nodesDbContext);
                SerialController.messagesDb = new NodesMessagesRepositoryEF(nodesDbContext);
                SerialController.nodesTasksDb = new NodesTasksRepositoryEF(nodesDbContext);
                SerialController.logicalNodesDb = new LogicalNodesRepositoryEF(nodesDbContext);
            }

            if (portName != null)
            {
                SerialController.logs.OnGatewayStateLog += (logMessage) => { Log(logMessage, ConsoleColor.Green); };
                SerialController.logs.OnGatewayMessagesLog += (logMessage) => { Log(logMessage, ConsoleColor.DarkGreen); };
                // SerialController.logs.OnGatewayRawMessagesLog += (logMessage) => { Log(logMessage, ConsoleColor.DarkGreen); };
                SerialController.logs.OnDataBaseStateLog += (logMessage) => { Log(logMessage, ConsoleColor.Gray); };
                SerialController.logs.OnLogicalNodesEngineLog += (logMessage) => { Log(logMessage, ConsoleColor.Cyan); };
                SerialController.logs.OnLogicalNodesLog += (logMessage) => { Log(logMessage, ConsoleColor.DarkCyan); };
                SerialController.logs.OnSerialControllerLog += (logMessage) => { Log(logMessage, ConsoleColor.White); };



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
