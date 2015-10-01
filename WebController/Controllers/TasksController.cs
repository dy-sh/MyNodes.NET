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
using MyNetSensors.WebController.Code;

namespace MyNetSensors.WebController.Controllers
{
    public class TasksController : Controller
    {
        private IHubContext clientsHub = GlobalHost.ConnectionManager.GetHubContext<ClientsHub>();
        private ISensorsTasksRepository tasksDb;
        private IGatewayRepository gatewayDb;

        public TasksController()
        {
            string cs = ConfigurationManager.ConnectionStrings["GatewayDbConnection"].ConnectionString;
            gatewayDb = new GatewayRepositoryDapper(cs);
            tasksDb = new SensorsTasksRepositoryDapper(cs);
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List(int? id1 = null, int? id2 = null)
        {
            if (id1 != null && id2 != null)
            {
                Sensor sensor = gatewayDb.GetSensor(id1.Value, id2.Value);

                if (sensor == null)
                    return new HttpNotFoundResult();

                ViewBag.nodeId = sensor.ownerNodeId;
                ViewBag.sensorId = sensor.sensorId;
                ViewBag.db_Id = sensor.db_Id;
                ViewBag.description = sensor.GetSimpleName1();

                List<SensorTask> tasks = tasksDb.GetTasks(id1.Value, id2.Value);

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

            return RedirectToAction("New", new {id1 = nodeId, id2 = sensorId});
        }



        [HttpGet]
        public ActionResult New(int? id1 = null, int? id2 = null)
        {
            if (id1 != null && id2 != null)
            {
                Sensor sensor = gatewayDb.GetSensor(id1.Value, id2.Value);

                if (sensor == null)
                    return new HttpNotFoundResult();

                Node node = gatewayDb.GetNodeByNodeId(sensor.ownerNodeId);
                ViewBag.description = sensor.GetSimpleName1();

                SensorTask task = new SensorTask
                {
                    nodeId = id1.Value,
                    sensorId = id2.Value,
                    sensorDbId = sensor.db_Id,
                    sensorDescription = string.Format("{0} {1}", node.GetSimpleName1(), sensor.GetSimpleName1()),
                    executionDate = DateTime.Now,
                    repeatingInterval = 1000,
                    enabled = true
                };

                SensorDataType? dataType = sensor.GetAllData()[0].dataType;
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
                    task.executionValue = "#FFFFFF";
                    task.repeatingAValue = "#000000";
                    task.repeatingBValue = "#FFFFFF";
                }
                else if (dataType == SensorDataType.V_RGBW)
                {
                    task.executionValue = "#FFFFFFFF";
                    task.repeatingAValue = "#FFFFFF00";
                    task.repeatingBValue = "#FFFFFFFF";
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
            clientsHub.Clients.All.updateSensorsTasks();
            return RedirectToAction("List",new {id1= task.nodeId,id2=task.sensorId});
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
            clientsHub.Clients.All.updateSensorsTasks();
            return RedirectToAction("List", new { id1 = task.nodeId, id2 = task.sensorId });
        }

        public ActionResult Delete(int id)
        {
            SensorTask task = tasksDb.GetTask(id);

            if (task == null)
                return new HttpNotFoundResult();

            tasksDb.DeleteTask(id);
            clientsHub.Clients.All.updateSensorsTasks();

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.ToString());
            else return RedirectToAction("List");
        }

        public ActionResult Enable(int id)
        {
            SensorTask task = tasksDb.GetTask(id);

            if (task == null)
                return new HttpNotFoundResult();

            tasksDb.UpdateTaskEnabled(task.db_Id,true);

            clientsHub.Clients.All.updateSensorsTasks();

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.ToString());
            else return RedirectToAction("List");
        }

        public ActionResult Disable(int id)
        {
            SensorTask task = tasksDb.GetTask(id);

            if (task == null)
                return new HttpNotFoundResult();

            tasksDb.UpdateTaskEnabled(task.db_Id, false);

            clientsHub.Clients.All.updateSensorsTasks();

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.ToString());
            else return RedirectToAction("List");
        }

        public ActionResult ExecuteNow(int id)
        {
            SensorTask task = tasksDb.GetTask(id);

            if (task == null)
                return new HttpNotFoundResult();

            tasksDb.UpdateTask(task.db_Id,true,false,DateTime.Now,0);

            clientsHub.Clients.All.updateSensorsTasks();

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.ToString());
            else return RedirectToAction("List");
        }

  
        public ActionResult DeleteAll(int? id1 = null, int? id2 = null)
        {
            if (id1 != null && id2 != null)
            {
                Sensor sensor = gatewayDb.GetSensor(id1.Value, id2.Value);

                if (sensor == null)
                    return new HttpNotFoundResult();

                tasksDb.DeleteTasks(id1.Value, id2.Value);
                clientsHub.Clients.All.updateSensorsTasks();

                if (Request.UrlReferrer != null)
                    return Redirect(Request.UrlReferrer.ToString());
                else return RedirectToAction("List");
            }
            else if (RouteData.Values.Count <= 2)
            {
                tasksDb.DropAllTasks();
                clientsHub.Clients.All.updateSensorsTasks();

                if (Request.UrlReferrer != null)
                    return Redirect(Request.UrlReferrer.ToString());
                else return RedirectToAction("List");
            }

            return new HttpNotFoundResult();
        }

        public ActionResult DeleteCompleted(int? id1 = null, int? id2 = null)
        {
            if (id1 != null && id2 != null)
            {
                Sensor sensor = gatewayDb.GetSensor(id1.Value, id2.Value);

                if (sensor == null)
                    return new HttpNotFoundResult();

                tasksDb.DeleteCompleted(id1.Value, id2.Value);
                clientsHub.Clients.All.updateSensorsTasks();

                if (Request.UrlReferrer != null)
                    return Redirect(Request.UrlReferrer.ToString());
                else return RedirectToAction("List");
            }
            else if (RouteData.Values.Count <= 2)
            {
                tasksDb.DeleteCompleted();
                clientsHub.Clients.All.updateSensorsTasks();

                if (Request.UrlReferrer != null)
                    return Redirect(Request.UrlReferrer.ToString());
                else return RedirectToAction("List");
            }

           return new HttpNotFoundResult();
        }


    }
}