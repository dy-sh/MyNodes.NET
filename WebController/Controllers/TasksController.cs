/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.AspNet.Mvc;


using MyNetSensors.Gateway;
using MyNetSensors.GatewayRepository;
using MyNetSensors.NodeTasks;
using MyNetSensors.SerialControl;
using MyNetSensors.WebController.Code;

namespace MyNetSensors.WebController.Controllers
{
    public class TasksController : Controller
    {

        private ISensorsTasksRepository tasksDb;
        private IGatewayRepository gatewayDb;

        public TasksController()
        {
            gatewayDb = SerialController.gatewayDb;
            tasksDb = SerialController.sensorsTasksDb;
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

                List<SensorTask> tasks = tasksDb.GetTasks(id.Value, id2.Value);

                return View(tasks);
            }

            else if (RouteData.Values.Count <= 2)
            {
                List<SensorTask> tasks = tasksDb.GetAllTasks();
                return View(tasks);
            }

            return new HttpNotFoundResult();
        }



        [HttpGet]
        public ActionResult NewSelect()
        {
            ViewBag.nodes = gatewayDb.GetNodes();

            return View();
        }

        [HttpPost]
        public ActionResult NewSelect(string sensor)
        {
            string[] args = sensor.Split('-');
            int nodeId = Int32.Parse(args[0]);
            int sensorId = Int32.Parse(args[1]);

            return RedirectToAction("New", new { id = nodeId, id2 = sensorId });
        }



