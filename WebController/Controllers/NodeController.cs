using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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

        [HttpGet]
        public ActionResult Settings(int id)
        {
            Node node = db.GetNodeByNodeId(id);
            return View(node);
        }

        [HttpPost]
        public ActionResult Settings()
        {
            int id = Int32.Parse(Request.Form["nodeId"]);
            Node node = db.GetNodeByNodeId(id);
            foreach (var sensor in node.sensors)
            {

                bool storetodb = Request.Form["storetodb" + sensor.sensorId] != "false";
                bool storechanges = Request.Form["storechanges" + sensor.sensorId] != "false";
                int storeinterval = Int32.Parse(Request.Form["storeinterval" + sensor.sensorId]);
                sensor.logToDbEnabled = storetodb;
                sensor.logToDbEveryChange = storechanges;
                sensor.logToDbWithInterval = storeinterval;
            }
            db.UpdateNodeSettings(node);
            return View(node);
        }



        public ActionResult Log(int id1, int id2)
        {
            Sensor sensor = db.GetSensor(id1, id2);

            if (sensor == null)
                return new HttpNotFoundResult();

            ViewBag.nodeId = sensor.ownerNodeId;
            ViewBag.sensorId = sensor.sensorId;
            ViewBag.db_Id = sensor.db_Id;
            ViewBag.description = sensor.GetDescrirtionOrType();

            List<SensorData> samples = db.GetSensorLog(sensor.db_Id);
            return View(samples);
        }


        public ActionResult Chart(int id1, int id2,string autoscroll,string style,string start,string end)
        {

            Sensor sensor = db.GetSensor(id1, id2);

            if (sensor == null)
                return new HttpNotFoundResult();

            ViewBag.nodeId = sensor.ownerNodeId;
            ViewBag.sensorId = sensor.sensorId;
            ViewBag.db_Id = sensor.db_Id;
            ViewBag.description = sensor.GetDescrirtionOrType();
            ViewBag.autoscroll = autoscroll;
            ViewBag.style = style;
            if (start != null)
                ViewBag.start = start;
            else
                ViewBag.start = "0";
            if (end != null)
                ViewBag.end = end;
            else 
                ViewBag.end = "0";
            return View();
        }


        public JsonResult GetSensorDataJsonByDbId(int id)
        {
            List<SensorData> samples = db.GetSensorLog(id);

            if (samples == null)
                return Json(new { }, JsonRequestBehavior.AllowGet);

            var chartData = new List<ChartData>();

            if (samples[0].dataType == SensorDataType.V_TRIPPED)
            {
                foreach (var item in samples)
                {
                    ChartData sample = new ChartData();
                    sample.x = String.Format("{0:yyyy-MM-dd HH:mm:ss}", item.dateTime);
                    sample.y = item.state == "1" ? "1" : "-0.1";
                    sample.group = 0;
                    chartData.Add(sample);
                }
            }
            else
                foreach (var item in samples)
                {
                    ChartData sample = new ChartData();
                    sample.x = String.Format("{0:yyyy-MM-dd HH:mm:ss}", item.dateTime);
                    sample.y = item.state == null ? null : item.state;
                    sample.group = 0;
                    chartData.Add(sample);
                }

            //Sensor sensor = db.GetSensorByDbId(id);
            string dataType = samples[0].dataType.ToString();
            JsonResult result = Json(new { chartData, dataType }, JsonRequestBehavior.AllowGet);

            return result;
        }

        public JsonResult GetSensorDataJson(int id1, int id2)
        {
            Sensor sensor = db.GetSensor(id1, id2);
            return GetSensorDataJson(sensor.ownerNodeId, sensor.sensorId);
        }


        public ActionResult ClearHistory(int id1, int id2)
        {
            db.DropSensorLog(id1, id2);

            return RedirectToAction("Chart", new { id1 = id1, id2 = id2 });
        }

        public ActionResult ClearHistoryByDbId(int id)
        {
            Sensor sensor = db.GetSensor( id);
            db.DropSensorLog(id);

            return RedirectToAction("Chart", new { id1 = sensor.ownerNodeId, id2 = sensor.sensorId });
        }

    }
}