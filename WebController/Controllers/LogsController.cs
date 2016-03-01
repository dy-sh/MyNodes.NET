/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using MyNodes.Users;
using MyNodes.WebController.Code;

namespace MyNodes.WebController.Controllers
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

        public IActionResult System()
        {
            ViewBag.LogType = "System";
            ViewBag.PageName = "System";
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
            ViewBag.LogType = "Gateway Messages";
            ViewBag.PageName = "Gateway Messages";
            return View("Logs");
        }

        public IActionResult GatewayDecodedMessages()
        {
            ViewBag.LogType = "Gateway Decoded Messages";
            ViewBag.PageName = "Gateway Decoded Messages";
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
            ViewBag.LogType = "Nodes Engine";
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
            else if (logType == "System")
            {
                return SystemController.logs.systemLog;
            }
            else if (logType == "Gateway")
            {
                return SystemController.logs.gatewayLog;
            }
            else if (logType == "Gateway Messages")
            {
                return SystemController.logs.gatewayMessagesLog;
            }
            else if (logType == "Gateway Decoded Messages")
            {
                return SystemController.logs.gatewayDecodedMessagesLog
                    .Select(log => new LogRecord(
                        LogRecordSource.GatewayDecodedMessage, 
                        LogRecordType.Info, 
                        log.ToString()))
                        .ToList();
            }
            else if (logType == "Nodes")
            {
                return SystemController.logs.nodesLog;
            }
            else if (logType == "Nodes Engine")
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
            else if (logType == "System")
            {
                SystemController.logs.systemLog.Clear();
            }
            else if (logType == "Gateway")
            {
                SystemController.logs.gatewayLog.Clear();
            }
            else if (logType == "Gateway Messages")
            {
                SystemController.logs.gatewayMessagesLog.Clear();
            }
            else if (logType == "Gateway Decoded Messages")
            {
                SystemController.logs.gatewayDecodedMessagesLog.Clear();
            }
            else if (logType == "Nodes")
            {
                SystemController.logs.nodesLog.Clear();
            }
            else if (logType == "Nodes Engine")
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
