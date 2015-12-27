﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public class NodesEditorAPIController : Controller
    {
        const int SLOT_SIZE = 15;
        const int NODE_WIDTH = 150;

        private LogicalNodesEngine engine = SerialController.logicalNodesEngine;

        public List<LiteGraph.Node> GetNodes()
        {

            List<LogicalNode> nodes = engine.nodes;
            if (!nodes.Any())
                return null;

            List<LiteGraph.Node> list = new List<LiteGraph.Node>();

            for (int i = 0; i < nodes.Count; i++)
            {
                LiteGraph.Node node = new LiteGraph.Node
                {
                    title = nodes[i].Title,
                    type = nodes[i].Type,
                    // pos = new[] { 100, 20 + i * 100 },
                    id = nodes[i].Id
                };


                node.properties["objectType"] = nodes[i].GetType().ToString();

                if (nodes[i].Position != null)
                    node.pos = new[] { nodes[i].Position.X, nodes[i].Position.Y };

                node.inputs = new List<Input>();
                node.outputs = new List<Output>();

                if (nodes[i].Inputs != null)
                    for (int j = 0; j < nodes[i].Inputs.Count; j++)
                    {
                        node.inputs.Add(new Input
                        {
                            name = nodes[i].Inputs[j].Name,
                            type = "string"
                        });
                    }

                if (nodes[i].Outputs != null)
                    for (int j = 0; j < nodes[i].Outputs.Count; j++)
                    {
                        node.outputs.Add(new Output
                        {
                            name = nodes[i].Outputs[j].Name,
                            type = "string"
                        });
                    }


                node.size = new[] { NODE_WIDTH, CalculateSizeY(node) };

                list.Add(node);
            }

            MooveNewNodesToFreeSpace(list);

            return list;
        }




        public List<LiteGraph.Link> GetLinks()
        {

            List<LogicalLink> links = engine.links;
            if (!links.Any())
                return null;

            List< LiteGraph.Link > list=new List<Link>();

            for (int i = 0; i < links.Count; i++)
            {
                LiteGraph.Link link = new LiteGraph.Link
                {
                    origin_id = engine.GetOutputOwner(links[i].OutputId).Id,
                    target_id = engine.GetInputOwner(links[i].InputId).Id,
                    origin_slot = GetOutputSlot(links[i].OutputId),
                    target_slot = GetInputSlot(links[i].InputId),
                };
                
                list.Add(link);
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

                nodes[k].pos = new float[2];

                float result = START_POS;


                for (int i = 0; i < nodes.Count; i++)
                {
                    float needFromY = result;
                    float needToY = result + nodes[k].size[1];

                    if (i == k)
                        continue;

                    if (nodes[i].pos == null)
                        continue;

                    if (nodes[i].pos[0] > NODE_WIDTH + 20 + START_POS)
                        continue;

                    float occupyFromY = nodes[i].pos[1]- FREE_SPACE_UNDER;
                    float occupyToY = nodes[i].pos[1] + nodes[i].size[1];

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

        public bool PutGraph(string json)
        {
            Graph graph = JsonConvert.DeserializeObject<Graph>(json);

            engine.RemoveAllLinks();
            engine.RemoveAllNonHardwareNodes();

            foreach (var node in graph.nodes)
            {
                string type = node.properties["objectType"];

                if (type == "MyNetSensors.LogicalNodes.LogicalNodeMySensors")
                {
                    LogicalNode oldNode=engine.GetNode(node.id);
                    oldNode.Position= new Position { X = node.pos[0], Y = node.pos[1] };
                }
                else
                {
                    var newObject = Activator.CreateInstance("LogicalNodes", type);
                    LogicalNode newNode = (LogicalNode) newObject.Unwrap();

                    //LogicalNode newNode = newObject as LogicalNode;
                    newNode.Position = new Position {X = node.pos[0], Y = node.pos[1]};
                    newNode.Size = new Size {Width = node.size[0], Height = node.size[1]};
                    newNode.Id = node.id;

                    engine.AddNode(newNode);
                }
            }

            foreach (Link link in graph.links.Values)
            {
                LogicalNode outNode = SerialController.logicalNodesEngine.GetNode(link.origin_id);
                LogicalNode inNode = SerialController.logicalNodesEngine.GetNode(link.target_id);
                engine.AddLink(outNode.Outputs[link.origin_slot], inNode.Inputs[link.target_slot]);
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
    }
}