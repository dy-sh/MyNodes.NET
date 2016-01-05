/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using MyNetSensors.Gateways;
using MyNetSensors.WebController.Code;


namespace MyNetSensors.WebController.Controllers
{
    [ResponseCache(Duration = 0)]
    public class GatewayController : Controller
    {

        public ActionResult Index()
        {
            return RedirectToAction("Control");
        }


        public ActionResult Observe()
        {
            return View();
        }

        public ActionResult Control()
        {
            return View();
        }
    }
}