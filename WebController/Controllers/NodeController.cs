using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using MyNetSensors.Gateway;
using MyNetSensors.GatewayRepository;
using MyNetSensors.NodeTasks;
using MyNetSensors.SensorsHistoryRepository;
using MyNetSensors.WebController.Code;
using MyNetSensors.WebController.Code.Hubs;

namespace MyNetSensors.WebController.Controllers
{
    public class NodeController : Controller
    {
        IHubContext context = GlobalHost.ConnectionManager.GetHubContext<GatewayHub>();
        private ISensorsHistoryRepository historyDb;
        private IGatewayRepository gatewayDb;

        public NodeController()
        {
            string cs = ConfigurationManager.ConnectionStrings["GatewayDbConnection"].ConnectionString;
            historyDb = new SensorsHistoryRepositoryDapper(cs);
            gatewayDb = new GatewayRepositoryDapper(cs);
        }

        [HttpGet]
        public ActionResult Settings(int id)
        {
            Node node = gatewayDb.GetNodeByNodeId(id);
            if (node == null)
                return HttpNotFound();

            return View(node);
        }

        [HttpPost]
        public ActionResult Settings()
        {
            int id = Int32.Parse(Request.Form["nodeId"]);
            Node node = gatewayDb.GetNodeByNodeId(id);
            string nodename = Request.Form["nodename"];
            if (nodename == "")
                nodename = null;
            node.name = nodename;
            foreach (var sensor in node.sensors)
            {

                bool storehistory = Request.Form["storehistory" + sensor.sensorId] != "false";
                bool writeeverychange = Request.Form["writeeverychange" + sensor.sensorId] != "false";
                int writeinterval = Int32.Parse(Request.Form["writeinterval" + sensor.sensorId]);
                string sensordescription = Request.Form["sensordescription" + sensor.sensorId];
                if (sensordescription == "")
                    sensordescription = null;
                sensor.storeHistoryEnabled = storehistory;
                sensor.storeHistoryEveryChange = writeeverychange;
                sensor.storeHistoryWithInterval = writeinterval;
                sensor.description = sensordescription;
            }
            gatewayDb.UpdateNodeSettings(node);
            context.Clients.Client(GatewayHubStaticData.gatewayId).updateNodeSettings(node);
            return RedirectToAction("Control", "Gateway");
            // return View(node);
        }


    
    }
}