using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MyNetSensors.Gateways;
using LiteGraph;
using MyNetSensors.LogicalNodes;
using MyNetSensors.LogicalNodesMySensors;
using MyNetSensors.SerialControllers;
using Newtonsoft.Json;
using Input = LiteGraph.Input;
using Node = MyNetSensors.Gateways.Node;
using Output = LiteGraph.Output;

namespace MyNetSensors.WebController.Controllers
{
    public class NodesEditorAPIController : Controller
    {
        const int SLOT_SIZE = 15;
        const int NODE_WIDTH = 150;

        private LogicalNodesEngine engine = SerialController.logicalNodesEngine;
        private LogicalHardwareNodesEngine hardwareEngine = LogicalHardwareNodesEngine.logicalHardwareNodesEngine;

        public List<LiteGraph.Node> GetNodes()
        {
            List<LogicalNode> nodes = engine.nodes;
            if (!nodes.Any())
                return null;

            List<LiteGraph.Node> list = new List<LiteGraph.Node>();

            foreach (var node in nodes)
            {
                LiteGraph.Node newNode = ConvertLogicalNodeToLitegraphNode(node);
                list.Add(newNode);
            }

            return list;
        }


        public LiteGraph.Node ConvertLogicalNodeToLitegraphNode(LogicalNode logicalNode)
        {
            LiteGraph.Node node = new LiteGraph.Node
            {
                title = logicalNode.Title,
                type = logicalNode.Type,
                id = logicalNode.Id
            };


            node.properties["objectType"] = logicalNode.GetType().ToString();

            if (logicalNode.Position != null)
                node.pos = new[] { logicalNode.Position.X, logicalNode.Position.Y };

            if (logicalNode.Size != null)
                node.size = new[] { logicalNode.Size.Width, logicalNode.Size.Height };

            node.inputs = new List<Input>();
            node.outputs = new List<Output>();

            if (logicalNode.Inputs != null)
                foreach (var input in logicalNode.Inputs)
                {
                    node.inputs.Add(new Input
                    {
                        name = input.Name,
                        type = "string",
                        link = engine.GetLinkForInput(input)?.Id
                    });
                }
 

            if (logicalNode.Outputs != null)
                foreach (var output in logicalNode.Outputs)
                {
                    List<LogicalLink> links = engine.GetLinksForOutput(output);
                    if (links != null)
                    {
                        string[] linksIds = new string[links.Count];
                        for (int i = 0; i < links.Count; i++)
                        {
                            linksIds[i] = links[i].Id;
                        }
                        node.outputs.Add(new Output
                        {
                            name = output.Name,
                            type = "string",
                            links = linksIds
                        });
                    }
                    else
                    {
                        node.outputs.Add(new Output
                        {
                            name = output.Name,
                            type = "string"
                        });
                    }
                }

            return node;
        }


        public Link ConvertLogicalNodeToLitegraphLink(LogicalLink logicalLink)
        {
            if (logicalLink == null)
                return null;
            LiteGraph.Link link = new LiteGraph.Link
            {
                origin_id = engine.GetOutputOwner(logicalLink.OutputId).Id,
                target_id = engine.GetInputOwner(logicalLink.InputId).Id,
                origin_slot = GetOutputSlot(logicalLink.OutputId),
                target_slot = GetInputSlot(logicalLink.InputId),
                id = logicalLink.Id
            };

            return link;
        }


        public List<LiteGraph.Link> GetLinks()
        {

            List<LogicalLink> links = engine.links;
            if (!links.Any())
                return null;

            List<LiteGraph.Link> list = new List<Link>();

            foreach (var link in links)
            {
                list.Add(ConvertLogicalNodeToLitegraphLink(link));

            }

            return list;
        }


        private int GetInputSlot(string inputId)
        {
            for (int i = 0; i < engine.nodes.Count; i++)
            {
                for (int j = 0; j < engine.nodes[i].Inputs.Count; j++)
                {
                    if (engine.nodes[i].Inputs[j].Id == inputId)
                        return j;
                }
            }
            return -1;
        }

        private int GetOutputSlot(string outputId)
        {
            for (int i = 0; i < engine.nodes.Count; i++)
            {
                for (int j = 0; j < engine.nodes[i].Outputs.Count; j++)
                {
                    if (engine.nodes[i].Outputs[j].Id == outputId)
                        return j;
                }
            }
            return -1;
        }

        //private int CalculateNodeHeight(LiteGraph.Node node)
        //{
        //    int sizeOutY = 0, sizeInY = 0;

        //    if (node.outputs != null)
        //        sizeOutY = SLOT_SIZE + (SLOT_SIZE * node.outputs.Count);
        //    if (node.inputs != null)
        //        sizeInY = SLOT_SIZE + (SLOT_SIZE * node.inputs.Count);

        //    return (sizeOutY > sizeInY) ? sizeOutY : sizeInY;
        //}



        //private void MooveNewNodesToFreeSpace(List<LiteGraph.Node> nodes)
        //{
        //    const int START_POS = 50;
        //    const int FREE_SPACE_UNDER = 30;

        //    for (int k = 0; k < nodes.Count; k++)
        //    {
        //        if (nodes[k].pos != null)
        //            continue;

        //        nodes[k].pos = new float[2];

        //        float result = START_POS;


        //        for (int i = 0; i < nodes.Count; i++)
        //        {
        //            float needFromY = result;
        //            float needToY = result + nodes[k].size[1];

        //            if (i == k)
        //                continue;

