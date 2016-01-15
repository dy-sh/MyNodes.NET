using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MyNetSensors.Gateways;
using LiteGraph;
using MyNetSensors.LogicalNodes;
using MyNetSensors.Repositories.EF.SQLite;
//using MyNetSensors.LogicalNodes;
using MyNetSensors.SerialControllers;
using Newtonsoft.Json;
using Input = LiteGraph.Input;
using Node = MyNetSensors.Gateways.Node;
using Output = LiteGraph.Output;

namespace MyNetSensors.WebController.Controllers
{

    public class NodesEditorController : Controller
    {
        const string MAIN_PANEL_ID = "Main";

        private LogicalNodesEngine engine = SerialController.logicalNodesEngine;

        public IActionResult Index(string panelId)
        {
            ViewBag.panelId = MAIN_PANEL_ID;
           // ViewBag.ownerPanelId = MAIN_PANEL_ID;

            if (panelId != null)
            {
                LogicalNodePanel panel = engine.GetPanelNode(panelId);

                if (panel==null)
                    return HttpNotFound();

                ViewBag.panelId = panel.Id;
                ViewBag.ownerPanelId = panel.PanelId;

                //create menu stack
                List<LogicalNodePanel> panelsStack=new List<LogicalNodePanel>();

                bool findNext=true;
                while (findNext)
                {
                    panelsStack.Add(panel);
                    if (panel.PanelId == MAIN_PANEL_ID)
                        findNext = false;
                    else
                    {
                        panel= engine.GetPanelNode(panel.PanelId);
                    }
                }

                panelsStack.Reverse();
                ViewBag.panelsStack = panelsStack;
            }

            return View();
        }

    }
}
