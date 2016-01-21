using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MyNetSensors.Gateways;
using LiteGraph;
using MyNetSensors.Nodes;
using MyNetSensors.WebController.Code;
using Newtonsoft.Json;
using Link = MyNetSensors.Nodes.Link;
using Node = MyNetSensors.Nodes.Node;

namespace MyNetSensors.WebController.Controllers
{
    public class NodesEditorAPIController : Controller
    {
        const string MAIN_PANEL_ID = "Main";

        private NodesEngine engine = SystemController.nodesEngine;
        private UiNodesEngine uiEngine = SystemController.uiNodesEngine;


        public List<LiteGraph.Node> GetNodes(string panelId)
        {
            if (engine == null)
                return null;

            if (panelId == null)
                panelId = MAIN_PANEL_ID;

            List<Node> nodes = engine.nodes;
            if (nodes == null || !nodes.Any())
                return null;

            return (
                from node in nodes
                where node.PanelId == panelId
                select ConvertNodeToLiteGraphNode(node)).ToList();
        }


        public LiteGraph.Node ConvertNodeToLiteGraphNode(Node node)
        {
            LiteGraph.Node litegraphNode = new LiteGraph.Node
            {
                title = node.Title,
                type = node.Type,
                id = node.Id,
                panel_id = node.PanelId
            };


            litegraphNode.properties["ObjectType"] = node.GetType().ToString();

            if (node.Position != null)
                litegraphNode.pos = new[] { node.Position.X, node.Position.Y };

            if (node.Size != null)
                litegraphNode.size = new[] { node.Size.Width, node.Size.Height };

            litegraphNode.inputs = new List<LiteGraph.Input>();
            litegraphNode.outputs = new List<LiteGraph.Output>();

            if (node.Inputs != null)
                foreach (var input in node.Inputs)
                {
                    litegraphNode.inputs.Add(new LiteGraph.Input
                    {
                        name = input.Name,
                        type = "string",
                        link = engine.GetLinkForInput(input)?.Id
                    });
                }


            if (node.Outputs != null)
                foreach (var output in node.Outputs)
                {
                    List<Link> links = engine.GetLinksForOutput(output);
                    if (links != null)
                    {
                        string[] linksIds = new string[links.Count];
                        for (int i = 0; i < links.Count; i++)
                        {
                            linksIds[i] = links[i].Id;
                        }
                        litegraphNode.outputs.Add(new LiteGraph.Output
                        {
                            name = output.Name,
                            type = "string",
                            links = linksIds
                        });
                    }
                    else
                    {
                        litegraphNode.outputs.Add(new LiteGraph.Output
                        {
                            name = output.Name,
                            type = "string"
                        });
                    }
                }

            if (node is UiNode)
            {
                UiNode n = (UiNode)node;
                litegraphNode.properties["Name"] = n.Name;
                litegraphNode.properties["ShowOnMainPage"] = n.ShowOnMainPage ? "true" : "false";
            }

            if (node is UiSliderNode)
            {
                UiSliderNode n = (UiSliderNode) node;
                litegraphNode.properties["Min"] = n.Min.ToString();
                litegraphNode.properties["Max"] = n.Max.ToString();
            }

            if (node is UiChartNode)
            {
                UiChartNode n = (UiChartNode)node;
                litegraphNode.properties["State"] = n.State.ToString();
                litegraphNode.properties["WriteInDatabase"] = n.WriteInDatabase ? "true" : "false";
                litegraphNode.properties["UpdateInterval"] = n.UpdateInterval.ToString();
            }

            if (node is ConstantNode)
            {
                ConstantNode n = (ConstantNode)node;
                litegraphNode.properties["Value"] = n.Value;
            }

            if (node is PanelNode)
            {
                PanelNode n = (PanelNode)node;
                litegraphNode.properties["PanelName"] = n.Name;
            }

            if (node is PanelInputNode)
            {
                PanelInputNode n = (PanelInputNode)node;
                litegraphNode.properties["Name"] = n.Name;
            }

            if (node is PanelOutputNode)
            {
                PanelOutputNode n = (PanelOutputNode)node;
                litegraphNode.properties["Name"] = n.Name;
            }

            return litegraphNode;
        }


        public LiteGraph.Link ConvertLinkToLiteGraphLink(Link link)
        {
            if (link == null)
                return null;
            LiteGraph.Link liteGraphLink = new LiteGraph.Link
            {
                origin_id = engine.GetOutputOwner(link.OutputId).Id,
                target_id = engine.GetInputOwner(link.InputId).Id,
                origin_slot = GetOutputSlot(link.OutputId),
                target_slot = GetInputSlot(link.InputId),
                id = link.Id,
                panel_id = link.PanelId
            };

            return liteGraphLink;
        }