        //            if (nodes[i].pos == null)
        //                continue;

        //            if (nodes[i].pos[0] > NODE_WIDTH + 20 + START_POS)
        //                continue;

        //            float occupyFromY = nodes[i].pos[1]- FREE_SPACE_UNDER;
        //            float occupyToY = nodes[i].pos[1] + nodes[i].size[1];

        //            if (occupyFromY <= needToY && occupyToY >= needFromY)
        //            {
        //                result = occupyToY + FREE_SPACE_UNDER;
        //                i = -1;
        //            }
        //        }

        //        nodes[k].pos[0] = START_POS;
        //        nodes[k].pos[1] = result;
        //    }
        //}

        //public IActionResult GetGraph()
        //{
        //    string json = "{ \"iteration\":0,\"last_node_id\":3,\"last_link_id\":2,\"links\":{ \"0\":{ \"id\":0,\"origin_id\":2,\"origin_slot\":0,\"target_id\":0,\"target_slot\":0,\"data\":null},\"1\":{ \"id\":1,\"origin_id\":0,\"origin_slot\":0,\"target_id\":1,\"target_slot\":0,\"data\":null} },\"config\":{ },\"nodes\":[{\"id\":0,\"title\":\"SimpleNode\",\"type\":\"Nodes/SimpleNode\",\"pos\":[344,234],\"size\":[100,20],\"flags\":{},\"inputs\":[{\"name\":\"in\",\"type\":\"number\",\"link\":0}],\"outputs\":[{\"name\":\"out\",\"type\":\"number\",\"links\":[1]}],\"properties\":{\"min\":0,\"max\":1}},{\"id\":2,\"title\":\"SimpleOut\",\"type\":\"Nodes/SimpleOut\",\"pos\":[211,282],\"size\":[100,20],\"flags\":{},\"outputs\":[{\"name\":\"out\",\"type\":\"number\",\"links\":[0]}],\"properties\":{\"min\":0,\"max\":1}},{\"id\":1,\"title\":\"SimpleIn\",\"type\":\"Nodes/SimpleIn\",\"pos\":[471,160],\"size\":[100,20],\"flags\":{},\"inputs\":[{\"name\":\"in\",\"type\":\"number\",\"link\":1}],\"properties\":{\"min\":0,\"max\":1}}]}";
        //    Graph graph = JsonConvert.DeserializeObject<Graph>(json);


        //    return Json(graph);
        //}

        public bool PutGraph(string json)
        {
            return false;
            Graph graph = JsonConvert.DeserializeObject<Graph>(json);

            engine.RemoveAllLinks();
            hardwareEngine.RemoveAllNonHardwareNodes();

            foreach (var node in graph.nodes)
            {
                string type = node.properties["objectType"];

                if (type == "MyNetSensors.LogicalNodes.LogicalHardwareNode")
                {
                    LogicalNode oldNode = engine.GetNode(node.id);
                    oldNode.Position = new Position { X = node.pos[0], Y = node.pos[1] };
                }
                else
                {
                    CreateNode(node);
                }
            }

            foreach (Link link in graph.links.Values)
            {
                CreateLink(link);
            }

            //for (int i = 0; i < graph.links.Count; i++)
            //{
            //    Link link = graph.links[i];

            //    LogicalNode outNode = SerialController.logicalNodesEngine.GetNode(link.origin_id);
            //    LogicalNode inNode = SerialController.logicalNodesEngine.GetNode(link.target_id);
            //    engine.AddLink(outNode.Outputs[link.origin_slot], inNode.Inputs[link.target_slot]);
            //}


            return true;
        }

        public bool DeleteLink(Link link)
        {
            if (link.origin_id == null || link.target_id == null)
                return false;

            LogicalNode outNode = SerialController.logicalNodesEngine.GetNode(link.origin_id);
            LogicalNode inNode = SerialController.logicalNodesEngine.GetNode(link.target_id);
            engine.DeleteLink(outNode.Outputs[link.origin_slot], inNode.Inputs[link.target_slot]);
            return true;
        }

        public bool CreateLink(Link link)
        {
            LogicalNode outNode = SerialController.logicalNodesEngine.GetNode(link.origin_id);
            LogicalNode inNode = SerialController.logicalNodesEngine.GetNode(link.target_id);
            engine.AddLink(outNode.Outputs[link.origin_slot], inNode.Inputs[link.target_slot]);
            return true;
        }

        public bool CreateNode(LiteGraph.Node node)
        {
            string type = node.properties["objectType"];

            var newObject = Activator.CreateInstance("LogicalNodes", type);
            LogicalNode newNode = (LogicalNode)newObject.Unwrap();

            //LogicalNode newNode = newObject as LogicalNode;
            newNode.Position = new Position { X = node.pos[0], Y = node.pos[1] };
            newNode.Size = new Size { Width = node.size[0], Height = node.size[1] };
            newNode.Id = node.id;

            engine.AddNode(newNode);
            return true;
        }

        public bool DeleteNode(LiteGraph.Node node)
        {
            LogicalNode oldNode = engine.GetNode(node.id);

            engine.RemoveNode(oldNode);
            return true;
        }

        public bool UpdateNode(LiteGraph.Node node)
        {
            LogicalNode oldNode = engine.GetNode(node.id);

            oldNode.Position = new Position { X = node.pos[0], Y = node.pos[1] };
            oldNode.Size = new Size { Width = node.size[0], Height = node.size[1] };

            engine.UpdateNode(oldNode);

            return true;
        }
    }
}
