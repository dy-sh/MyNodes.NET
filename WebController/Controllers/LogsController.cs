using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MyNetSensors.SerialControllers;

namespace MyNetSensors.WebController.Controllers
{
    public class LogsController:Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("All");
        }

        public IActionResult All()
        {
            ViewBag.LogType = "All";
            ViewBag.PageName = "All";
            return View("Logs");
        }

        public IActionResult Controller()
        {
            ViewBag.LogType = "Controller";
            ViewBag.PageName = "Controller";
            return View("Logs");
        }

        public IActionResult GatewayState()
        {
            ViewBag.LogType = "GatewayState";
            ViewBag.PageName = "Gateway State";
            return View("Logs");
        }

        public IActionResult GatewayMessages()
        {
            ViewBag.LogType = "GatewayMessages";
            ViewBag.PageName = "Gateway Messages";
            return View("Logs");
        }


        public IActionResult LogicalNodes()
        {
            ViewBag.LogType = "LogicalNodes";
            ViewBag.PageName = "Logical Nodes";
            return View("Logs");
        }

        public IActionResult LogicalNodesEngine()
        {
            ViewBag.LogType = "LogicalNodesEngine";
            ViewBag.PageName = "Logical Nodes Engine";
            return View("Logs");
        }

        public IActionResult DataBase()
        {
            ViewBag.LogType = "DataBase";
            ViewBag.PageName = "DataBase";
            return View("Logs");
        }

        public List<string> GetLogs(string logType)
        {
            if (logType == "All")
            {
                List<LogRecord> list = SerialController.logs.GetAllLogs();
                return list.Select(logMessage => logMessage.ToStringWithType()).ToList();
            }
            else if (logType == "Controller")
            {
                List<LogRecord> list = SerialController.logs.serialControllerLog;
                return list.Select(logMessage => logMessage.ToString()).ToList();
            }
            else if (logType == "GatewayState")
            {
                List<LogRecord> list = SerialController.logs.gatewayLog;
                return list.Select(logMessage => logMessage.ToString()).ToList();
            }
            else if (logType == "GatewayMessages")
            {
                List<LogRecord> list = SerialController.logs.nodesLog;
                return list.Select(logMessage => logMessage.Message).ToList();
            }
            else if (logType == "LogicalNodes")
            {
                List<LogRecord> list = SerialController.logs.logicalNodesLog;
                return list.Select(logMessage => logMessage.ToString()).ToList();
            }
            else if (logType == "LogicalNodesEngine")
            {
                List<LogRecord> list = SerialController.logs.logicalNodesEngineLog;
                return list.Select(logMessage => logMessage.ToString()).ToList();
            }
            else if (logType == "DataBase")
            {
                List<LogRecord> list = SerialController.logs.dataBaseLog;
                return list.Select(logMessage => logMessage.ToString()).ToList();
            }
            return null;
        }

        public bool ClearLogs(string logType)
        {
            if (logType == "All")
            {
                SerialController.logs.ClearAllLogs();
            }
            else if (logType == "Controller")
            {
                SerialController.logs.serialControllerLog.Clear();
            }
            else if (logType == "GatewayState")
            {
                SerialController.logs.gatewayLog.Clear();
            }
            else if (logType == "GatewayMessages")
            {
                SerialController.logs.nodesLog.Clear();
            }
            else if (logType == "LogicalNodes")
            {
                SerialController.logs.logicalNodesLog.Clear();
            }
            else if (logType == "LogicalNodesEngine")
            {
                SerialController.logs.logicalNodesEngineLog.Clear();
            }
            else if (logType == "DataBase")
            {
                SerialController.logs.dataBaseLog.Clear();
            }

            return true;
        }
        

    }
}
