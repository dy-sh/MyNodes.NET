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


        public ActionResult Settings(int id)
        {
            return View();
        }

        public ActionResult SensorLog(int id)
        {
            List<SensorData> samples = db.GetSensorDataLog(id);
            Sensor sensor = db.GetSensorByDbId(id);

            if (sensor == null && samples == null)
                return new HttpNotFoundResult();

            ViewBag.nodeId = sensor.ownerNodeId;
            ViewBag.sensorId = sensor.sensorId;
            return View(samples);
        }


        public ActionResult SensorGraph(int id1, int id2)
        {

            Sensor sensor = db.GetSensorBySensorId(id1, id2);

            if (sensor == null)
                return new HttpNotFoundResult();

            ViewBag.nodeId = sensor.ownerNodeId;
            ViewBag.sensorId = sensor.sensorId;
            ViewBag.db_Id = sensor.db_Id;

            if (sensor.description != null)
                ViewBag.description = sensor.description;
            else
                ViewBag.description = MySensors.GetSimpleSensorType(sensor.sensorType);

            return View();
     }

        //public ActionResult SensorGraph(int id)
        //{

        //    Sensor sensor = db.GetSensorByDbId(id);

        //    if (sensor == null)
        //        return new HttpNotFoundResult();

        //    ViewBag.nodeId = sensor.ownerNodeId;
        //    ViewBag.sensorId = sensor.sensorId;
        //    ViewBag.db_Id = sensor.db_Id;

        //    return View();
        //}

        /*
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
        */

        public JsonResult GetSensorDataJson(int id)
        {
            List<SensorData> samples = db.GetSensorDataLog(id);

            if (samples == null)
                return null;

            var chartData = new List<ChartData>();

            if (samples[0].dataType == SensorDataType.V_TRIPPED)
            {
                foreach (var item in samples)
                {
                    ChartData sample = new ChartData();
                    sample.x = String.Format("{0:yyyy-MM-dd HH:mm:ss}", item.dateTime);
                    sample.y = item.state == "1" ? "1" : "-1";
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
    }
}