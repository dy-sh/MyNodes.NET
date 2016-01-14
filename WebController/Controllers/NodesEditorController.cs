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
        private LogicalNodesEngine engine = SerialController.logicalNodesEngine;

        public IActionResult Index(string panelId)
        {
            if (panelId != null)
            {
                LogicalNodePanel panel = engine.GetPanelNode(panelId);

                if (panel==null)
                    return HttpNotFound();

                ViewBag.panelId = panel.Id;
                ViewBag.ownerPanelId = panel.PanelId;
            }

            //panelId==null - main graph
            return View();
        }

    }
}
