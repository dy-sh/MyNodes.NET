using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MyNetSensors.Gateways;
using LiteGraph;
using MyNetSensors.LogicalNodes;
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

        //private LogicalNodesEngine engine = SerialController.logicalNodesEngine;

        public IActionResult Index()
        {
            return View();
        }

    }
}
