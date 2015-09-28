using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using MyNetSensors.Gateway;
using MyNetSensors.GatewayRepository;
using MyNetSensors.NodesLinks;
using MyNetSensors.NodeTasks;
using MyNetSensors.WebController.Code.Hubs;

namespace MyNetSensors.WebController.Controllers
{
    public class LinksController : Controller
    {
        private IHubContext context = GlobalHost.ConnectionManager.GetHubContext<GatewayHub>();
        private ISensorsLinksRepository linksDb;
        private IGatewayRepository gatewayDb;

        public LinksController()
        {
            string cs = ConfigurationManager.ConnectionStrings["GatewayDbConnection"].ConnectionString;
            gatewayDb = new GatewayRepositoryDapper(cs);
            linksDb = new SensorsLinksRepositoryDapper(cs);
        }



        public ActionResult List(int id1, int id2)
        {
            Sensor sensor = gatewayDb.GetSensor(id1, id2);

            if (sensor == null)
                return new HttpNotFoundResult();

            ViewBag.nodeId = sensor.ownerNodeId;
            ViewBag.sensorId = sensor.sensorId;
            ViewBag.db_Id = sensor.db_Id;
            ViewBag.description = sensor.GetSimpleName1();

            List<SensorLink> links = linksDb.GetLinksTo(id1, id2);

            return View(links);
        }


        [HttpGet]
        public ActionResult New(int id1, int id2)
        {
            Sensor sensor = gatewayDb.GetSensor(id1, id2);

            if (sensor == null)
                return new HttpNotFoundResult();

            ViewBag.nodes = gatewayDb.GetNodes();
            ViewBag.sensorDatas = sensor.GetAllData();
            ViewBag.description = sensor.GetSimpleName1();

            SensorLink link = new SensorLink()
            {
                toNodeId = sensor.ownerNodeId,
                toSensorId = sensor.sensorId
            };
            
            return View(link);
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

            SensorLink link = new SensorLink()
            {
                fromNodeId = fromNodeId,
                fromSensorId = fromSensorId,
                fromDataType = fromDataType,
                toNodeId = toNodeId,
                toSensorId = toSensorId,
                toDataType = toDataType
            };

            Sensor fromSensor = gatewayDb.GetSensor(link.toNodeId,link.toSensorId);
            Sensor toSensor = gatewayDb.GetSensor(link.toNodeId,link.toSensorId);

            if (fromSensor == null || toSensor==null)
                return new HttpNotFoundResult();

            linksDb.AddLink(link);
            context.Clients.Client(GatewayHubStaticData.gatewayId).updateSensorsLinks();
            return RedirectToAction("List", new { id1 = link.toNodeId, id2 = link.toSensorId });
        }



     

        public ActionResult Delete(int id)
        {
            SensorLink link = linksDb.GetLink(id);

            if (link == null)
                return new HttpNotFoundResult();

            linksDb.DeleteLink(id);
            context.Clients.Client(GatewayHubStaticData.gatewayId).updateSensorsLinks();
            return RedirectToAction("List", new { id1 = link.toNodeId, id2 = link.toSensorId });
        }

     
        public ActionResult DeleteAll(int id1, int id2)
        {
            Sensor sensor = gatewayDb.GetSensor(id1, id2);

            if (sensor == null)
                return new HttpNotFoundResult();

            linksDb.DeleteLinksTo(id1, id2);
            context.Clients.Client(GatewayHubStaticData.gatewayId).updateSensorsLinks();
            return RedirectToAction("List", new { id1, id2 });
        }




    }
}