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
using MyNetSensors.LogicalNodesUI;
using MyNetSensors.SerialControllers;
using Newtonsoft.Json;
using Input = LiteGraph.Input;
using Node = MyNetSensors.Gateways.Node;
using Output = LiteGraph.Output;

namespace MyNetSensors.WebController.Controllers
{
    public class NodesEditorAPIController : Controller
    {
        const string MAIN_PANEL_ID = "Main";

        private LogicalNodesEngine engine = SerialController.logicalNodesEngine;

        public List<LiteGraph.Node> GetNodes(string panelId)
        {
            if (engine == null)
                return null;

            if (panelId == null)
                panelId = MAIN_PANEL_ID;

            List<LogicalNode> nodes = engine.nodes;
            if (nodes == null || !nodes.Any())
                return null;

            return (
                from node in nodes
                where node.PanelId == panelId
                select ConvertLogicalNodeToLitegraphNode(node)).ToList();
        }


        public LiteGraph.Node ConvertLogicalNodeToLitegraphNode(LogicalNode logicalNode)
        {
            LiteGraph.Node node = new LiteGraph.Node
            {
                title = logicalNode.Title,
                type = logicalNode.Type,
                id = logicalNode.Id,
                panel_id = logicalNode.PanelId
            };


            node.properties["ObjectType"] = logicalNode.GetType().ToString();

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

            if (logicalNode is LogicalNodeUI)
            {
                LogicalNodeUI n = (LogicalNodeUI)logicalNode;
                node.properties["Name"] = n.Name;
                node.properties["ShowOnMainPage"] = n.ShowOnMainPage ? "true" : "false";
            }

            if (logicalNode is LogicalNodeUISlider)
            {
                LogicalNodeUISlider n = (LogicalNodeUISlider) logicalNode;
                node.properties["Min"] = n.Min.ToString();
                node.properties["Max"] = n.Max.ToString();
            }

            if (logicalNode is LogicalNodeUIChart)
            {
                LogicalNodeUIChart n = (LogicalNodeUIChart)logicalNode;
                node.properties["State"] = n.State.ToString();
                node.properties["WriteInDatabase"] = n.WriteInDatabase ? "true" : "false";
                node.properties["WriteInDatabaseMinInterval"] = n.WriteInDatabaseMinInterval.ToString();
            }

            if (logicalNode is LogicalNodeConstant)
            {
                LogicalNodeConstant n = (LogicalNodeConstant)logicalNode;
                node.properties["Value"] = n.Value;
            }

            if (logicalNode is LogicalNodePanel)
            {
                LogicalNodePanel n = (LogicalNodePanel)logicalNode;
                node.properties["PanelName"] = n.Name;
            }

            if (logicalNode is LogicalNodePanelInput)
            {
                LogicalNodePanelInput n = (LogicalNodePanelInput)logicalNode;
                node.properties["Name"] = n.Name;
            }

            if (logicalNode is LogicalNodePanelOutput)
            {
                LogicalNodePanelOutput n = (LogicalNodePanelOutput)logicalNode;
                node.properties["Name"] = n.Name;
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
                id = logicalLink.Id,
                panel_id = logicalLink.PanelId
            };

            return link;
        }


