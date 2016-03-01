/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MyNodes.Gateways;
using LiteGraph;
using Microsoft.AspNet.Authorization;
using Microsoft.Extensions.Configuration;
using MyNodes.Repositories.EF.SQLite;
using MyNodes.Nodes;
using MyNodes.Users;
using MyNodes.WebController.Code;
using MyNodes.WebController.ViewModels.NodeEditor;
using Newtonsoft.Json;
using Node = MyNodes.Nodes.Node;


namespace MyNodes.WebController.Controllers
{
    [Authorize(UserClaims.EditorObserver)]
    public class NodeEditorController : Controller
    {
        const string MAIN_PANEL_ID = "Main";

        private NodesEngine engine = SystemController.nodesEngine;


        public IActionResult Index(bool split)
        {
            if (engine == null)
                return View("Error", "Nodes Engine is not started.<br/><br/>   <a href='/Config'>Check settings</a>");

            ViewBag.split = split;
            ViewBag.panelId = MAIN_PANEL_ID;

            ViewBag.Theme = SystemController.nodeEditorConfig.Theme;

            return View();
        }

        public IActionResult Panel(string id, bool split)
        {
            if (engine == null)
                return View("Error", "Nodes Engine is not started.<br/><br/>   <a href='/Config'>Check settings</a>");


            ViewBag.split = split;

            if (id == null || id == MAIN_PANEL_ID)
                return RedirectToAction("Index");

            PanelNode panel = engine.GetPanelNode(id);

            if (panel == null)
                return HttpNotFound();

            ViewBag.panelId = panel.Id;
            ViewBag.ownerPanelId = panel.PanelId;

            //create menu stack
            List<PanelNode> panelsStack = new List<PanelNode>();

            bool findNext = true;
            while (findNext)
            {
                panelsStack.Add(panel);
                if (panel.PanelId == MAIN_PANEL_ID)
                    findNext = false;
                else
                {
                    panel = engine.GetPanelNode(panel.PanelId);
                }
            }

            panelsStack.Reverse();
            ViewBag.panelsStack = panelsStack;

            ViewBag.Theme = SystemController.nodeEditorConfig.Theme;


            return View("Index");
        }


        public IActionResult Split(string id)
        {
            if (engine == null)
                return View("Error", "Nodes Engine is not started.<br/><br/>   <a href='/Config'>Check settings</a>");

            if (id == null)
                ViewBag.route = "";
            else
                ViewBag.route = "Panel/" + id;
            return View();
        }

        public IActionResult NodesDescription()
        {
            List<Nodes.Node> nodes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                 .Where(t => t.IsSubclassOf(typeof(Nodes.Node)) && !t.IsAbstract)
                .Select(t => (Nodes.Node)Activator.CreateInstance(t)).ToList();

            nodes = nodes.OrderBy(x => x.Category + x.Type).ToList();

            List<NodeDescription> descrition = nodes.Select(node => new NodeDescription
            {
                category = node.Category,
                type = node.Type,
                description = node.GetNodeDescription()
            }).ToList();

            return View(descrition);
        }
    }
}
