using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MyNetSensors.Gateways;
using LiteGraph;
using MyNetSensors.LogicalNodes;
//using MyNetSensors.LogicalNodes;
using MyNetSensors.NodesLinks;
using MyNetSensors.SerialControllers;
using Newtonsoft.Json;
using Input = LiteGraph.Input;
using Node = MyNetSensors.Gateways.Node;
using Output = LiteGraph.Output;

namespace MyNetSensors.WebController.Controllers
{
    public class NodesEditorController : Controller
    {
        const int SLOT_SIZE = 15;
        const int NODE_WIDTH = 150;

        private LogicalNodesEngine engine = SerialController.logicalNodesEngine;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetNodes()
        {

            List<LogicalNode> nodes = engine.nodes;
            List<LiteGraph.Node> list = new List<LiteGraph.Node>();

            for (int i = 0; i < nodes.Count; i++)
            {
                LiteGraph.Node newNode = new LiteGraph.Node
                {
                    title = nodes[i].Title,
                    type = nodes[i].Type,
                    // pos = new[] { 100, 20 + i * 100 },
                    id = nodes[i].Id
                };

                if (nodes[i].Position != null)
                    newNode.pos = new[] { nodes[i].Position.X, nodes[i].Position.Y };

                newNode.inputs = new List<Input>();
                newNode.outputs = new List<Output>();

                if (nodes[i].Inputs != null)
                    for (int j = 0; j < nodes[i].Inputs.Count; j++)
                    {
                        newNode.inputs.Add(new Input
                        {
                            name = nodes[i].Inputs[j].Name,
                            type = "string"
                        });
                    }

                if (nodes[i].Outputs != null)
                    for (int j = 0; j < nodes[i].Outputs.Count; j++)
                    {
                        newNode.outputs.Add(new Output
                        {
                            name = nodes[i].Outputs[j].Name,
                            type = "string"
                        });
                    }


                newNode.size = new[] { NODE_WIDTH, CalculateSizeY(newNode) };

                list.Add(newNode);
            }

            MooveNewNodesToFreeSpace(list);

            return Json(list);
        }


        private int CalculateSizeY(LiteGraph.Node node)
        {
            int sizeOutY = 0, sizeInY = 0;

            if (node.outputs != null)
                sizeOutY = SLOT_SIZE + (SLOT_SIZE * node.outputs.Count);
            if (node.inputs != null)
                sizeInY = SLOT_SIZE + (SLOT_SIZE * node.inputs.Count);

            return (sizeOutY > sizeInY) ? sizeOutY : sizeInY;
        }



        private void MooveNewNodesToFreeSpace(List<LiteGraph.Node> nodes)
        {
            const int START_POS = 50;
            const int FREE_SPACE_UNDER = 30;

            for (int k = 0; k < nodes.Count; k++)
            {
                if (nodes[k].pos != null)
                    continue;

                nodes[k].pos = new int[2];

                int result = START_POS;


                for (int i = 0; i < nodes.Count; i++)
                {
                    int needFromY = result;
                    int needToY = result + nodes[k].size[1];

                    if (i == k)
                        continue;

                    if (nodes[i].pos == null)
                        continue;

                    if (nodes[i].pos[0] > NODE_WIDTH + 20 + START_POS)
                        continue;

                    int occupyFromY = nodes[i].pos[1]- FREE_SPACE_UNDER;
                    int occupyToY = nodes[i].pos[1] + nodes[i].size[1];

                    if (occupyFromY <= needToY && occupyToY >= needFromY)
                    {
                        result = occupyToY + FREE_SPACE_UNDER;
                        i = -1;
                    }
                }

                nodes[k].pos[0] = START_POS;
                nodes[k].pos[1] = result;
            }
        }

        //public IActionResult GetGraph()
        //{
        //    string json = "{ \"iteration\":0,\"last_node_id\":3,\"last_link_id\":2,\"links\":{ \"0\":{ \"id\":0,\"origin_id\":2,\"origin_slot\":0,\"target_id\":0,\"target_slot\":0,\"data\":null},\"1\":{ \"id\":1,\"origin_id\":0,\"origin_slot\":0,\"target_id\":1,\"target_slot\":0,\"data\":null} },\"config\":{ },\"nodes\":[{\"id\":0,\"title\":\"SimpleNode\",\"type\":\"Nodes/SimpleNode\",\"pos\":[344,234],\"size\":[100,20],\"flags\":{},\"inputs\":[{\"name\":\"in\",\"type\":\"number\",\"link\":0}],\"outputs\":[{\"name\":\"out\",\"type\":\"number\",\"links\":[1]}],\"properties\":{\"min\":0,\"max\":1}},{\"id\":2,\"title\":\"SimpleOut\",\"type\":\"Nodes/SimpleOut\",\"pos\":[211,282],\"size\":[100,20],\"flags\":{},\"outputs\":[{\"name\":\"out\",\"type\":\"number\",\"links\":[0]}],\"properties\":{\"min\":0,\"max\":1}},{\"id\":1,\"title\":\"SimpleIn\",\"type\":\"Nodes/SimpleIn\",\"pos\":[471,160],\"size\":[100,20],\"flags\":{},\"inputs\":[{\"name\":\"in\",\"type\":\"number\",\"link\":1}],\"properties\":{\"min\":0,\"max\":1}}]}";
        //    Graph graph = JsonConvert.DeserializeObject<Graph>(json);


        //    return Json(graph);
        //}

        public IActionResult PutGraph(string json)
        {
            Graph graph = JsonConvert.DeserializeObject<Graph>(json);

            //List<Node> nodes = SerialController.gateway.GetNodes();

            for (int i = 0; i < graph.links.Count; i++)
            {
                Link link = graph.links[i];
                Node outNode = SerialController.gateway.GetNode(link.origin_id);
                Node inNode = SerialController.gateway.GetNode(link.target_id);
                Sensor outSensor = outNode.sensors[link.origin_slot];
                Sensor inSensor = inNode.sensors[link.target_slot];





                SensorLink sensorLink = new SensorLink()
                {
                    fromNodeId = outNode.nodeId,
                    fromSensorId = outSensor.sensorId,
                    fromDataType = SensorDataType.V_RGB,
                    fromSensorDbId = outSensor.db_Id,
                    fromSensorDescription = $"{outNode.GetSimpleName1()} {outSensor.GetSimpleName1()}",
                    toNodeId = inNode.nodeId,
                    toSensorId = inSensor.sensorId,
                    toDataType = SensorDataType.V_RGB,
                    toSensorDbId = inSensor.db_Id,
                    toSensorDescription = $"{inNode.GetSimpleName1()} {inSensor.GetSimpleName1()}"
                };
                SerialController.sensorsLinksDb.AddLink(sensorLink);
            }

            GatewayAPIController gatewayApi = new GatewayAPIController();
            gatewayApi.UpdateSensorsLinks();



            return Json(true);
        }
    }
}
