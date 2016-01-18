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
    public class DashboardController : Controller
    {
        const string MAIN_PANEL_ID = "Main";

        private LogicalNodesUIEngine engineUI = SerialController.logicalNodesUIEngine;


        public IActionResult Index()
        {
            return View();
        }



        public IActionResult Panel(string id)
        {
            if (id == null || id == MAIN_PANEL_ID)
            {
                id = MAIN_PANEL_ID;
                ViewBag.panelName = "Main Panel";
            }
            else
            {
                LogicalNodePanel panel = engineUI.GetPanel(id);
                if (panel == null)
                    return HttpNotFound();
                ViewBag.panelName = panel.Name;
            }

            ViewBag.panelId = id;
            return View("Index");
        }

        public IActionResult List()
        {
            ViewBag.showMainPanel = engineUI.GetUINodesForPanel(MAIN_PANEL_ID).Any();

            List<LogicalNodePanel> allPanels = engineUI.GetPanels();
            List<LogicalNodePanel> notEmptyPanels = new List<LogicalNodePanel>();
            foreach (var panel in allPanels)
            {
                if (engineUI.GetUINodesForPanel(panel.Id).Any())
                    notEmptyPanels.Add(panel);
            }

            return View(notEmptyPanels);
        }


        
        public ActionResult Chart(string id, string autoscroll, string style, string start, string end)
        {
            if (engineUI == null)
                return null;

            LogicalNodeUIChart chart = engineUI.GetUINode(id) as LogicalNodeUIChart;
            if (chart == null)
                return new HttpNotFoundResult();

            ViewBag.autoscroll = autoscroll;
            ViewBag.style = style;
            ViewBag.start = start ?? "0";
            ViewBag.end = end ?? "0";

            return View(chart);
        }
    }
}
