/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace MyNodes.WebController.Controllers
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
