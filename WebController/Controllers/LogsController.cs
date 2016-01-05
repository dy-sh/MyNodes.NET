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
            return View();
        }

        public IActionResult System()
        {
            return View();
        }

        public IActionResult Messages()
        {
            return View();
        }

        public List<string> GetAllLogs()
        {
            List<LogMessage> list = SerialController.logs.GetAllLogs();
            return list.Select(logMessage => logMessage.ToStringWithType()).ToList();
        }

        public bool ClearAllLogs()
        {
            SerialController.logs.ClearAllLogs();
            return true;
        }
    }
}
