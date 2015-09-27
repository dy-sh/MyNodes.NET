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
                executionDate = DateTime.Now.AddMinutes(1),
                repeatingInterval = 1000,
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
            //for infinity
            if (task.repeatingCount == 0)
                task.repeatingCount = -1;

            tasksDb.AddOrUpdateTask(task);
            return RedirectToAction("List",new {id1= task.nodeId,id2=task.sensorId});
        }
    }
}