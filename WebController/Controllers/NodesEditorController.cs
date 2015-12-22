using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MyNetSensors.NodesEditor;
using Newtonsoft.Json;

namespace MyNetSensors.WebController.Controllers
{
    public class NodesEditorController:Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetNodes()
        {
            List<Node> list = new List<Node>
            {
                new Node
                {
                    pos = new []{100,100},
                    type = "Nodes/SimpleNode"
                },
                new Node
                {
                    pos = new []{200,200},
                    type =  "Nodes/SimpleIn"
                },
                new Node
                {
                    pos = new []{300,300},
                    type = "Nodes/SimpleOut"
                },
            };

            return Json(list);
        }

        public IActionResult GetGraph()
        {
            string json = "{ \"iteration\":0,\"last_node_id\":3,\"last_link_id\":2,\"links\":{ \"0\":{ \"id\":0,\"origin_id\":2,\"origin_slot\":0,\"target_id\":0,\"target_slot\":0,\"data\":null},\"1\":{ \"id\":1,\"origin_id\":0,\"origin_slot\":0,\"target_id\":1,\"target_slot\":0,\"data\":null} },\"config\":{ },\"nodes\":[{\"id\":0,\"title\":\"SimpleNode\",\"type\":\"Nodes/SimpleNode\",\"pos\":[344,234],\"size\":[100,20],\"flags\":{},\"inputs\":[{\"name\":\"in\",\"type\":\"number\",\"link\":0}],\"outputs\":[{\"name\":\"out\",\"type\":\"number\",\"links\":[1]}],\"properties\":{\"min\":0,\"max\":1}},{\"id\":2,\"title\":\"SimpleOut\",\"type\":\"Nodes/SimpleOut\",\"pos\":[211,282],\"size\":[100,20],\"flags\":{},\"outputs\":[{\"name\":\"out\",\"type\":\"number\",\"links\":[0]}],\"properties\":{\"min\":0,\"max\":1}},{\"id\":1,\"title\":\"SimpleIn\",\"type\":\"Nodes/SimpleIn\",\"pos\":[471,160],\"size\":[100,20],\"flags\":{},\"inputs\":[{\"name\":\"in\",\"type\":\"number\",\"link\":1}],\"properties\":{\"min\":0,\"max\":1}}]}";
            Graph graph = JsonConvert.DeserializeObject<Graph>(json);


            return Json(graph);
        }

        public IActionResult PutGraph(string json)
        {
            Graph graph = JsonConvert.DeserializeObject<Graph>(json);
            return null;
        }
    }
}
