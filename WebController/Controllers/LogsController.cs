using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MyNetSensors.WebController.Code;

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
                return NodesController.logs.GetAllLogs();
            }
            else if(logType == "Errors")
            {
                return NodesController.logs.GetErrorsLogs();
            }
            else if (logType == "Controller")
            {
                return NodesController.logs.nodesControllerLog;
            }
            else if (logType == "Gateway")
            {
                return NodesController.logs.gatewayLog;
            }
            else if (logType == "HardwareNodes")
            {
                return NodesController.logs.hardwareNodesLog;
            }
            else if (logType == "Nodes")
            {
                return NodesController.logs.nodesLog;
            }
            else if (logType == "NodesEngine")
            {
                return NodesController.logs.nodesEngineLog;
            }
            else if (logType == "DataBase")
            {
                return NodesController.logs.dataBaseLog;
            }
            return null;
        }

        public bool ClearLogs(string logType)
        {
            if (logType == "All")
            {
                NodesController.logs.ClearAllLogs();
            }
            else if (logType == "Controller")
            {
                NodesController.logs.nodesControllerLog.Clear();
            }
            else if (logType == "Gateway")
            {
                NodesController.logs.gatewayLog.Clear();
            }
            else if (logType == "HardwareNodes")
            {
                NodesController.logs.hardwareNodesLog.Clear();
            }
            else if (logType == "Nodes")
            {
                NodesController.logs.nodesLog.Clear();
            }
            else if (logType == "NodesEngine")
            {
                NodesController.logs.nodesEngineLog.Clear();
            }
            else if (logType == "DataBase")
            {
                NodesController.logs.dataBaseLog.Clear();
            }

            return true;
        }
        

    }
}