        public List<LiteGraph.Link> GetLinks(string panelId)
        {
            if (engine == null)
                return null;

            List<LogicalLink> links = engine.links;
            if (links == null || !links.Any())
                return null;

            return (
                from link in links
                where link.PanelId == panelId
                select ConvertLogicalNodeToLitegraphLink(link)).ToList();
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



        public bool RemoveLink(Link link)
        {
            if (engine == null)
                return false;

            if (link.origin_id == null || link.target_id == null)
                return false;

            LogicalNode outNode = SerialController.logicalNodesEngine.GetNode(link.origin_id);
            LogicalNode inNode = SerialController.logicalNodesEngine.GetNode(link.target_id);
            if (outNode == null || inNode == null)
            {
                engine.LogEngineError($"Can`t remove link from [{link.origin_id}] to [{link.target_id}]. Does not exist.");
                return false;
            }
            engine.RemoveLink(outNode.Outputs[link.origin_slot], inNode.Inputs[link.target_slot]);

            return true;
        }

        public bool CreateLink(Link link)
        {
            if (engine == null)
                return false;

            LogicalNode outNode = SerialController.logicalNodesEngine.GetNode(link.origin_id);
            LogicalNode inNode = SerialController.logicalNodesEngine.GetNode(link.target_id);

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

            LogicalNode newNode;

            try
            {
                string type = node.properties["ObjectType"];
                string assemblyName = type.Split('.')[1];

                var newObject = Activator.CreateInstance(assemblyName, type);
                newNode = (LogicalNode) newObject.Unwrap();
            }
            catch
            {
                engine.LogEngineError($"Can`t create node [{node.properties["ObjectType"]}]. Type does not exist.");
                return false;
            }

            //LogicalNode newNode = newObject as LogicalNode;
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

            LogicalNode oldNode = engine.GetNode(node.id);
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

            LogicalNode oldNode = engine.GetNode(node.id);
            if (oldNode == null)
            {
                engine.LogEngineError($"Can`t update node [{node.id}]. Does not exist.");
                return false;
            }

            oldNode.Position = new Position { X = node.pos[0], Y = node.pos[1] };
            oldNode.Size = new Size { Width = node.size[0], Height = node.size[1] };

            engine.UpdateNode(oldNode);

            return true;
        }


        public bool PanelSettings(string id, string panelname)
        {
            LogicalNode n = engine.GetNode(id);
            if (n == null)
            {
                engine.LogEngineError($"Can`t set settings for Panel [{id}]. Does not exist.");
                return false;
            }

            LogicalNodePanel node = (LogicalNodePanel)n;
            node.Name = panelname;
            engine.UpdateNode(node);

            return true;
        }

        public bool InputOutputSettings(string id, string name)
        {
            LogicalNode n = engine.GetNode(id);
            if (n == null)
            {
                engine.LogEngineError($"Can`t set settings for Input/Output [{id}]. Does not exist.");
                return false;
            }

            if (n is LogicalNodePanelInput)
            {
                LogicalNodePanelInput node = (LogicalNodePanelInput)n;
                node.Name = name;
                engine.UpdateNode(node);
            }

            if (n is LogicalNodePanelOutput)
            {
                LogicalNodePanelOutput node = (LogicalNodePanelOutput)n;
                node.Name = name;
                engine.UpdateNode(node);
            }

            return true;
        }

        public bool UINodeSettings(string id, string name,bool show)
        {
            LogicalNode n = engine.GetNode(id);
            if (n == null)
            {
                engine.LogEngineError($"Can`t set settings for Node [{id}]. Does not exist.");
                return false;
            }

            LogicalNodeUI node = (LogicalNodeUI)n;
            node.Name = name;
            node.ShowOnMainPage = show;
            engine.UpdateNode(node);

            return true;
        }


        public bool UISliderSettings(string id, string name, int min,int max,bool show)
        {
            LogicalNode n = engine.GetNode(id);
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

            LogicalNodeUISlider node = (LogicalNodeUISlider)n;
            node.Name = name;
            node.Min = min;
            node.Max = max;
            node.ShowOnMainPage = show;
            engine.UpdateNode(node);

            return true;
        }

        public bool ConstantSettings(string id, string value)
        {
            LogicalNode n = engine.GetNode(id);
            if (n == null)
            {
                engine.LogEngineError($"Can`t set settings for Node [{id}]. Does not exist.");
                return false;
            }

            LogicalNodeConstant node = (LogicalNodeConstant)n;
            node.SetValue(value);
            engine.UpdateNode(node);

            return true;
        }


        public bool UIChartSettings(string id, string name, bool show, bool writeInDatabase, int writeInDatabaseMinInterval)
        {
            LogicalNode n = engine.GetNode(id);
            if (n == null)
            {
                engine.LogEngineError($"Can`t set settings for Node [{id}]. Does not exist.");
                return false;
            }

            LogicalNodeUIChart node = (LogicalNodeUIChart)n;
            node.Name = name;
            node.ShowOnMainPage = show;
            node.WriteInDatabase = writeInDatabase;
            node.WriteInDatabaseMinInterval = writeInDatabaseMinInterval;
            engine.UpdateNode(node);

            return true;
        }


        
    }
}
