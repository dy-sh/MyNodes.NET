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
using MyNetSensors.Gateways.MySensors.Serial;
using MyNetSensors.WebController.Code;


namespace MyNetSensors.WebController.Controllers
{
    [ResponseCache(Duration = 0)]
    public class HardwareController : Controller
    {
        private IMySensorsRepository mySensorsDb;

        public HardwareController()
        {
            mySensorsDb = SystemController.mySensorsDb;
        }

        public ActionResult Index()
        {
            return View();
        }





        public ActionResult SettingsSelect()
        {
            var nodes = mySensorsDb.GetNodes();
            return View(nodes);
        }

        [HttpGet]
        public ActionResult Settings(int? id)
        {
            if (id == null)
                return RedirectToAction("SettingsSelect");

            Node node = mySensorsDb.GetNode(id.Value);
            if (node == null)
                return HttpNotFound();

            return View(node);
        }

        [HttpPost]
        public ActionResult Settings()
        {
            int id = Int32.Parse(Request.Form["Id"]);
            Node node = mySensorsDb.GetNode(id);
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

            }
            mySensorsDb.UpdateNode(node);

            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.UpdateNodeSettings(node);


            return RedirectToAction("Index");
            // return View(node);
        }

        public ActionResult Remove(int id)
        {
            Node node = mySensorsDb.GetNode(id);
            if (node == null)
                return HttpNotFound();


            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.RemoveNode(node.Id);

            mySensorsDb.RemoveNode(node.Id);


            return RedirectToAction("Index");
        }
    }
}