        public List<LiteGraph.Link> GetLinks(string panelId)
        {
            if (engine == null)
                return null;

            List<Link> links = engine.links;
            if (links == null || !links.Any())
                return null;

            return (
                from link in links
                where link.PanelId == panelId
                select ConvertLinkToLiteGraphLink(link)).ToList();
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



        public bool RemoveLink(LiteGraph.Link link)
        {
            if (engine == null)
                return false;

            if (link.origin_id == null || link.target_id == null)
                return false;

            Node outNode = SystemController.nodesEngine.GetNode(link.origin_id);
            Node inNode = SystemController.nodesEngine.GetNode(link.target_id);
            if (outNode == null || inNode == null)
            {
                engine.LogEngineError($"Can`t remove link from [{link.origin_id}] to [{link.target_id}]. Does not exist.");
                return false;
            }
            engine.RemoveLink(outNode.Outputs[link.origin_slot], inNode.Inputs[link.target_slot]);

            return true;
        }

        public bool CreateLink(LiteGraph.Link link)
        {
            if (engine == null)
                return false;

            Node outNode = SystemController.nodesEngine.GetNode(link.origin_id);
            Node inNode = SystemController.nodesEngine.GetNode(link.target_id);

            if (outNode == null || inNode == null)
            {
                engine.LogEngineError($"Can`t create link from [{link.origin_id}] to [{link.target_id}]. Does not exist.");
                return false;
            }

            engine.AddLink(outNode.Outputs[link.origin_slot], inNode.Inputs[link.target_slot]);
            return true;
        }

        public bool CreateNode(LiteGraph.Node node)
        {
            if (engine == null)
                return false;

            Node newNode;

            try
            {
                string type = node.properties["ObjectType"];
                string assemblyName = node.properties["Assembly"];

                var newObject = Activator.CreateInstance(assemblyName, type);
                newNode = (Node) newObject.Unwrap();
            }
            catch
            {
                engine.LogEngineError($"Can`t create node [{node.properties["ObjectType"]}]. Type does not exist.");
                return false;
            }

            //Node newNode = newObject as HardwareNode;
            newNode.Position = new Position { X = node.pos[0], Y = node.pos[1] };
            if (node.size.Length==2)
                newNode.Size = new Size { Width = node.size[0], Height = node.size[1] };
            newNode.Id = node.id;
            newNode.PanelId = node.panel_id ?? MAIN_PANEL_ID;

            engine.AddNode(newNode);

            return true;
        }

        public bool RemoveNode(LiteGraph.Node node)
        {
            if (engine == null)
                return false;

            Node oldNode = engine.GetNode(node.id);
            if (oldNode == null)
            {
                engine.LogEngineError($"Can`t remove node [{node.id}]. Does not exist.");
                return false;
            }

            engine.RemoveNode(oldNode);
            return true;
        }

        public bool UpdateNode(LiteGraph.Node node)
        {
            if (engine == null)
                return false;

            Node oldNode = engine.GetNode(node.id);
            if (oldNode == null)
            {
                engine.LogEngineError($"Can`t update node [{node.id}]. Does not exist.");
                return false;
            }

            oldNode.Position = new Position { X = node.pos[0], Y = node.pos[1] };
            oldNode.Size = new Size { Width = node.size[0], Height = node.size[1] };

            engine.UpdateNode(oldNode, true);

            return true;
        }


        public bool PanelSettings(string id, string panelname)
        {
            Node n = engine.GetNode(id);
            if (n == null)
            {
                engine.LogEngineError($"Can`t set settings for Panel [{id}]. Does not exist.");
                return false;
            }

            PanelNode node = (PanelNode)n;
            node.Name = panelname;
            engine.UpdateNode(node, true);

            return true;
        }

        public bool InputOutputSettings(string id, string name)
        {
            Node n = engine.GetNode(id);
            if (n == null)
            {
                engine.LogEngineError($"Can`t set settings for Input/Output [{id}]. Does not exist.");
                return false;
            }

            if (n is PanelInputNode)
            {
                PanelInputNode node = (PanelInputNode)n;
                node.Name = name;
                engine.UpdateNode(node, true);
            }

            if (n is PanelOutputNode)
            {
                PanelOutputNode node = (PanelOutputNode)n;
                node.Name = name;
                engine.UpdateNode(node, true);
            }

            return true;
        }

        public bool UINodeSettings(string id, string name,bool show)
        {
            Node n = engine.GetNode(id);
            if (n == null)
            {
                engine.LogEngineError($"Can`t set settings for Node [{id}]. Does not exist.");
                return false;
            }

            UiNode uiNode = (UiNode)n;
            uiNode.Name = name;
            uiNode.ShowOnMainPage = show;
            engine.UpdateNode(uiNode, true);

            return true;
        }


        public bool UISliderSettings(string id, string name, int min,int max,bool show)
        {
            Node n = engine.GetNode(id);
            if (n == null)
            {
                engine.LogEngineError($"Can`t set settings for Node [{id}]. Does not exist.");
                return false;
            }
            if (min >= max)
            {
                engine.LogEngineError($"Can`t set settings for Node [{id}]. Min must be > Max.");
                return false;
            }

            UiSliderNode uiSliderNode = (UiSliderNode)n;
            uiSliderNode.Name = name;
            uiSliderNode.Min = min;
            uiSliderNode.Max = max;
            uiSliderNode.ShowOnMainPage = show;
            engine.UpdateNode(uiSliderNode, true);

            return true;
        }

        public bool ConstantSettings(string id, string value)
        {
            Node n = engine.GetNode(id);
            if (n == null)
            {
                engine.LogEngineError($"Can`t set settings for Node [{id}]. Does not exist.");
                return false;
            }

            ConstantNode node = (ConstantNode)n;
            node.SetValue(value);
            engine.UpdateNode(node, true);

            return true;
        }


        public bool UIChartSettings(string id, string name, bool show, bool writeInDatabase, int updateInterval)
        {
            Node n = engine.GetNode(id);
            if (n == null)
            {
                engine.LogEngineError($"Can`t set settings for Node [{id}]. Does not exist.");
                return false;
            }

            UiChartNode uiNode = (UiChartNode)n;

            if (uiNode.WriteInDatabase && !writeInDatabase)
                uiEngine.ClearChart(uiNode.Id);

            uiNode.Name = name;
            uiNode.ShowOnMainPage = show;
            uiNode.WriteInDatabase = writeInDatabase;
            uiNode.UpdateInterval = updateInterval;
            engine.UpdateNode(uiNode, true);


            return true;
        }


        
    }
}
