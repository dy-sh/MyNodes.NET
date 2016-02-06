/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;


using MyNetSensors.Gateways;
using MyNetSensors.Nodes;
using MyNetSensors.Users;
using MyNetSensors.WebController.Code;

namespace MyNetSensors.WebController.Controllers
{
    [Authorize(UserClaims.DashboardObserver)]

    public class UITimerController : Controller
    {
        private UITimerNodesEngine tasksEngine;
        private NodesEngine engine;

        public UITimerController()
        {
            tasksEngine = SystemController.uiTimerNodesEngine;
            engine = SystemController.nodesEngine;
        }

        public ActionResult Tasks(string id)
        {
            UiTimerNode node = engine.GetNode(id) as UiTimerNode;

            if (node == null)
                return HttpNotFound();

            ViewBag.NodeId = node.Id;
            ViewBag.Name = node.Name;

            List<UITimerTask> tasks = tasksEngine.GetTasksForNode(id);

            return View(tasks);
        }


        [Authorize(UserClaims.DashboardEditor)]

        [HttpGet]
        public ActionResult New(string id)
        {
            UiTimerNode node = engine.GetNode(id) as UiTimerNode;

            if (node == null)
                return HttpNotFound();

            ViewBag.NodeId = node.Id;
            ViewBag.Name = node.Name;

            UITimerTask task = new UITimerTask
            {
                NodeId = id,
                ExecutionDate = DateTime.Now,
                RepeatingInterval = 1000,
                Enabled = true
            };

            return View(task);
        }


        [Authorize(UserClaims.DashboardEditor)]

        [HttpPost]
        public ActionResult New(UITimerTask task)
        {
            task.IsRepeating = Request.Form["isRepeating"] != "false";

            bool result = tasksEngine.AddTask(task);
            if (!result)
                return HttpBadRequest();

            return RedirectToAction("Tasks", new { id = task.NodeId});
        }


        [Authorize(UserClaims.DashboardEditor)]

        [HttpGet]
        public ActionResult Edit(int id)
        {
            UITimerTask task = tasksEngine.GetTask(id);

            if (task == null)
                return HttpNotFound();

            UiTimerNode node = engine.GetNode(task.NodeId) as UiTimerNode;

            if (node == null)
                return HttpNotFound();

            ViewBag.Name = node.Name;

            return View(task);
        }


        [Authorize(UserClaims.DashboardEditor)]

        [HttpPost]
        public ActionResult Edit(UITimerTask task)
        {
            task.IsRepeating = Request.Form["isRepeating"] != "false";

            bool result = tasksEngine.UpdateTask(task);
            if (!result)
                return HttpBadRequest();

            return RedirectToAction("Tasks", new { id = task.NodeId });
        }


        [Authorize(UserClaims.DashboardEditor)]

        public ActionResult Remove(int id)
        {
            bool result = tasksEngine.RemoveTask(id);
            if (!result)
                return HttpBadRequest();

            if (Request.Headers["Referer"].Any())
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("Tasks");
        }


        [Authorize(UserClaims.DashboardEditor)]

        public ActionResult Enable(int id)
        {
            UITimerTask task = tasksEngine.GetTask(id);

            if (task == null)
                return HttpNotFound();

            task.Enabled = true;

            bool result = tasksEngine.UpdateTask(task);
            if (!result)
                return HttpBadRequest();

            if (Request.Headers["Referer"].Any())
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("Tasks");
        }

        [Authorize(UserClaims.DashboardEditor)]

        public ActionResult Disable(int id)
        {
            UITimerTask task = tasksEngine.GetTask(id);

            if (task == null)
                return HttpNotFound();

            task.Enabled = false;

            bool result = tasksEngine.UpdateTask(task);
            if (!result)
                return HttpBadRequest();

            if (Request.Headers["Referer"].Any())
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("Tasks");
        }


        [Authorize(UserClaims.DashboardEditor)]

        public ActionResult ExecuteNow(int id)
        {
            bool result = tasksEngine.ExecuteNowTask(id);
            if (!result)
                return HttpBadRequest();

            if (Request.Headers["Referer"].Any())
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("Tasks");
        }


        [Authorize(UserClaims.DashboardEditor)]

        public ActionResult RemoveAll(string id)
        {
            bool result = tasksEngine.RemoveTasksForNode(id);
            if (!result)
                return HttpBadRequest();

            if (Request.Headers["Referer"].Any())
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("Tasks");
        }



        [Authorize(UserClaims.DashboardEditor)]

        public ActionResult RemoveCompleted(string id)
        {
            bool result = tasksEngine.RemoveCompletedTasksForNode(id);
            if (!result)
                return HttpBadRequest();

            if (Request.Headers["Referer"].Any())
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("Tasks");
        }


    }
}