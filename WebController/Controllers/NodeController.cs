/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Configuration;
using Microsoft.AspNet.Mvc;


using MyNetSensors.Gateway;
using MyNetSensors.GatewayRepository;
using MyNetSensors.NodesLinks;
using MyNetSensors.NodeTasks;
using MyNetSensors.SensorsHistoryRepository;
using MyNetSensors.SerialControl;


namespace MyNetSensors.WebServer.Controllers
{
    public class NodeController : Controller
    {


      //  IHubContext clientsHub;
        private ISensorsHistoryRepository historyDb;
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

                string sensordescription = Request.Form["sensordescription" + sensor.sensorId];
                if (sensordescription == "")
                    sensordescription = null;
                sensor.description = sensordescription;

                bool storehistory = Request.Form["storehistory" + sensor.sensorId] != "false";
                bool writeeverychange = Request.Form["writeeverychange" + sensor.sensorId] != "false";
                int writeinterval = Int32.Parse(Request.Form["writeinterval" + sensor.sensorId]);
                sensor.storeHistoryEnabled = storehistory;
                sensor.storeHistoryEveryChange = writeeverychange;
                sensor.storeHistoryWithInterval = writeinterval;

                bool invertData = Request.Form["invertData" + sensor.sensorId] != "false";
                bool remapEnabled = Request.Form["remapData" + sensor.sensorId] != "false";
                string remapFromMin = Request.Form["remapFromMin" + sensor.sensorId];
                string remapFromMax = Request.Form["remapFromMax" + sensor.sensorId];
                string remapToMin = Request.Form["remapToMin" + sensor.sensorId];
                string remapToMax = Request.Form["remapToMax" + sensor.sensorId];
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
            Node node = gatewayDb.GetNodeByNodeId(id);
            if (node == null)
                return HttpNotFound();


            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.DeleteNode(node.nodeId);

            gatewayDb.DeleteNodeByDbId(node.db_Id);



            string cs = ConfigurationManager.ConnectionStrings["GatewayDbConnection"].ConnectionString;
            ISensorsLinksRepository linksDb = new SensorsLinksRepositoryDapper(cs);
            ISensorsTasksRepository tasksDb = new SensorsTasksRepositoryDapper(cs);

            foreach (var sensor in node.sensors)
            {
                historyDb.DropSensorHistory(sensor.db_Id);
                linksDb.DeleteLinksTo(sensor.db_Id);
                linksDb.DeleteLinksFrom(sensor.db_Id);
                tasksDb.DeleteTasks(sensor.db_Id);
            }

            gatewayApi.UpdateSensorsTasks();
            gatewayApi.UpdateSensorsLinks();



            return RedirectToAction("Control", "Gateway");
        }

    }
}