using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MyNetSensors.LogicalNodes;
using MyNetSensors.SerialControllers;

namespace MyNetSensors.WebController.Controllers
{
    public class DashboardController:Controller
    {

        private LogicalNodesEngine engine = SerialController.logicalNodesEngine;



        public IActionResult Index()
        {
            if (engine == null)
                return null;

            List<LogicalNode> nodes = engine.nodes;

            return View(nodes);
        }
    }
}
