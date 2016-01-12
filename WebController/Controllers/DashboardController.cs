using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MyNetSensors.LogicalNodes;
using MyNetSensors.LogicalNodesUI;
using MyNetSensors.SerialControllers;

namespace MyNetSensors.WebController.Controllers
{
    public class DashboardController:Controller
    {

        private LogicalNodesUIEngine engineUI = SerialController.logicalNodesUIEngine;


        public IActionResult Index()
        {
            return View();
        }


        public List<LogicalNodeUI> GetUINodes()
        {
            if (engineUI == null)
                return null;

            return engineUI.GetUINodes();
        }

    }
}
