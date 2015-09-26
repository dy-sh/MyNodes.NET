using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using MyNetSensors.GatewayRepository;
using MyNetSensors.SensorsHistoryRepository;
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



        public ActionResult DropNodes()
        {
            DropHistoryDatabase();
            context.Clients.Client(GatewayHubStaticData.gatewayId).clearLog();
            context.Clients.Client(GatewayHubStaticData.gatewayId).clearNodes();
            return RedirectToAction("Settings");
        }

        public ActionResult DropHistory()
        {
            DropHistoryDatabase();

            return RedirectToAction("Settings");
        }

        private void DropHistoryDatabase()
        {
            string cs = ConfigurationManager.ConnectionStrings["GatewayDbConnection"].ConnectionString;
            ISensorsHistoryRepository historyDb = new SensorsHistoryRepositoryDapper(cs);
            IGatewayRepository gatewayDb = new GatewayRepositoryDapper(cs);

            var nodes = gatewayDb.GetNodes();
            //turn off writing history in nodes settings
            foreach (var node in nodes)
            {
                foreach (var sensor in node.sensors)
                {
                    sensor.storeHistoryEnabled = false;
                }
                gatewayDb.UpdateNodeSettings(node);
                context.Clients.Client(GatewayHubStaticData.gatewayId).updateNodeSettings(node);
                Task.Delay(100);
            }

            //waiting for all history writings finished
            Task.Delay(2000);

            //drop tables
            foreach (var node in nodes)
            {
                foreach (var sensor in node.sensors)
                {
                    historyDb.DropSensorHistory(sensor.db_Id);
                }
            }
        }

        public int GetConnectedUsersCount()
        {
            return GatewayHubStaticData.connectedUsersId.Count;
        }


    }
}