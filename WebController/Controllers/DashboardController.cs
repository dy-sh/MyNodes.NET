/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using System.Linq;
using MyNodes.Nodes;
using MyNodes.Users;
using MyNodes.WebController.Code;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyNodes.WebController.Controllers
{
    [Authorize(UserClaims.DashboardObserver)]
    public class DashboardController : Controller
    {
        const string MAIN_PANEL_ID = "Main";
        const string HOME_PANEL_ID = "Home";

        private UiNodesEngine engine = SystemController.uiNodesEngine;


        public IActionResult Index()
        {
            ViewBag.panelId = HOME_PANEL_ID;

            return View();
        }



        public IActionResult Panel(string id)
        {
            if (engine == null)
                return BadRequest();

            if (id == null || id == MAIN_PANEL_ID)
            {
                id = MAIN_PANEL_ID;
                ViewBag.panelName = "Main Panel";
            }
            else
            {
                PanelNode panel = engine.GetPanel(id);
                if (panel == null)
                    return NotFound();
                ViewBag.panelName = panel.Settings["Name"].Value;
            }

            ViewBag.panelId = id;
            return View("Index");
        }

        public IActionResult List()
        {
            if (engine == null)
                return BadRequest();

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


        
        public IActionResult Chart(string id, string autoscroll, string style, string start, string end)
        {
            if (engine == null)
                return BadRequest();

            UiChartNode node = engine.GetUINode(id) as UiChartNode;
            if (node == null)
                return new NotFoundResult();

            ViewBag.autoscroll = autoscroll;

            if (style == null)
                style = node.Style;

            ViewBag.style = style;
            ViewBag.start = start ?? "0";
            ViewBag.end = end ?? "0";
            ViewBag.panelId = node.PanelId;

            return View(node);
        }
    }
}
