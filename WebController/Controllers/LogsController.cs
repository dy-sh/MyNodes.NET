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

        public IActionResult GatewayRawMessages()
        {
            ViewBag.LogType = "GatewayRawMessages";
            ViewBag.PageName = "Gateway Raw Messages";
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
                List<LogMessage> list = SerialController.logs.GetAllLogs();
                return list.Select(logMessage => logMessage.ToStringWithType()).ToList();
            }
            else if (logType == "Controller")
            {
                List<LogMessage> list = SerialController.logs.serialControllerLog;
                return list.Select(logMessage => logMessage.ToString()).ToList();
            }
            else if (logType == "GatewayState")
            {
                List<LogMessage> list = SerialController.logs.gatewayStateLog;
                return list.Select(logMessage => logMessage.ToString()).ToList();
            }
            else if (logType == "GatewayMessages")
            {
                List<LogMessage> list = SerialController.logs.gatewayTxRxLog;
                return list.Select(logMessage => logMessage.Message).ToList();
            }
            else if (logType == "GatewayRawMessages")
            {
                List<LogMessage> list = SerialController.logs.gatewayRawTxRxLog;
                return list.Select(logMessage => logMessage.ToString()).ToList();
            }
            else if (logType == "LogicalNodes")
            {
                List<LogMessage> list = SerialController.logs.logicalNodesLog;
                return list.Select(logMessage => logMessage.ToString()).ToList();
            }
            else if (logType == "LogicalNodesEngine")
            {
                List<LogMessage> list = SerialController.logs.logicalNodesEngineLog;
                return list.Select(logMessage => logMessage.ToString()).ToList();
            }
            else if (logType == "DataBase")
            {
                List<LogMessage> list = SerialController.logs.dataBaseStateLog;
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
                SerialController.logs.gatewayStateLog.Clear();
            }
            else if (logType == "GatewayMessages")
            {
                SerialController.logs.gatewayTxRxLog.Clear();
            }
            else if (logType == "GatewayRawMessages")
            {
                SerialController.logs.gatewayRawTxRxLog.Clear();
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
                SerialController.logs.dataBaseStateLog.Clear();
            }

            return true;
        }
        

    }
}
