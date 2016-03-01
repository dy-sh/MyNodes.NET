/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using MyNodes.Nodes;
using MyNodes.Users;
using MyNodes.WebController.Code;

namespace MyNodes.WebController.Controllers
{
    [Authorize(UserClaims.DashboardObserver)]

    public class UITimerController : Controller
    {
        private NodesEngine engine;

        const string ERROR_MESSAGE= "Nodes Engine is not started.<br/><br/>   <a href='/Config'>Check settings</a>";


        public UITimerController()
        {
            engine = SystemController.nodesEngine;
        }

        public ActionResult Tasks(string id)
        {
            if (engine == null)
                return View("Error", ERROR_MESSAGE);

            UiTimerNode node = engine.GetNode(id) as UiTimerNode;

            if (node == null)
                return HttpNotFound();

            ViewBag.NodeId = node.Id;
            ViewBag.Name = node.Settings["Name"].Value;

            List<UITimerTask> tasks = node.GetAllTasks();

            return View(tasks);
        }


        [Authorize(UserClaims.DashboardEditor)]

        [HttpGet]
        public ActionResult New(string id)
        {
            if (engine == null)
                return View("Error", ERROR_MESSAGE);

            UiTimerNode node = engine.GetNode(id) as UiTimerNode;
            if (node == null)
                return HttpNotFound();

            ViewBag.NodeId = node.Id;
            ViewBag.Name = node.Settings["Name"].Value;

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
            if (engine == null)
                return View("Error", ERROR_MESSAGE);

            UiTimerNode node = engine.GetNode(task.NodeId) as UiTimerNode;
            if (node == null)
                return HttpNotFound();

            node.AddTask(task);

            return RedirectToAction("Tasks", new { id = task.NodeId});
        }


        [Authorize(UserClaims.DashboardEditor)]

        [HttpGet]
        public ActionResult Edit(string id, int id2)
        {
            if (engine == null)
                return View("Error", ERROR_MESSAGE);

            UiTimerNode node = engine.GetNode(id) as UiTimerNode;
            if (node == null)
                return HttpNotFound();

            UITimerTask task = node.GetTask(id2);
            if (task == null)
                return HttpNotFound();

            ViewBag.Name = node.Settings["Name"].Value;

            return View(task);
        }


        [Authorize(UserClaims.DashboardEditor)]

        [HttpPost]
        public ActionResult Edit(UITimerTask task,int TaskId)
        {
            if (engine == null)
                return View("Error", ERROR_MESSAGE);

            task.Id = TaskId; //pasing id dont work!
            UiTimerNode node = engine.GetNode(task.NodeId) as UiTimerNode;
            if (node == null)
                return HttpNotFound();

            node.UpdateTask(task);

            return RedirectToAction("Tasks", new { id = task.NodeId });
        }


        [Authorize(UserClaims.DashboardEditor)]

        public ActionResult Remove(string id, int id2)
        {
            if (engine == null)
                return View("Error", ERROR_MESSAGE);

            UiTimerNode node = engine.GetNode(id) as UiTimerNode;
            if (node == null)
                return HttpNotFound();

            node.RemoveTask(id2);

            if (Request.Headers["Referer"].Any())
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("Tasks");
        }


        [Authorize(UserClaims.DashboardEditor)]

        public ActionResult Enable(string id, int id2)
        {
            if (engine == null)
                return View("Error", ERROR_MESSAGE);

            UiTimerNode node = engine.GetNode(id) as UiTimerNode;
            if (node == null)
                return HttpNotFound();

            UITimerTask task = node.GetTask(id2);
            if (task == null)
                return HttpNotFound();

            task.Enabled = true;

            node.UpdateTask(task);

            if (Request.Headers["Referer"].Any())
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("Tasks");
        }

        [Authorize(UserClaims.DashboardEditor)]

        public ActionResult Disable(string id, int id2)
        {
            if (engine == null)
                return View("Error", ERROR_MESSAGE);

            UiTimerNode node = engine.GetNode(id) as UiTimerNode;
            if (node == null)
                return HttpNotFound();

            UITimerTask task = node.GetTask(id2);
            if (task == null)
                return HttpNotFound();

            task.Enabled = false;

            node.UpdateTask(task);

            if (Request.Headers["Referer"].Any())
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("Tasks");
        }


        [Authorize(UserClaims.DashboardEditor)]

        public ActionResult ExecuteNow(string id, int id2)
        {
            if (engine == null)
                return View("Error", ERROR_MESSAGE);

            UiTimerNode node = engine.GetNode(id) as UiTimerNode;
            if (node == null)
                return HttpNotFound();

            node.ExecuteNowTask(id2);

            if (Request.Headers["Referer"].Any())
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("Tasks");
        }


        [Authorize(UserClaims.DashboardEditor)]

        public ActionResult RemoveAll(string id)
        {
            if (engine == null)
                return View("Error", ERROR_MESSAGE);

            UiTimerNode node = engine.GetNode(id) as UiTimerNode;
            if (node == null)
                return HttpNotFound();

            node.RemoveAllTasks();

            if (Request.Headers["Referer"].Any())
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("Tasks");
        }



        [Authorize(UserClaims.DashboardEditor)]

        public ActionResult RemoveCompleted(string id)
        {
            if (engine == null)
                return View("Error", ERROR_MESSAGE);

            UiTimerNode node = engine.GetNode(id) as UiTimerNode;
            if (node == null)
                return HttpNotFound();

            node.RemoveCompletedTasks();

            if (Request.Headers["Referer"].Any())
                return Redirect(Request.Headers["Referer"].ToString());
            else return RedirectToAction("Tasks");
        }


    }
}