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
using MyNetSensors.SerialControllers;
using MyNetSensors.WebController.Controllers;

namespace MyNetSensors.WebController.Code
{
    public static class SerialControllerConfigurator
    {
        private static bool serialControllerStarted;


        public static void Start(IConfigurationRoot Configuration)
        {
            if (serialControllerStarted) return;
            serialControllerStarted = true;


            //configure
            string portName = null;
            try
            {
                SerialController.serialPortDebugState = Boolean.Parse(Configuration["SerialPort:DebugState"]);
                SerialController.serialPortDebugTxRx = Boolean.Parse(Configuration["SerialPort:DebugTxRx"]);
                SerialController.enableAutoAssignId = Boolean.Parse(Configuration["Gateway:EnableAutoAssignId"]);
                SerialController.gatewayDebugState = Boolean.Parse(Configuration["Gateway:DebugState"]);
                SerialController.gatewayDebugTxRx = Boolean.Parse(Configuration["Gateway:DebugTxRx"]);

                SerialController.dataBaseEnabled = Boolean.Parse(Configuration["DataBase:Enable"]);
                SerialController.dataBaseConnectionString = Configuration["DataBase:ConnectionString"];
                SerialController.dataBaseWriteInterval = Int32.Parse(Configuration["DataBase:WriteInterval"]);
                SerialController.dataBaseDebugState = Boolean.Parse(Configuration["DataBase:DebugState"]);
                SerialController.dataBaseWriteTxRxMessages = Boolean.Parse(Configuration["DataBase:WriteTxRxMessages"]);
                SerialController.sensorsTasksEnabled = Boolean.Parse(Configuration["SensorsTasks:Enable"]);
                SerialController.sensorsTasksUpdateInterval = Int32.Parse(Configuration["SensorsTasks:UpdateInterval"]);
                SerialController.softNodesEnabled = Boolean.Parse(Configuration["SoftNodes:Enable"]);
                SerialController.softNodesPort = Int32.Parse(Configuration["SoftNodes:Port"]);
                SerialController.softNodesDebugTxRx = Boolean.Parse(Configuration["SoftNodes:DebugTxRx"]);
                SerialController.softNodesDebugState = Boolean.Parse(Configuration["SoftNodes:DebugState"]);
                SerialController.logicalNodesEnabled = Boolean.Parse(Configuration["LogicalNodes:Enable"]);
                SerialController.logicalNodesUpdateInterval = Int32.Parse(Configuration["LogicalNodes:UpdateInterval"]);
                SerialController.logicalNodesDebugEngine = Boolean.Parse(Configuration["LogicalNodes:DebugEngine"]);
                SerialController.logicalNodesDebugNodes = Boolean.Parse(Configuration["LogicalNodes:DebugNodes"]);


                portName = Configuration["SerialPort:Name"];
            }
            catch
            {
                Log("ERROR: Bad configuration in appsettings.json file.");
                throw new Exception("Bad configuration in appsettings.json file.");
            }

            if (portName != null)
            {
                SerialController.OnDebugStateMessage += Log;
                SerialController.OnDebugTxRxMessage += Log;

                SerialController.Start(portName);
            }
        }

        public static void Log(string message)
        {
            Console.WriteLine(message);
            // logger.LogInformation(message);
        }


    }
}
