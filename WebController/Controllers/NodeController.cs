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
        public ActionResult Settings(int id)
        {
            Node node = db.GetNodeByNodeId(id);
            return RedirectToAction("SensorLog", new { id = node.sensors[0].db_Id});
           // return View();
        }

        public ActionResult SensorLog(int id)
        {
            List<SensorData> samples = db.GetSensorDataLog(id);
            Sensor sensor = db.GetSensorByDbId(id);

            if (sensor==null && samples == null)
                return new HttpNotFoundResult();

            ViewBag.nodeId = sensor.ownerNodeId;
            ViewBag.sensorId = sensor.sensorId;
            return View(samples);
        }

        public ActionResult SensorGraph(int id)
        {
            Sensor sensor = db.GetSensorByDbId(id);

            if (sensor == null)
                return new HttpNotFoundResult();

            ViewBag.nodeId = sensor.ownerNodeId;
            ViewBag.sensorId = sensor.sensorId;


            return View();
        }

        public JsonResult GetSensorDataJson(int id)
        {
            List<SensorData> samples = db.GetSensorDataLog(id);

            if (samples == null)
                return null;

            var dateTime = new List<string>();
            var state = new List<string>();

            foreach (var item in samples)
            {
                dateTime.Add(String.Format("{0:yyyy-MM-dd HH:mm:ss}", item.dateTime));
                state.Add(item.state == null ? null : item.state);
            }

            return Json(new { dateTime, state }, JsonRequestBehavior.AllowGet);
        }
    }
}