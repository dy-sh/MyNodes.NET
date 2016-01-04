using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

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
    }
}
