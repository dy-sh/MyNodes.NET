using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MyNetSensors.Nodes;
using MyNetSensors.WebController.Code;

namespace MyNetSensors.WebController.Controllers
{
    public class DashboardAPIController : Controller
    {

        private UiNodesEngine engine = SystemController.uiNodesEngine;


        public async Task<string> GetNameForPanel(string id)
        {
            return await Task.Run(() =>
            {
                if (engine == null)
                    return null;

                PanelNode panel = engine.GetPanel(id);

                return panel?.Name;
            });
        }

        public async Task<List<UiNode>> GetUINodesForMainPage()
        {
            return await Task.Run(() =>
            {
                if (engine == null)
                    return null;

                return engine.GetUINodesForMainPage();
            });
        }

        public async Task<List<UiNode>> GetUINodesForPanel(string panelId)
        {
            return await Task.Run(() =>
            {
                if (engine == null)
                    return null;

                return engine.GetUINodesForPanel(panelId);
            });
        }


        public bool ClearLog(string nodeId)
        {
            engine.ClearLog(nodeId);
            return true;
        }


        public bool TextBoxSend(string nodeId, string value)
        {
            engine.TextBoxSend(nodeId, value);
            return true;
        }



        public bool ButtonClick(string nodeId)
        {
            engine.ButtonClick(nodeId);
            return true;
        }

        public bool ToggleButtonClick(string nodeId)
        {
            engine.ToggleButtonClick(nodeId);
            return true;
        }

        public bool SwitchClick(string nodeId)
        {
            engine.SwitchClick(nodeId);
            return true;
        }

        public async Task<bool> SliderChange(string nodeId, int value)
        {
            return await Task.Run(() =>
            {
                engine.SliderChange(nodeId, value);
                return true;
            });
        }

        public async Task<bool> RGBSlidersChange(string nodeId, string value)
        {
            return await Task.Run(() =>
            {
                engine.RGBSlidersChange(nodeId, value);
                return true;
            });
        }

        public async Task<bool> RGBWSlidersChange(string nodeId, string value)
        {
            return await Task.Run(() =>
            {
                engine.RGBWSlidersChange(nodeId, value);
                return true;
            });
        }




        public async Task<List<ChartData>> GetChartData(string id)
        {
            return await Task.Run(() =>
            {
                UiChartNode chart = engine.GetUINode(id) as UiChartNode;
                if (chart == null)
                    return null;

                List<NodeState> nodeStates = chart.GetStates();

                if (nodeStates == null || !nodeStates.Any())
                    return null;

                //copy to array to prevent changing data error
                NodeState[] nodeStatesArray = new NodeState[nodeStates.Count];
                nodeStates.CopyTo(nodeStatesArray);

                return nodeStatesArray.Select(item => new ChartData
                {
                    x = $"{item.DateTime:yyyy-MM-dd HH:mm:ss.fff}",
                    y = item.State == "0" ? "-0.01" : item.State
                }).ToList();
            });
        }


        public bool ClearChart(string nodeId)
        {
            engine.ClearChart(nodeId);
            return true;
        }
    }
}
