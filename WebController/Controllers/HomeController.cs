using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;

namespace MyNetSensors.WebController.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult About()
        {
            ViewBag.Version = Assembly.GetExecutingAssembly().GetName().Version;


            return View();
        }


        public IActionResult Error()
        {
            return View();
        }








    }
}