        [HttpGet]
        public ActionResult New(int? id = null, int? id2 = null)
        {
            if (id != null && id2 != null)
            {
                Sensor sensor = gatewayDb.GetSensor(id.Value, id2.Value);

                if (sensor == null)
                    return new HttpNotFoundResult();

                Node node = gatewayDb.GetNodeByNodeId(sensor.nodeId);
                ViewBag.description = sensor.GetSimpleName1();

                SensorTask task = new SensorTask
                {
                    nodeId = id.Value,
                    sensorId = id2.Value,
                    sensorDbId = sensor.db_Id,
                    sensorDescription = $"{node.GetSimpleName1()} {sensor.GetSimpleName1()}",
                    executionDate = DateTime.Now,
                    repeatingInterval = 1000,
                    enabled = true
                };

                SensorDataType? dataType = sensor.dataType;
                task.dataType = dataType;

                if (dataType == SensorDataType.V_ARMED ||
                    dataType == SensorDataType.V_TRIPPED)
                {
                    task.executionValue = "1";
                    task.repeatingAValue = "0";
                    task.repeatingBValue = "1";
                }
                else if (dataType == SensorDataType.V_RGB)
                {
                    task.executionValue = "FFFFFF";
                    task.repeatingAValue = "000000";
                    task.repeatingBValue = "FFFFFF";
                }
                else if (dataType == SensorDataType.V_RGBW)
                {
                    task.executionValue = "FFFFFFFF";
                    task.repeatingAValue = "FFFFFF00";
                    task.repeatingBValue = "FFFFFFFF";
                }
                else if (dataType == SensorDataType.V_PERCENTAGE ||
                         dataType == SensorDataType.V_DIMMER)
                {
                    task.executionValue = "100";
                    task.repeatingAValue = "0";
                    task.repeatingBValue = "100";
                }
                return View(task);
            }
            else if (RouteData.Values.Count <= 2)
            {
                return RedirectToAction("NewSelect");
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        public ActionResult New(SensorTask task)
        {
            Sensor sensor = gatewayDb.GetSensor(task.nodeId, task.sensorId);

            if (sensor == null)
                return new HttpNotFoundResult();

            if (task.isRepeating)
                task.executionValue = task.repeatingBValue;

            tasksDb.AddTask(task);

            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.UpdateSensorsTasks();

            return RedirectToAction("List", new { id = task.nodeId, id2 = task.sensorId });
        }



        [HttpGet]
        public ActionResult Edit(int id)
        {
            SensorTask task = tasksDb.GetTask(id);

            if (task == null)
                return new HttpNotFoundResult();

            Sensor sensor = gatewayDb.GetSensor(task.nodeId, task.sensorId);

            ViewBag.description = sensor.GetSimpleName1();

            return View(task);
        }

        [HttpPost]
        public ActionResult Edit(SensorTask task)
        {
            Sensor sensor = gatewayDb.GetSensor(task.nodeId, task.sensorId);

            if (sensor == null)
                return new HttpNotFoundResult();

            if (task.isRepeating)
                task.executionValue = task.repeatingBValue;

            tasksDb.UpdateTask(task);

            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.UpdateSensorsTasks();

            return RedirectToAction("List", new { id = task.nodeId, id2 = task.sensorId });
        }

        public ActionResult Delete(int id)
        {
            SensorTask task = tasksDb.GetTask(id);

            if (task == null)
                return new HttpNotFoundResult();

            tasksDb.DeleteTask(id);

            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.UpdateSensorsTasks();

            if (Request.Headers["Referer"].Any())
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("List");
        }

        public ActionResult Enable(int id)
        {
            SensorTask task = tasksDb.GetTask(id);

            if (task == null)
                return new HttpNotFoundResult();

            tasksDb.UpdateTaskEnabled(task.db_Id, true);


            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.UpdateSensorsTasks();

            if (Request.Headers["Referer"].Any())
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("List");
        }

        public ActionResult Disable(int id)
        {
            SensorTask task = tasksDb.GetTask(id);

            if (task == null)
                return new HttpNotFoundResult();

            tasksDb.UpdateTaskEnabled(task.db_Id, false);


            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.UpdateSensorsTasks();

            if (Request.Headers["Referer"].Any())
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("List");
        }

        public ActionResult ExecuteNow(int id)
        {
            SensorTask task = tasksDb.GetTask(id);

            if (task == null)
                return new HttpNotFoundResult();

            tasksDb.UpdateTask(task.db_Id, true, false, DateTime.Now, 0);


            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.UpdateSensorsTasks();

            if (Request.Headers["Referer"].Any())
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

                tasksDb.DeleteTasks(id.Value, id2.Value);

                GatewayAPIController gatewayApi = new GatewayAPIController();
                gatewayApi.UpdateSensorsTasks();

                if (Request.Headers["Referer"].Any())
                    return Redirect(Request.Headers["Referer"].ToString());
                else return RedirectToAction("List");
            }
            else if (RouteData.Values.Count <= 2)
            {
                tasksDb.DropTasks();

                GatewayAPIController gatewayApi = new GatewayAPIController();
                gatewayApi.UpdateSensorsTasks();

                if (Request.Headers["Referer"].Any())
                    return Redirect(Request.Headers["Referer"].ToString());
                else return RedirectToAction("List");
            }

            return new HttpNotFoundResult();
        }

        public ActionResult DeleteCompleted(int? id = null, int? id2 = null)
        {
            if (id != null && id2 != null)
            {
                Sensor sensor = gatewayDb.GetSensor(id.Value, id2.Value);

                if (sensor == null)
                    return new HttpNotFoundResult();

                tasksDb.DeleteCompleted(id.Value, id2.Value);

                GatewayAPIController gatewayApi = new GatewayAPIController();
                gatewayApi.UpdateSensorsTasks();

                if (Request.Headers["Referer"].Any())
                    return Redirect(Request.Headers["Referer"].ToString());
                else return RedirectToAction("List");
            }
            else if (RouteData.Values.Count <= 2)
            {
                tasksDb.DeleteCompleted();

                GatewayAPIController gatewayApi = new GatewayAPIController();
                gatewayApi.UpdateSensorsTasks();

                if (Request.Headers["Referer"].Any())
                    return Redirect(Request.Headers["Referer"].ToString());
                else return RedirectToAction("List");
            }

            return new HttpNotFoundResult();
        }


    }
}