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

        public IActionResult AllErrors()
        {
            ViewBag.LogType = "AllErrors";
            ViewBag.PageName = "Errors";
            return View("Logs");
        }

        public IActionResult Controller()
        {
            ViewBag.LogType = "Controller";
            ViewBag.PageName = "Controller";
            return View("Logs");
        }

        public IActionResult Gateway()
        {
            ViewBag.LogType = "Gateway";
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

        public List<LogRecord> GetLogs(string logType)
        {
            if (logType == "All")
            {
                return SerialController.logs.GetAllLogs();
            }
            else if(logType == "AllErrors")
            {
                return SerialController.logs.GetAllErrorLogs();
            }
            else if (logType == "Controller")
            {
                return SerialController.logs.serialControllerLog;
            }
            else if (logType == "Gateway")
            {
                return SerialController.logs.gatewayLog;
            }
            else if (logType == "GatewayMessages")
            {
                return SerialController.logs.nodesLog;
            }
            else if (logType == "LogicalNodes")
            {
                return SerialController.logs.logicalNodesLog;
            }
            else if (logType == "LogicalNodesEngine")
            {
                return SerialController.logs.logicalNodesEngineLog;
            }
            else if (logType == "DataBase")
            {
                return SerialController.logs.dataBaseLog;
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
            else if (logType == "Gateway")
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
