using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MyNetSensors.Nodes;
using MyNetSensors.WebController.Code;

namespace MyNetSensors.WebController.Controllers
{
    public class DashboardController : Controller
    {
        const string MAIN_PANEL_ID = "Main";

        private UiNodesEngine engine = NodesController.uiNodesEngine;


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
                PanelNode panel = engine.GetPanel(id);
                if (panel == null)
                    return HttpNotFound();
                ViewBag.panelName = panel.Name;
            }

            ViewBag.panelId = id;
            return View("Index");
        }

        public IActionResult List()
        {
            ViewBag.showMainPanel = engine.GetUINodesForPanel(MAIN_PANEL_ID).Any();

            List<PanelNode> allPanels = engine.GetPanels();
            List<PanelNode> notEmptyPanels = new List<PanelNode>();
            foreach (var panel in allPanels)
            {
                if (engine.GetUINodesForPanel(panel.Id).Any())
                    notEmptyPanels.Add(panel);
            }

            return View(notEmptyPanels);
        }


        
        public ActionResult Chart(string id, string autoscroll, string style, string start, string end)
        {
            if (engine == null)
                return null;

            UiChartNode chart = engine.GetUINode(id) as UiChartNode;
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
