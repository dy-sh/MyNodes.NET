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
            return RedirectToAction("Show",new { id = "System"});
        }


        public IActionResult Show(string id)
        {
            ViewBag.LogType = id;
            ViewBag.PageName = id;

            switch (id)
            {
                case "GatewayState":
                    ViewBag.PageName = "Gateway State";
                    break;
                case "GatewayMessages":
                    ViewBag.PageName = "Gateway Messages";
                    break;
                case "GatewayDecodedMessages":
                    ViewBag.PageName = "Gateway Decoded Messages";
                    break;
                case "NodesEngine":
                    ViewBag.PageName = "Nodes Engine";
                    break;
            }

            return View();
        }
        
        



        public List<LogRecord> GetLogs(string logType)
        {
            if (logType == "All")
                return SystemController.logs.GetAllLogs();

            if (logType == "Errors")
                return SystemController.logs.GetErrorsLogs();

            LogRecordSource source;
            bool result = LogRecordSource.TryParse(logType, out source);
            if (!result)
                return null;

            return SystemController.logs.GetLogsOfSource(
                (LogRecordSource)Enum.Parse(typeof(LogRecordSource), logType));
        }



        [Authorize(UserClaims.LogsEditor)]

        public bool ClearLogs(string logType)
        {
            if (logType == "All")
            {
                SystemController.logs.ClearAllLogs();
                return true;
            }

            SystemController.logs.ClearLogsOfSource(
                (LogRecordSource)Enum.Parse(typeof(LogRecordSource), logType));
  

            return true;
        }
        

    }
}
