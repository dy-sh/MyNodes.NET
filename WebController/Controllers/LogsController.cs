using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using MyNetSensors.Users;
using MyNetSensors.WebController.Code;

namespace MyNetSensors.WebController.Controllers
{
    [Authorize(UserClaims.LogsObserver)]

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

        public IActionResult Errors()
        {
            ViewBag.LogType = "Errors";
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

        public IActionResult HardwareNodes()
        {
            ViewBag.LogType = "HardwareNodes";
            ViewBag.PageName = "Hardware Nodes";
            return View("Logs");
        }


        public IActionResult Nodes()
        {
            ViewBag.LogType = "Nodes";
            ViewBag.PageName = "Nodes";
            return View("Logs");
        }

        public IActionResult NodesEngine()
        {
            ViewBag.LogType = "NodesEngine";
            ViewBag.PageName = "Nodes Engine";
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
                return SystemController.logs.GetAllLogs();
            }
            else if(logType == "Errors")
            {
                return SystemController.logs.GetErrorsLogs();
            }
            else if (logType == "Controller")
            {
                return SystemController.logs.systemLog;
            }
            else if (logType == "Gateway")
            {
                return SystemController.logs.gatewayLog;
            }
            else if (logType == "HardwareNodes")
            {
                return SystemController.logs.hardwareNodesLog;
            }
            else if (logType == "Nodes")
            {
                return SystemController.logs.nodesLog;
            }
            else if (logType == "NodesEngine")
            {
                return SystemController.logs.nodesEngineLog;
            }
            else if (logType == "DataBase")
            {
                return SystemController.logs.dataBaseLog;
            }
            return null;
        }

        [Authorize(UserClaims.LogsEditor)]

        public bool ClearLogs(string logType)
        {
            if (logType == "All")
            {
                SystemController.logs.ClearAllLogs();
            }
            else if (logType == "Controller")
            {
                SystemController.logs.systemLog.Clear();
            }
            else if (logType == "Gateway")
            {
                SystemController.logs.gatewayLog.Clear();
            }
            else if (logType == "HardwareNodes")
            {
                SystemController.logs.hardwareNodesLog.Clear();
            }
            else if (logType == "Nodes")
            {
                SystemController.logs.nodesLog.Clear();
            }
            else if (logType == "NodesEngine")
            {
                SystemController.logs.nodesEngineLog.Clear();
            }
            else if (logType == "DataBase")
            {
                SystemController.logs.dataBaseLog.Clear();
            }

            return true;
        }
        

    }
}
