using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyNetSensors.Gateway;
using MyNetSensors.GatewayRepository;
using MyNetSensors.SensorsHistoryRepository;
using MyNetSensors.WebController.Code;

namespace MyNetSensors.WebController.Controllers
{
    public class NodeController : Controller
    {
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
            return View(node);
        }

        [HttpPost]
        public ActionResult Settings()
        {
            int id = Int32.Parse(Request.Form["nodeId"]);
            Node node = gatewayDb.GetNodeByNodeId(id);
            foreach (var sensor in node.sensors)
            {

                bool storehistory = Request.Form["storehistory" + sensor.sensorId] != "false";
                bool writeeverychange = Request.Form["writeeverychange" + sensor.sensorId] != "false";
                int writeinterval = Int32.Parse(Request.Form["writeinterval" + sensor.sensorId]);
                sensor.storeHistoryEnabled = storehistory;
                sensor.storeHistoryEveryChange = writeeverychange;
                sensor.storeHistoryWithInterval = writeinterval;
            }
            gatewayDb.UpdateNodeSettings(node);
           //todo controller.UpdateNodeSettings();
            return View(node);
        }



        public ActionResult Log(int id1, int id2)
        {
            Sensor sensor = gatewayDb.GetSensor(id1, id2);

            if (sensor == null)
                return new HttpNotFoundResult();

            ViewBag.nodeId = sensor.ownerNodeId;
            ViewBag.sensorId = sensor.sensorId;
            ViewBag.db_Id = sensor.db_Id;
            ViewBag.description = sensor.GetDescrirtionOrType();

            List<SensorData> samples = historyDb.GetSensorHistory(sensor.db_Id);
            return View(samples);
        }


        public ActionResult Chart(int id1, int id2,string autoscroll,string style,string start,string end)
        {

            Sensor sensor = gatewayDb.GetSensor(id1, id2);

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
            List<SensorData> samples = historyDb.GetSensorHistory(id);

            if (samples == null)
                return Json(new { }, JsonRequestBehavior.AllowGet);

            var chartData = new List<ChartData>();

            if (samples[0].dataType == SensorDataType.V_TRIPPED)
            {
                foreach (var item in samples)
                {
                    ChartData sample = new ChartData();
                    sample.x = String.Format("{0:yyyy-MM-dd HH:mm:ss:fff}", item.dateTime);
                    sample.y = item.state == "1" ? "1" : "-0.1";
                    sample.group = 0;
                    chartData.Add(sample);
                }
            }
            else
                foreach (var item in samples)
                {
                    ChartData sample = new ChartData();
                    sample.x = String.Format("{0:yyyy-MM-dd HH:mm:ss:fff}", item.dateTime);
                    sample.y = item.state == null ? null : item.state;
                    sample.group = 0;
                    chartData.Add(sample);
                }

            //Sensor sensor = historyDb.GetSensorByDbId(id);
            string dataType = samples[0].dataType.ToString();
            JsonResult result = Json(new { chartData, dataType }, JsonRequestBehavior.AllowGet);

            return result;
        }

        public JsonResult GetSensorDataJson(int id1, int id2)
        {
            Sensor sensor = gatewayDb.GetSensor(id1, id2);
            return GetSensorDataJsonByDbId(sensor.db_Id);
        }


        public ActionResult ClearHistory(int id1, int id2)
        {
            Sensor sensor = gatewayDb.GetSensor(id1, id2);
            historyDb.DropSensorHistory(sensor.db_Id);

            return RedirectToAction("Chart", new { id1 = id1, id2 = id2 });
        }

        public ActionResult ClearHistoryByDbId(int id)
        {
            Sensor sensor = gatewayDb.GetSensor( id);
            historyDb.DropSensorHistory(id);

            return RedirectToAction("Chart", new { id1 = sensor.ownerNodeId, id2 = sensor.sensorId });
        }

    }
}