/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.AspNet.Mvc;

using MyNetSensors.Gateways;
using MyNetSensors.Repositories.Dapper;
using MyNetSensors.SerialControllers;
using MyNetSensors.WebController.Code;


namespace MyNetSensors.WebController.Controllers
{
    public class HistoryController : Controller
    {
        private INodesHistoryRepository historyDb;
        private IGatewayRepository gatewayDb;

        public HistoryController()
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

        public ActionResult Log(int id, int id2)
        {
            Sensor sensor = gatewayDb.GetSensor(id, id2);

            if (sensor == null)
                return new HttpNotFoundResult();

            ViewBag.nodeId = sensor.nodeId;
            ViewBag.sensorId = sensor.sensorId;
            ViewBag.db_Id = sensor.Id;
            ViewBag.description = sensor.GetSimpleName1();

            List<SensorData> samples = historyDb.GetSensorHistory(sensor.nodeId,sensor.sensorId);
            return View(samples);
        }


        public ActionResult Chart(int id, int id2, string autoscroll, string style, string start, string end)
        {

            Sensor sensor = gatewayDb.GetSensor(id, id2);

            if (sensor == null)
                return new HttpNotFoundResult();

            ViewBag.nodeId = sensor.nodeId;
            ViewBag.sensorId = sensor.sensorId;
            ViewBag.db_Id = sensor.Id;
            ViewBag.description = sensor.GetSimpleName1();
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
            Sensor sensor = gatewayDb.GetSensor(id);

            List<SensorData> samples = historyDb.GetSensorHistory(sensor.nodeId,sensor.sensorId);

            if (!samples.Any())
                return Json(new { });

            var chartData = new List<ChartData>();

            if (samples[0].dataType == SensorDataType.V_TRIPPED)
            {
                foreach (var item in samples)
                {
                    ChartData sample = new ChartData();
                    sample.x = $"{item.dateTime:yyyy-MM-dd HH:mm:ss:fff}";
                    sample.y = item.state == "1" ? "1" : "-0.1";
                    sample.group = 0;
                    chartData.Add(sample);
                }
            }
            else
                foreach (var item in samples)
                {
                    ChartData sample = new ChartData();
                    sample.x = $"{item.dateTime:yyyy-MM-dd HH:mm:ss:fff}";
                    sample.y = item.state == null ? null : item.state;
                    sample.group = 0;
                    chartData.Add(sample);
                }

            //Sensor sensor = historyDb.GetSensorByDbId(id);
            string dataType = samples[0].dataType.ToString();
            JsonResult result = Json(new { chartData, dataType });

            return result;
        }

        public JsonResult GetSensorDataJson(int id, int id2)
        {
            Sensor sensor = gatewayDb.GetSensor(id, id2);
            return GetSensorDataJsonByDbId(sensor.Id);
        }


        public ActionResult ClearHistory(int id, int id2)
        {
            Sensor sensor = gatewayDb.GetSensor(id, id2);
            historyDb.DropSensorHistory(sensor.nodeId, sensor.sensorId);

            return RedirectToAction("Chart", new { id = id, id2 = id2 });
        }

        public ActionResult ClearHistoryByDbId(int id)
        {
            Sensor sensor = gatewayDb.GetSensor(id);
            historyDb.DropSensorHistory(sensor.nodeId, sensor.sensorId);

            return RedirectToAction("Chart", new { id = sensor.nodeId, id2 = sensor.sensorId });
        }



    }
}