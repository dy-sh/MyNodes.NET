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
using MyNetSensors.NodesTasks;
using MyNetSensors.SerialControllers;
using MyNetSensors.WebController.Code;

namespace MyNetSensors.WebController.Controllers
{
    public class TasksController : Controller
    {
        const int MIN_REPEAT_INTERVAL = 20;


        private INodesTasksRepository tasksDb;
        private IGatewayRepository gatewayDb;

        public TasksController()
        {
            gatewayDb = SerialController.gatewayDb;
            tasksDb = SerialController.nodesTasksDb;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List(int? id = null, int? id2 = null)
        {
            //if (id != null && id2 != null)
            //{
            //    Sensor sensor = gatewayDb.GetSensor(id.Value, id2.Value);

            //    if (sensor == null)
            //        return new HttpNotFoundResult();

            //    ViewBag.nodeId = sensor.nodeId;
            //    ViewBag.sensorId = sensor.sensorId;
            //    ViewBag.description = sensor.GetSimpleName1();

            //    List<NodeTask> tasks = tasksDb.GetTasks(id.Value, id2.Value);

            //    return View(tasks);
            //}

            //else if (RouteData.Values.Count <= 2)
            //{
            //    List<NodeTask> tasks = tasksDb.GetAllTasks();
            //    return View(tasks);
            //}

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
            //if (id != null && id2 != null)
            //{
            //    Sensor sensor = gatewayDb.GetSensor(id.Value, id2.Value);

            //    if (sensor == null)
            //        return new HttpNotFoundResult();

            //    Node node = gatewayDb.GetNode(sensor.nodeId);
            //    ViewBag.description = sensor.GetSimpleName1();

            //    NodeTask task = new NodeTask
            //    {
            //        NodeId = id.Value,
            //        sensorId = id2.Value,
            //        sensorDbId = sensor.Id,
            //        sensorDescription = $"{node.GetSimpleName1()} {sensor.GetSimpleName1()}",
            //        ExecutionDate = DateTime.Now,
            //        RepeatingInterval = 1000,
            //        Enabled = true
            //    };

            //    task.dataType = sensor.dataType;

            //    if (sensor.IsBinary())
            //    {
            //        task.ExecutionValue = "1";
            //        task.RepeatingAValue = "0";
            //        task.RepeatingBValue = "1";
            //    }
            //    else if (sensor.dataType == SensorDataType.V_RGB)
            //    {
            //        task.ExecutionValue = "FFFFFF";
            //        task.RepeatingAValue = "000000";
            //        task.RepeatingBValue = "FFFFFF";
            //    }
            //    else if (sensor.dataType == SensorDataType.V_RGBW)
            //    {
            //        task.ExecutionValue = "FFFFFFFF";
            //        task.RepeatingAValue = "FFFFFF00";
            //        task.RepeatingBValue = "FFFFFFFF";
            //    }
            //    else if (sensor.IsPercentage())
            //    {
            //        task.ExecutionValue = "100";
            //        task.RepeatingAValue = "0";
            //        task.RepeatingBValue = "100";
            //    }
            //    return View(task);
            //}
            //else if (RouteData.Values.Count <= 2)
            //{
            //    return RedirectToAction("NewSelect");
            //}
            return new HttpNotFoundResult();
        }

        [HttpPost]
        public ActionResult New(NodeTask task)
        {
            //task.Id = 0;
            //Sensor sensor = gatewayDb.GetSensor(task.NodeId, task.sensorId);

            //if (sensor == null)
            //    return new HttpNotFoundResult();

            //task.IsRepeating= Request.Form["isRepeating"] != "false";
            //if (task.IsRepeating)
            //    task.ExecutionValue = task.RepeatingBValue;

            //if (task.IsRepeating && task.RepeatingInterval < MIN_REPEAT_INTERVAL)
            //{
            //    ModelState.Clear();
            //    ModelState.AddModelError("", "Repeating Interval must be at least 20 ms");
            //    return View(task);
            //}

            //tasksDb.AddTask(task);

            //GatewayAPIController gatewayApi = new GatewayAPIController();
            //gatewayApi.UpdateNodesTasks();

           // return RedirectToAction("List", new { id = task.nodeId, id2 = task.sensorId });
            return RedirectToAction("List");
        }



        [HttpGet]
        public ActionResult Edit(int id)
        {
            //NodeTask task = tasksDb.GetTask(id);

            //if (task == null)
            //    return new HttpNotFoundResult();

            //Sensor sensor = gatewayDb.GetSensor(task.NodeId, task.sensorId);

            //ViewBag.description = sensor.GetSimpleName1();

            //return View(task);
            return null;
        }

        [HttpPost]
        public ActionResult Edit(NodeTask task)
        {
           // Sensor sensor = gatewayDb.GetSensor(task.nodeId, task.sensorId);

            //if (sensor == null)
            //    return new HttpNotFoundResult();

            task.IsRepeating= Request.Form["isRepeating"] != "false";
            if (task.IsRepeating)
                task.ExecutionValue = task.RepeatingBValue;

            if (task.IsRepeating && task.RepeatingInterval < MIN_REPEAT_INTERVAL)
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Repeating Interval must be at least 20 ms");
                return View(task);
            }

            tasksDb.UpdateTask(task);

            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.UpdateNodesTasks();

            // return RedirectToAction("List", new { id = task.nodeId, id2 = task.sensorId });
            return RedirectToAction("List");
        }

