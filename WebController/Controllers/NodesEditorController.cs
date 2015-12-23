using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MyNetSensors.Gateway;
using MyNetSensors.NodesEditor;
using MyNetSensors.NodesLinks;
using MyNetSensors.SerialControl;
using Newtonsoft.Json;
using Node = MyNetSensors.NodesEditor.Node;

namespace MyNetSensors.WebController.Controllers
{
    public class NodesEditorController : Controller
    {
        const int slotSize = 15;
        const int nodeWidth = 150;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetNodes()
        {
            List<Gateway.Node> nodes = SerialController.gateway.GetNodes();
            List<Node> list = new List<Node>();

            for (int i = 0; i < nodes.Count; i++)
            {
                Node newNode = new Node
                {
                    title = nodes[i].name,
                    type = "Nodes/SimpleNode",
                    pos = new[] { 100, 20 + i * 100 },
                    id = nodes[i].nodeId
                };

                newNode.inputs=new List<Input>();
                newNode.outputs=new List<Output>();

                if (nodes[i].sensors != null)
                    for (int j = 0; j < nodes[i].sensors.Count; j++)
                    {
                        newNode.outputs.Add(new Output
                        {
                            name = nodes[i].sensors[j].GetSimpleName1(),
                            type = "number"
                        });
                        newNode.inputs.Add(new Input
                        {
                            name = nodes[i].sensors[j].GetSimpleName1(),
                            type = "number"
                        });
                    }


                int sizeOutY = 0, sizeInY = 0;

                

                if (newNode.outputs != null)
                    sizeOutY = slotSize + (slotSize * newNode.outputs.Count);
                if (newNode.inputs != null)
                    sizeInY = slotSize + (slotSize * newNode.inputs.Count);

                int sizeY = (sizeOutY > sizeInY) ? sizeOutY : sizeInY;
                newNode.size = new[] { nodeWidth, sizeY };

                list.Add(newNode);
            }


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

            //List<Gateway.Node> nodes = SerialController.gateway.GetNodes();

            for (int i = 0; i < graph.links.Count; i++)
            {
                Link link = graph.links[i];
                Gateway.Node outNode =  SerialController.gateway.GetNode(link.origin_id);
                Gateway.Node inNode = SerialController.gateway.GetNode(link.target_id);
                Gateway.Sensor outSensor = outNode.sensors[link.origin_slot];
                Gateway.Sensor inSensor = inNode.sensors[link.target_slot];





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


    public class X
    {
        public int a;
        public int b;
    }
}
