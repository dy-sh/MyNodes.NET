using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace MyNetSensors.WebController.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Control", "Gateway");
            // return View();
        }

        public IActionResult About()
        {
            return View();
        }
        

        public IActionResult Error()
        {
            return View();
        }
    }
}
