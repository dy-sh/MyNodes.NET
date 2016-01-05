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
                SerialController.logs.enableGatewayStateEvent = Boolean.Parse(Configuration["Gateway:DebugState"]);
                SerialController.logs.enableGatewayTxRxEvent = Boolean.Parse(Configuration["Gateway:DebugTxRx"]);
                SerialController.logs.enableGatewayRawTxRxEvent = Boolean.Parse(Configuration["Gateway:DebugRawTxRx"]);
                SerialController.logs.enableLogicalNodesEngineEvent = Boolean.Parse(Configuration["LogicalNodes:DebugEngine"]);
                SerialController.logs.enableLogicalNodesEvent = Boolean.Parse(Configuration["LogicalNodes:DebugNodes"]);
                SerialController.logs.enableDataBaseStateEvent = Boolean.Parse(Configuration["DataBase:DebugState"]);


                SerialController.enableAutoAssignId = Boolean.Parse(Configuration["Gateway:EnableAutoAssignId"]);

                SerialController.nodesTasksEnabled = Boolean.Parse(Configuration["NodesTasks:Enable"]);
                SerialController.nodesTasksUpdateInterval = Int32.Parse(Configuration["NodesTasks:UpdateInterval"]);
                //SerialController.softNodesEnabled = Boolean.Parse(Configuration["SoftNodes:Enable"]);
                //SerialController.softNodesPort = Int32.Parse(Configuration["SoftNodes:Port"]);
                //SerialController.softNodesDebugTxRx = Boolean.Parse(Configuration["SoftNodes:DebugTxRx"]);
                //SerialController.softNodesDebugState = Boolean.Parse(Configuration["SoftNodes:DebugState"]);
                SerialController.logicalNodesEnabled = Boolean.Parse(Configuration["LogicalNodes:Enable"]);
                SerialController.logicalNodesUpdateInterval = Int32.Parse(Configuration["LogicalNodes:UpdateInterval"]);
                SerialController.dataBaseEnabled = Boolean.Parse(Configuration["DataBase:Enable"]);
                SerialController.dataBadeUseMSSQL = Boolean.Parse(Configuration["DataBase:UseMSSQL"]);
                SerialController.dataBaseWriteInterval = Int32.Parse(Configuration["DataBase:WriteInterval"]);

                if (SerialController.dataBadeUseMSSQL)
                    SerialController.dataBaseConnectionString = Configuration["DataBase:MSSQLConnectionString"];
                else
                    SerialController.dataBaseConnectionString = Configuration["DataBase:SqliteConnectionString"];

                portName = Configuration["Gateway:SerialPort"];
            }
            catch
            {
                Log(new LogMessage(LogMessageType.SerialController, "ERROR: Bad configuration in appsettings.json file."),ConsoleColor.Red);
                throw new Exception("Bad configuration in appsettings.json file.");
            }

            if (!SerialController.dataBadeUseMSSQL)
            {
                //todo temporary
                SerialController.dataBaseConnectionString = Configuration["DataBase:MSSQLConnectionString"];

                SerialController.gatewayDb = new GatewayRepositoryEF(nodesDbContext);
                SerialController.historyDb = new NodesHistoryRepositoryEf(nodesDbContext);
                SerialController.nodesTasksDb = new NodesTasksRepositoryEF(nodesDbContext);
                SerialController.logicalNodesRepository = new LogicalNodesRepositoryEF(nodesDbContext);
            }

            if (portName != null)
            {
                SerialController.logs.OnGatewayStateLog += (logMessage) => { Log(logMessage, ConsoleColor.Green); };
                SerialController.logs.OnGatewayTxRxLog += (logMessage) => { Log(logMessage, ConsoleColor.DarkGreen); };
                SerialController.logs.OnGatewayRawTxRxLog += (logMessage) => { Log(logMessage, ConsoleColor.DarkGreen); };
                SerialController.logs.OnDataBaseStateLog += (logMessage) => { Log(logMessage, ConsoleColor.Gray); };
                SerialController.logs.OnLogicalNodesEngineLog += (logMessage) => { Log(logMessage, ConsoleColor.Cyan); };
                SerialController.logs.OnLogicalNodesLog += (logMessage) => { Log(logMessage, ConsoleColor.DarkCyan); };
                SerialController.logs.OnSerialControllerLog += (logMessage) => { Log(logMessage, ConsoleColor.White); };



                SerialController.Start(portName);
            }
        }

        public static void Log(LogMessage message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message.ToStringWithType());
            Console.ForegroundColor = ConsoleColor.Gray;
        }


    }
}
