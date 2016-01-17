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
using MyNetSensors.NodesTasks;
using MyNetSensors.SerialControllers;
using MyNetSensors.WebController.Code;


namespace MyNetSensors.WebController.Controllers
{
    [ResponseCache(Duration = 0)]
    public class HardwareController : Controller
    {
        private IGatewayRepository gatewayDb;

        public HardwareController()
        {
            gatewayDb = SerialController.gatewayDb;
        }

        public ActionResult Index()
        {
            return View();
        }




        public ActionResult Control()
        {
            return View();
        }




        public ActionResult SettingsSelect()
        {
            var nodes = gatewayDb.GetNodes();
            return View(nodes);
        }

        [HttpGet]
        public ActionResult Settings(int? id)
        {
            if (id == null)
                return RedirectToAction("SettingsSelect");

            Node node = gatewayDb.GetNode(id.Value);
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


            return RedirectToAction("Index");
            // return View(node);
        }

        public ActionResult Remove(int id)
        {
            Node node = gatewayDb.GetNode(id);
            if (node == null)
                return HttpNotFound();


            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.RemoveNode(node.Id);

            gatewayDb.RemoveNode(node.Id);



            INodesTasksRepository tasksDb = SerialController.nodesTasksDb;

            foreach (var sensor in node.sensors)
            {
                tasksDb.RemoveTasks(sensor.nodeId, sensor.sensorId);
            }

            gatewayApi.UpdateNodesTasks();



            return RedirectToAction("Index");
        }
    }
}