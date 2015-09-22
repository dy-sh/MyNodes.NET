using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyNetSensors.SerialGateway;
using MyNetSensors.WebController.Code;

namespace MyNetSensors.WebController.Controllers
{
    public class NodeController : Controller
    {
        private ISensorsRepository db;

        public NodeController()
        {
            string cs = ConfigurationManager.ConnectionStrings["GatewayDbConnection"].ConnectionString;
            db = new SensorsRepositoryDapper(cs);
        }

        // GET: Node
        public ActionResult Index(int id)
        {
            return View();
        }

        public ActionResult SensorLog(int id)
        {
            List<SensorData> list= db.GetSensorDataLog(id);
            Sensor sensor = db.GetSensor(id);

            if (sensor==null && list==null)
                return new HttpNotFoundResult();

            ViewBag.nodeId = sensor.ownerNodeId;
            ViewBag.sensorId = sensor.sensorId;
            return View(list);
        }

        public ActionResult SensorGraph(int id)
        {
            List<SensorData> list = db.GetSensorDataLog(id);
            Sensor sensor = db.GetSensor(id);

            if (sensor == null && list == null)
                return new HttpNotFoundResult();

            ViewBag.nodeId = sensor.ownerNodeId;
            ViewBag.sensorId = sensor.sensorId;
            return View(list);
        }
    }
}