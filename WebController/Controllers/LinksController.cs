/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.AspNet.Mvc;


using MyNetSensors.Gateways;
using MyNetSensors.GatewayRepository;
using MyNetSensors.NodesLinks;
using MyNetSensors.SerialControllers;
using MyNetSensors.WebController.Code;

namespace MyNetSensors.WebController.Controllers
{
     public class LinksController : Controller
    {
        //private IHubContext clientsHub;
        private ISensorsLinksRepository linksDb;
        private IGatewayRepository gatewayDb;

        public LinksController()
        {
            gatewayDb = SerialController.gatewayDb;
            linksDb = SerialController.sensorsLinksDb;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List(int? id = null, int? id2 = null)
        {
            if (id != null && id2 != null)
            {
                Sensor sensor = gatewayDb.GetSensor(id.Value, id2.Value);

                if (sensor == null)
                    return new HttpNotFoundResult();

                ViewBag.nodeId = sensor.nodeId;
                ViewBag.sensorId = sensor.sensorId;
                ViewBag.db_Id = sensor.db_Id;
                ViewBag.description = sensor.GetSimpleName1();

                List<SensorLink> links = linksDb.GetLinksTo(id.Value, id2.Value);
                return View(links);

            }
            else if (RouteData.Values.Count<=2)
            {
                List<SensorLink> links = linksDb.GetAllLinks();
                return View(links);
            }

            return new HttpNotFoundResult();
        }


        [HttpGet]
        public ActionResult New(int? id = null, int? id2 = null)
        {
            if (id != null && id2 != null)
            {
                Sensor sensor = gatewayDb.GetSensor(id.Value, id2.Value);

                if (sensor == null)
                    return new HttpNotFoundResult();

                ViewBag.from = gatewayDb.GetNodes();
                ViewBag.description = sensor.GetSimpleName1();

                List<Node> toList=new List<Node>();
                Node toNode = gatewayDb.GetNodeByNodeId(sensor.nodeId);
                toNode.sensors.Clear();
                toNode.sensors.Add(sensor);
                toList.Add(toNode);

                ViewBag.to = toList;


                SensorLink link = new SensorLink()
                {
                    toNodeId = sensor.nodeId,
                    toSensorId = sensor.sensorId
                };

                return View(link);
            }
            else if (RouteData.Values.Count <= 2)
            {
                ViewBag.from = gatewayDb.GetNodes();
                ViewBag.to = ViewBag.from;

                return View();
            }

            return new HttpNotFoundResult();
        }

        [HttpPost]
        public ActionResult New(string from, string to)
        {
            string[] args = from.Split('-');
            int fromNodeId = Int32.Parse(args[0]);
            int fromSensorId = Int32.Parse(args[1]);
            SensorDataType fromDataType;
            SensorDataType.TryParse(args[2], true, out fromDataType);

            args = to.Split('-');
            int toNodeId = Int32.Parse(args[0]);
            int toSensorId = Int32.Parse(args[1]);
            SensorDataType toDataType;
            SensorDataType.TryParse(args[2], true, out toDataType);

            Sensor fromSensor = gatewayDb.GetSensor(fromNodeId, fromSensorId);
            Sensor toSensor = gatewayDb.GetSensor(toNodeId, toSensorId);

            if (fromSensor == null || toSensor == null)
                return new HttpNotFoundResult();

            bool linkPossible = CheckLinkPossible(fromSensor, toSensor);
            if (!linkPossible)
            {
                ModelState.AddModelError("", "Link impossible");
                return RedirectToAction("New");
            }

            Node fromNode = gatewayDb.GetNodeByNodeId(fromSensor.nodeId);
            Node toNode = gatewayDb.GetNodeByNodeId(toSensor.nodeId);

            SensorLink link = new SensorLink()
            {
                fromNodeId = fromNodeId,
                fromSensorId = fromSensorId,
                fromDataType = fromDataType,
                fromSensorDbId = fromSensor.db_Id,
                fromSensorDescription = $"{fromNode.GetSimpleName1()} {fromSensor.GetSimpleName1()}",
                toNodeId = toNodeId,
                toSensorId = toSensorId,
                toDataType = toDataType,
                toSensorDbId = toSensor.db_Id,
                toSensorDescription = $"{toNode.GetSimpleName1()} {toSensor.GetSimpleName1()}"
            };



            linksDb.AddLink(link);

            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.UpdateSensorsLinks();

            return RedirectToAction("List", new { id = link.toNodeId, id2 = link.toSensorId });
        }

        private bool CheckLinkPossible(Sensor fromSensor, Sensor toSensor)
        {
            //if it's the same sensor
            if (fromSensor.nodeId == toSensor.nodeId
                && fromSensor.sensorId == toSensor.sensorId)
                return false;

            
            List<SensorLink> links = linksDb.GetAllLinks();
            foreach (var link in links)
            {
                //prevent infinite loop 
                if (link.fromNodeId == toSensor.nodeId
                    && link.fromSensorId == toSensor.sensorId
                    && link.toNodeId == fromSensor.nodeId
                    && link.toSensorId == fromSensor.sensorId)
                    return false;

                //prevend duplicates
                if (link.fromNodeId == fromSensor.nodeId
                    && link.fromSensorId == fromSensor.sensorId
                    && link.toNodeId == toSensor.nodeId
                    && link.toSensorId == toSensor.sensorId)
                    return false;
            }

            return true;
        }


        public ActionResult Delete(int id)
        {
            SensorLink link = linksDb.GetLink(id);

            if (link == null)
                return new HttpNotFoundResult();

            linksDb.DeleteLink(id);

            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.UpdateSensorsLinks();

            if (Request.Headers["Referer"].Count > 0)
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("List");
        }


        public ActionResult DeleteAll(int? id = null, int? id2 = null)
        {
            if (id != null && id2 != null)
            {
                Sensor sensor = gatewayDb.GetSensor(id.Value, id2.Value);

                if (sensor == null)
                    return new HttpNotFoundResult();

                linksDb.DeleteLinksTo(id.Value, id2.Value);

                GatewayAPIController gatewayApi = new GatewayAPIController();
                gatewayApi.UpdateSensorsLinks();

                if (Request.Headers["Referer"].Count > 0)
                    return Redirect(Request.Headers["Referer"].ToString());
                else return RedirectToAction("List");
            }
            else if (RouteData.Values.Count <= 2)
            {
                linksDb.DropLinks();

                GatewayAPIController gatewayApi = new GatewayAPIController();
                gatewayApi.UpdateSensorsLinks();

                if (Request.Headers["Referer"].Count>0)
                    return Redirect(Request.Headers["Referer"].ToString());
                else return RedirectToAction("List");
            }

            return new HttpNotFoundResult();
        }









    }
}