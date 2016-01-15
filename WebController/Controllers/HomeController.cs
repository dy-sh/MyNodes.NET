using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;

namespace MyNetSensors.WebController.Controllers
{
    public class HomeController : Controller
    {
        private IConfigurationRoot сonfiguration;

        public HomeController(IConfigurationRoot сonfiguration)
        {
            this.сonfiguration = сonfiguration;
        }

        public IActionResult Index()
        {
            bool firstRun = Boolean.Parse(сonfiguration["FirstRun"]);

            if (firstRun)
                return RedirectToAction("Index", "Config");
            else
                return RedirectToAction("Index", "Dashboard");

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
