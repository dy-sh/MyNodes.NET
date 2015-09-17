using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using MyNetSensors.WebController.Code.Hubs;
using MyNetSensors.SerialGateway;

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

        public ActionResult Log()
        {
            return View();
        }

        public ActionResult NodesView()
        {
            return View();
        }

        public ActionResult NodesControl()
        {
            return View();
        }

        public void ClearLog()
        {
            context.Clients.All.clearLog();
        }

        public void GetLog()
        {
            context.Clients.All.getLog();
            //todo store if of requesting cliend, and further send respond to this id
        }

        public void GetNodes()
        {
            context.Clients.All.getNodes();
            //todo store if of requesting cliend, and further send respond to this id
        }

    }
}