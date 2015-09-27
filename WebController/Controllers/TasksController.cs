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

            tasks.Add(new SensorTask
            {
                executionValue = new SensorData(SensorDataType.V_DIMMER, "100"),
                sensorId = 3,
                nodeId = 2,
                description = "Task1",
                db_Id = 1,
                executionDate = new DateTime(2015, 9, 27, 19, 30, 5),
                isCompleted = false,
                isRepeating = false,
                repeatingCount = 0,
                executionsDoneCount = 0
            });
            tasks.Add(new SensorTask
            {
                executionValue = new SensorData(SensorDataType.V_DIMMER, "100"),
                sensorId = 3,
                nodeId = 2,
                description = "Task1",
                db_Id = 1,
                executionDate = new DateTime(2015, 9, 27, 19, 30, 5),
                isCompleted = false,
                isRepeating = true,
                repeatingAValue = new SensorData(SensorDataType.V_DIMMER, "0"),
                repeatingBValue = new SensorData(SensorDataType.V_DIMMER, "100"),
                repeatingInterval = 1000,
                executionsDoneCount = 20,
                repeatingCount = -1
            });
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
                sensorId = id1,
                nodeId = id2,
                executionDate = DateTime.Now.AddMinutes(1),
                repeatingInterval = 1000,

            };

            SensorDataType? dataType = sensor.GetAllData()[0].dataType;
            if (dataType == SensorDataType.V_ARMED ||
                dataType == SensorDataType.V_TRIPPED)
            {
                task.repeatingAValue = new SensorData(SensorDataType.V_TRIPPED, "1");
                task.repeatingBValue = new SensorData(SensorDataType.V_TRIPPED, "0");
                task.executionValue = new SensorData(SensorDataType.V_TRIPPED, "1");
            }
            else
               if (dataType == SensorDataType.V_RGB)
            {
                task.repeatingAValue = new SensorData(SensorDataType.V_RGB, "#FFFFFF");
                task.repeatingBValue = new SensorData(SensorDataType.V_RGB, "#000000");
                task.executionValue = new SensorData(SensorDataType.V_RGB, "#FFFFFF");
            }
            else
               if (dataType == SensorDataType.V_RGBW)
            {
                task.repeatingAValue = new SensorData(SensorDataType.V_RGBW, "#FFFFFFFF");
                task.repeatingBValue = new SensorData(SensorDataType.V_RGBW, "#FFFFFF00");
                task.executionValue = new SensorData(SensorDataType.V_RGBW, "#FFFFFFFF");
            }
            else
               if (dataType == SensorDataType.V_PERCENTAGE ||
                    dataType == SensorDataType.V_DIMMER)
            {
                task.repeatingAValue = new SensorData(SensorDataType.V_DIMMER, "100");
                task.repeatingBValue = new SensorData(SensorDataType.V_DIMMER, "0");
                task.executionValue = new SensorData(SensorDataType.V_DIMMER, "100");
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
            return RedirectToAction("List");
        }
    }
}