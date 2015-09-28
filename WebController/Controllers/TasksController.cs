/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using MyNetSensors.Gateway;
using MyNetSensors.GatewayRepository;
using MyNetSensors.NodeTasks;
using MyNetSensors.SensorsHistoryRepository;
using MyNetSensors.WebController.Code.Hubs;

namespace MyNetSensors.WebController.Controllers
{
    public class TasksController : Controller
    {
        private IHubContext context = GlobalHost.ConnectionManager.GetHubContext<GatewayHub>();
        private ISensorsTasksRepository tasksDb;
        private IGatewayRepository gatewayDb;

        public TasksController()
        {
            string cs = ConfigurationManager.ConnectionStrings["GatewayDbConnection"].ConnectionString;
            gatewayDb = new GatewayRepositoryDapper(cs);
            tasksDb = new SensorsTasksRepositoryDapper(cs);
        }



        public ActionResult List(int id1, int id2)
        {
            Sensor sensor = gatewayDb.GetSensor(id1, id2);

            if (sensor == null)
                return new HttpNotFoundResult();

            ViewBag.nodeId = sensor.ownerNodeId;
            ViewBag.sensorId = sensor.sensorId;
            ViewBag.db_Id = sensor.db_Id;
            ViewBag.description = sensor.GetDescrirtionOrType();

            List<SensorTask> tasks = tasksDb.GetTasks(id1, id2);

            return View(tasks);
        }


        [HttpGet]
        public ActionResult New(int id1, int id2)
        {
            Sensor sensor = gatewayDb.GetSensor(id1, id2);

            if (sensor == null)
                return new HttpNotFoundResult();

            ViewBag.description = sensor.GetDescrirtionOrType();

            SensorTask task = new SensorTask
            {
                nodeId = id1,
                sensorId = id2,
                executionDate = DateTime.Now,
                repeatingInterval = 1000,
                enabled = true
            };

            SensorDataType? dataType = sensor.GetAllData()[0].dataType;
            task.dataType = dataType;

            if (dataType == SensorDataType.V_ARMED ||
                dataType == SensorDataType.V_TRIPPED)
            {
                task.executionValue="1";
                task.repeatingAValue="0";
                task.repeatingBValue = "1";
            }
            else
               if (dataType == SensorDataType.V_RGB)
            {
                task.executionValue = "#FFFFFF";
                task.repeatingAValue = "#000000";
                task.repeatingBValue = "#FFFFFF";
            }
            else
               if (dataType == SensorDataType.V_RGBW)
            {
                task.executionValue = "#FFFFFFFF";
                task.repeatingAValue = "#FFFFFF00";
                task.repeatingBValue = "#FFFFFFFF";
            }
            else
               if (dataType == SensorDataType.V_PERCENTAGE ||
                    dataType == SensorDataType.V_DIMMER)
            {
                task.executionValue = "100";
                task.repeatingAValue = "0";
                task.repeatingBValue = "100";
            }
            return View(task);
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
            context.Clients.Client(GatewayHubStaticData.gatewayId).updateSensorsTasks();
            return RedirectToAction("List",new {id1= task.nodeId,id2=task.sensorId});
        }



        [HttpGet]
        public ActionResult Edit(int id)
        {
            SensorTask task = tasksDb.GetTask(id);

            if (task == null)
                return new HttpNotFoundResult();

            Sensor sensor = gatewayDb.GetSensor(task.nodeId, task.sensorId);

            ViewBag.description = sensor.GetDescrirtionOrType();

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
            context.Clients.Client(GatewayHubStaticData.gatewayId).updateSensorsTasks();
            return RedirectToAction("List", new { id1 = task.nodeId, id2 = task.sensorId });
        }

        public ActionResult Delete(int id)
        {
            SensorTask task = tasksDb.GetTask(id);

            if (task == null)
                return new HttpNotFoundResult();

            tasksDb.DeleteTask(id);
            context.Clients.Client(GatewayHubStaticData.gatewayId).updateSensorsTasks();
            return RedirectToAction("List", new { id1 = task.nodeId, id2 = task.sensorId });
        }

        public ActionResult Enable(int id)
        {
            SensorTask task = tasksDb.GetTask(id);

            if (task == null)
                return new HttpNotFoundResult();

            tasksDb.UpdateTaskEnabled(task.db_Id,true);

            context.Clients.Client(GatewayHubStaticData.gatewayId).updateSensorsTasks();
            return RedirectToAction("List", new { id1 = task.nodeId, id2 = task.sensorId });
        }

        public ActionResult Disable(int id)
        {
            SensorTask task = tasksDb.GetTask(id);

            if (task == null)
                return new HttpNotFoundResult();

            tasksDb.UpdateTaskEnabled(task.db_Id, false);

            context.Clients.Client(GatewayHubStaticData.gatewayId).updateSensorsTasks();
            return RedirectToAction("List", new { id1 = task.nodeId, id2 = task.sensorId });
        }

        public ActionResult ExecuteNow(int id)
        {
            SensorTask task = tasksDb.GetTask(id);

            if (task == null)
                return new HttpNotFoundResult();

            tasksDb.UpdateTask(task.db_Id,true,false,DateTime.Now,0);

            context.Clients.Client(GatewayHubStaticData.gatewayId).updateSensorsTasks();
            return RedirectToAction("List", new { id1 = task.nodeId, id2 = task.sensorId });
        }

  
        public ActionResult DeleteAll(int id1, int id2)
        {
            Sensor sensor = gatewayDb.GetSensor(id1, id2);

            if (sensor == null)
                return new HttpNotFoundResult();

            tasksDb.DeleteTasks(id1,id2);
            context.Clients.Client(GatewayHubStaticData.gatewayId).updateSensorsTasks();
            return RedirectToAction("List", new { id1, id2});
        }

        public ActionResult DeleteCompleted(int id1, int id2)
        {
            Sensor sensor = gatewayDb.GetSensor(id1, id2);

            if (sensor == null)
                return new HttpNotFoundResult();

            tasksDb.DeleteCompleted(id1, id2);
            context.Clients.Client(GatewayHubStaticData.gatewayId).updateSensorsTasks();
            return RedirectToAction("List", new { id1, id2 });
        }


    }
}