        public ActionResult Remove(int id)
        {
            NodeTask task = tasksDb.GetTask(id);

            if (task == null)
                return new HttpNotFoundResult();

            tasksDb.RemoveTask(id);

            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.UpdateNodesTasks();

            if (Request.Headers["Referer"].Any())
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("List");
        }

        public ActionResult Enable(int id)
        {
            NodeTask task = tasksDb.GetTask(id);

            if (task == null)
                return new HttpNotFoundResult();

            tasksDb.UpdateTaskEnabled(task.Id, true);


            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.UpdateNodesTasks();

            if (Request.Headers["Referer"].Any())
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("List");
        }

        public ActionResult Disable(int id)
        {
            NodeTask task = tasksDb.GetTask(id);

            if (task == null)
                return new HttpNotFoundResult();

            tasksDb.UpdateTaskEnabled(task.Id, false);


            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.UpdateNodesTasks();

            if (Request.Headers["Referer"].Any())
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("List");
        }

        public ActionResult ExecuteNow(int id)
        {
            NodeTask task = tasksDb.GetTask(id);

            if (task == null)
                return new HttpNotFoundResult();

            tasksDb.UpdateTask(task.Id, true, false, DateTime.Now, 0);


            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.UpdateNodesTasks();

            if (Request.Headers["Referer"].Any())
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("List");
        }


        public ActionResult RemoveAll(int? id = null, int? id2 = null)
        {
            //if (id != null && id2 != null)
            //{
            //    Sensor sensor = gatewayDb.GetSensor(id.Value, id2.Value);

            //    if (sensor == null)
            //        return new HttpNotFoundResult();

            //    tasksDb.RemoveTasks(id.Value, id2.Value);

            //    GatewayAPIController gatewayApi = new GatewayAPIController();
            //    gatewayApi.UpdateNodesTasks();

            //    if (Request.Headers["Referer"].Any())
            //        return Redirect(Request.Headers["Referer"].ToString());
            //    else return RedirectToAction("List");
            //}
            //else if (RouteData.Values.Count <= 2)
            //{
            //    tasksDb.RemoveAllTasks();

            //    GatewayAPIController gatewayApi = new GatewayAPIController();
            //    gatewayApi.UpdateNodesTasks();

            //    if (Request.Headers["Referer"].Any())
            //        return Redirect(Request.Headers["Referer"].ToString());
            //    else return RedirectToAction("List");
            //}

            return new HttpNotFoundResult();
        }

        public ActionResult RemoveCompleted(int? id = null, int? id2 = null)
        {
            //if (id != null && id2 != null)
            //{
            //    Sensor sensor = gatewayDb.GetSensor(id.Value, id2.Value);

            //    if (sensor == null)
            //        return new HttpNotFoundResult();

            //    tasksDb.RemoveCompletedTasks(id.Value, id2.Value);

            //    GatewayAPIController gatewayApi = new GatewayAPIController();
            //    gatewayApi.UpdateNodesTasks();

            //    if (Request.Headers["Referer"].Any())
            //        return Redirect(Request.Headers["Referer"].ToString());
            //    else return RedirectToAction("List");
            //}
            //else if (RouteData.Values.Count <= 2)
            //{
            //    tasksDb.RemoveCompletedTasks();

            //    GatewayAPIController gatewayApi = new GatewayAPIController();
            //    gatewayApi.UpdateNodesTasks();

            //    if (Request.Headers["Referer"].Any())
            //        return Redirect(Request.Headers["Referer"].ToString());
            //    else return RedirectToAction("List");
            //}

            return new HttpNotFoundResult();
        }


    }
}