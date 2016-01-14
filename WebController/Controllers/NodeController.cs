/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Configuration;
using Microsoft.AspNet.Mvc;


using MyNetSensors.Gateways;
using MyNetSensors.LogicalNodes;
using MyNetSensors.NodesTasks;
using MyNetSensors.Repositories.EF.SQLite;
using MyNetSensors.SerialControllers;


namespace MyNetSensors.WebController.Controllers
{
    public class NodeController : Controller
    {


      //  IHubContext clientsHub;
        private INodesHistoryRepository historyDb;
        private IGatewayRepository gatewayDb;

        public NodeController()
        {
            gatewayDb = SerialController.gatewayDb;
            historyDb = SerialController.historyDb;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var nodes = gatewayDb.GetNodes();
            return View(nodes);
        }

        [HttpGet]
        public ActionResult Settings(int id)
        {
            Node node = gatewayDb.GetNode(id);
            if (node == null)
                return HttpNotFound();

            foreach (var sensor in node.sensors)
            {
                if (String.IsNullOrEmpty(sensor.description))
                    sensor.description = sensor.GetSimpleName1();
            }

            return View(node);
        }

        [HttpPost]
        public ActionResult Settings()
        {
            int id = Int32.Parse(Request.Form["Id"]);
            Node node = gatewayDb.GetNode(id);
            string nodename = Request.Form["nodename"];
            if (nodename == "")
                nodename = null;
            node.name = nodename;
            foreach (var sensor in node.sensors)
            {

                string sensordescription = Request.Form["sensordescription-" + sensor.sensorId];
                if (sensordescription == "")
                    sensordescription = null;
                sensor.description = sensordescription;

                bool storehistory = Request.Form["storehistory-" + sensor.sensorId] != "false";
                bool writeeverychange = Request.Form["writeeverychange-" + sensor.sensorId] != "false";
                int writeinterval = Int32.Parse(Request.Form["writeinterval-" + sensor.sensorId]);
                sensor.storeHistoryEnabled = storehistory;
                sensor.storeHistoryEveryChange = writeeverychange;
                sensor.storeHistoryWithInterval = writeinterval;

                bool invertData = Request.Form["invertData-" + sensor.sensorId] != "false";
                bool remapEnabled = Request.Form["remapData-" + sensor.sensorId] != "false";
                string remapFromMin = Request.Form["remapFromMin-" + sensor.sensorId];
                string remapFromMax = Request.Form["remapFromMax-" + sensor.sensorId];
                string remapToMin = Request.Form["remapToMin-" + sensor.sensorId];
                string remapToMax = Request.Form["remapToMax-" + sensor.sensorId];
                sensor.invertData = invertData;
                sensor.remapEnabled = remapEnabled;
                sensor.remapFromMin = remapFromMin;
                sensor.remapFromMax = remapFromMax;
                sensor.remapToMin = remapToMin;
                sensor.remapToMax = remapToMax;
            }
            gatewayDb.UpdateNodeSettings(node);

            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.UpdateNodeSettings(node);


            return RedirectToAction("Control", "Gateway");
            // return View(node);
        }

        public ActionResult Delete(int id)
        {
            Node node = gatewayDb.GetNode(id);
            if (node == null)
                return HttpNotFound();


            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.DeleteNode(node.Id);

            gatewayDb.RemoveNode(node.Id);



            INodesTasksRepository tasksDb = SerialController.nodesTasksDb;

            foreach (var sensor in node.sensors)
            {
                historyDb.ClearSensorHistory(sensor.nodeId,sensor.sensorId);
                tasksDb.RemoveTasks(sensor.nodeId, sensor.sensorId);
            }

            gatewayApi.UpdateNodesTasks();



            return RedirectToAction("Control", "Gateway");
        }

    }
}