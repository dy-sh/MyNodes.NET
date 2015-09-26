using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using MyNetSensors.WebController.Code.Hubs;

/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.WebController.Controllers
{
    public class GatewayController : Controller
    {
        IHubContext context = GlobalHost.ConnectionManager.GetHubContext<GatewayHub>();

        // GET: Gateway
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Messages()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }

        public ActionResult Observe()
        {
            return View();
        }

        public ActionResult Control()
        {
            return View();
        }

 

        public ActionResult DropDatabase()
        {
            context.Clients.Client(GatewayHubStaticData.gatewayId).clearLog();
            context.Clients.Client(GatewayHubStaticData.gatewayId).clearNodes();
            return RedirectToAction("Settings");
        }

        public int GetConnectedUsersCount()
        {
           return GatewayHubStaticData.connectedUsersId.Count;
        }


    }
}