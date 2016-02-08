using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MyNetSensors.Gateways;
using LiteGraph;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using MyNetSensors.Nodes;
using MyNetSensors.Users;
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



        [Authorize(UserClaims.EditorObserver)]
        public async Task<List<LiteGraph.Node>> GetNodes(string panelId)
        {
            return await Task.Run(() =>
            {
                if (engine == null)
                    return null;

                if (panelId == null)
                    panelId = MAIN_PANEL_ID;

                List<Node> nodes = engine.GetNodes();
                if (nodes == null || !nodes.Any())
                    return null;

                return (
                    from node in nodes
                    where node.PanelId == panelId
                    select ConvertNodeToLiteGraphNode(node)).ToList();
            });
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
                litegraphNode.pos = new[] {node.Position.X, node.Position.Y};

            if (node.Size != null)
                litegraphNode.size = new[] {node.Size.Width, node.Size.Height};

            litegraphNode.inputs = new List<LiteGraph.Input>();
            litegraphNode.outputs = new List<LiteGraph.Output>();


            if (node.Inputs != null)
            {
                List<Nodes.Input> orderedInputs = node.Inputs.OrderBy(x => x.SlotIndex).ToList();
                foreach (var input in orderedInputs)
                {
                    litegraphNode.inputs.Add(new LiteGraph.Input
                    {
                        name = input.Name,
                        type = "string",
                        link = engine.GetLinkForInput(input)?.Id
                    });
                }
            }

            if (node.Outputs != null)
            {
                List<Nodes.Output> orderedOutputs = node.Outputs.OrderBy(x => x.SlotIndex).ToList();
                foreach (var output in orderedOutputs)
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
            }

            if (node is UiNode)
            {
                UiNode n = (UiNode) node;
                litegraphNode.properties["Name"] = n.Name;
                litegraphNode.properties["PanelIndex"] = n.PanelIndex.ToString();
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
                UiChartNode n = (UiChartNode) node;
                litegraphNode.properties["State"] = n.State.ToString();
                litegraphNode.properties["WriteInDatabase"] = n.WriteInDatabase ? "true" : "false";
                litegraphNode.properties["UpdateInterval"] = n.UpdateInterval.ToString();
            }

            if (node is ConstantNode)
            {
                ConstantNode n = (ConstantNode) node;
                litegraphNode.properties["Value"] = n.Value;
            }

            if (node is PanelNode)
            {
                PanelNode n = (PanelNode) node;
                litegraphNode.properties["PanelName"] = n.Name;
            }

            if (node is PanelInputNode)
            {
                PanelInputNode n = (PanelInputNode) node;
                litegraphNode.properties["Name"] = n.Name;
            }

            if (node is PanelOutputNode)
            {
                PanelOutputNode n = (PanelOutputNode) node;
                litegraphNode.properties["Name"] = n.Name;
            }

            if (node is ConnectionReceiverNode)
            {
                ConnectionReceiverNode n = (ConnectionReceiverNode) node;
                litegraphNode.properties["Channel"] = n.Channel.ToString();
                litegraphNode.properties["Name"] = n.Channel.ToString();
            }

            if (node is ConnectionTransmitterNode)
            {
                ConnectionTransmitterNode n = (ConnectionTransmitterNode) node;
                litegraphNode.properties["Channel"] = n.Channel.ToString();
                litegraphNode.properties["Name"] = n.Channel.ToString();
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



        [Authorize(UserClaims.EditorObserver)]
        public async Task<List<LiteGraph.Link>> GetLinks(string panelId)
        {
            return await Task.Run(() =>
            {
                if (engine == null)
                    return null;

                List<Link> links = engine.GetLinks();
                if (links == null || !links.Any())
                    return null;

                return (
                    from link in links
                    where link.PanelId == panelId
                    select ConvertLinkToLiteGraphLink(link)).ToList();
            });
        }


        private int GetInputSlot(string inputId)
        {
            foreach (Node node in engine.GetNodes())
            {
                for (int i = 0; i < node.Inputs.Count; i++)
                {
                    if (node.Inputs[i].Id == inputId)
                        return i;
                }
            }
            return -1;
        }

        private int GetOutputSlot(string outputId)
        {
            foreach (Node node in engine.GetNodes())
            {
                for (int i = 0; i < node.Outputs.Count; i++)
                {
                    if (node.Outputs[i].Id == outputId)
                        return i;
                }
            }
            return -1;
        }



        [Authorize(UserClaims.EditorEditor)]
        public async Task<bool> RemoveLink(LiteGraph.Link link)
        {
            return await Task.Run(() =>
            {
                if (engine == null)
                    return false;

                if (link.origin_id == null || link.target_id == null)
                    return false;

                Node outNode = SystemController.nodesEngine.GetNode(link.origin_id);
                Node inNode = SystemController.nodesEngine.GetNode(link.target_id);
                if (outNode == null || inNode == null)
                {
                    engine.LogEngineError(
                        $"Can`t remove link from [{link.origin_id}] to [{link.target_id}]. Does not exist.");
                    return false;
                }
                engine.RemoveLink(outNode.Outputs[link.origin_slot], inNode.Inputs[link.target_slot]);

                return true;
            });
        }



        [Authorize(UserClaims.EditorEditor)]
        public async Task<bool> CreateLink(LiteGraph.Link link)
        {
            return await Task.Run(() =>
            {
                if (engine == null)
                    return false;

                Node outNode = SystemController.nodesEngine.GetNode(link.origin_id);
                Node inNode = SystemController.nodesEngine.GetNode(link.target_id);

                if (outNode == null || inNode == null)
                {
                    engine.LogEngineError(
                        $"Can`t create link from [{link.origin_id}] to [{link.target_id}]. Does not exist.");
                    return false;
                }

                engine.AddLink(outNode.Outputs[link.origin_slot], inNode.Inputs[link.target_slot]);
                return true;
            });
        }



        [Authorize(UserClaims.EditorEditor)]
        public async Task<bool> AddNode(LiteGraph.Node node)
        {
            return await Task.Run(() =>
            {
                if (engine == null)
                    return false;

                string type = node.properties["ObjectType"];
                string assemblyName = node.properties["Assembly"];

                Node newNode = AddNode(type, assemblyName);

                if (newNode == null)
                {
                    engine.LogEngineError($"Can`t create node [{node.properties["ObjectType"]}]. Type does not exist.");
                    return false;
                }

                newNode.Position = new Position {X = node.pos[0], Y = node.pos[1]};
                if (node.size.Length == 2)
                    newNode.Size = new Size {Width = node.size[0], Height = node.size[1]};
                //newNode.Id = node.id;
                newNode.PanelId = node.panel_id ?? MAIN_PANEL_ID;

                engine.AddNode(newNode);

                return true;
            });
        }

        private Node AddNode(string type, string assemblyName)
        {
            try
            {
                var newObject = Activator.CreateInstance(assemblyName, type);
                return (Node) newObject.Unwrap();
            }
            catch
            {
                return null;
            }
        }





        [Authorize(UserClaims.EditorEditor)]
        public async Task<bool> CloneNode(string id)
        {
            return await Task.Run(() =>
            {
                if (engine == null)
                    return false;

                engine.CloneNode(id);

                return true;
            });
        }






        [Authorize(UserClaims.EditorEditor)]
        public async Task<bool> RemoveNode(LiteGraph.Node node)
        {
            return await Task.Run(() =>
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
            });
        }



        [Authorize(UserClaims.EditorEditor)]
        public async Task<bool> UpdateNode(LiteGraph.Node node)
        {
            return await Task.Run(() =>
            {
                if (engine == null)
                    return false;

                Node oldNode = engine.GetNode(node.id);
                if (oldNode == null)
                {
                    engine.LogEngineError($"Can`t update node [{node.id}]. Does not exist.");
                    return false;
                }

                oldNode.Position = new Position {X = node.pos[0], Y = node.pos[1]};
                oldNode.Size = new Size {Width = node.size[0], Height = node.size[1]};

                engine.UpdateNode(oldNode);
                engine.UpdateNodeInDb(oldNode);

                return true;
            });
        }




        [Authorize(UserClaims.EditorEditor)]
        public bool PanelSettings(string id, string panelname)
        {
            Node n = engine.GetNode(id);
            if (n == null)
            {
                engine.LogEngineError($"Can`t set settings for Panel [{id}]. Does not exist.");
                return false;
            }

            PanelNode node = (PanelNode) n;
            node.Name = panelname;
            engine.UpdateNode(node);
            engine.UpdateNodeInDb(node);

            return true;
        }



        [Authorize(UserClaims.EditorEditor)]
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
                PanelInputNode node = (PanelInputNode) n;
                node.UpdateName(name);
            }

            if (n is PanelOutputNode)
            {
                PanelOutputNode node = (PanelOutputNode) n;
                node.UpdateName(name);
            }

            return true;
        }



        [Authorize(UserClaims.EditorEditor)]
        public bool UINodeSettings(string id, string name, int panelIndex, bool show)
        {
            Node n = engine.GetNode(id);
            if (n == null)
            {
                engine.LogEngineError($"Can`t set settings for Node [{id}]. Does not exist.");
                return false;
            }

            UiNode node = (UiNode) n;
            node.Name = name;
            node.PanelIndex = panelIndex;
            node.ShowOnMainPage = show;
            engine.UpdateNode(node);
            engine.UpdateNodeInDb(node);

            return true;
        }




        [Authorize(UserClaims.EditorEditor)]
        public bool UISliderSettings(string id, string name, int panelIndex,int min, int max, bool show)
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

            UiSliderNode node = (UiSliderNode) n;
            node.Name = name;
            node.PanelIndex = panelIndex;
            node.Min = min;
            node.Max = max;
            node.ShowOnMainPage = show;
            engine.UpdateNode(node);
            engine.UpdateNodeInDb(node);

            return true;
        }



        [Authorize(UserClaims.EditorEditor)]
        public bool ConstantSettings(string id, string value)
        {
            Node n = engine.GetNode(id);
            if (n == null)
            {
                engine.LogEngineError($"Can`t set settings for Node [{id}]. Does not exist.");
                return false;
            }

            ConstantNode node = (ConstantNode) n;
            node.SetValue(value);
            engine.UpdateNode(node);
            engine.UpdateNodeInDb(node);

            return true;
        }




        [Authorize(UserClaims.EditorEditor)]
        public bool UIChartSettings(string id, string name, int panelIndex, bool show, bool writeInDatabase, int updateInterval)
        {
            Node n = engine.GetNode(id);
            if (n == null)
            {
                engine.LogEngineError($"Can`t set settings for Node [{id}]. Does not exist.");
                return false;
            }

            UiChartNode node = (UiChartNode) n;

            if (node.WriteInDatabase && !writeInDatabase)
                uiEngine.ClearChart(node.Id);

            node.Name = name;
            node.PanelIndex = panelIndex;
            node.ShowOnMainPage = show;
            node.WriteInDatabase = writeInDatabase;
            node.UpdateInterval = updateInterval;
            engine.UpdateNode(node);
            engine.UpdateNodeInDb(node);


            return true;
        }


        [Authorize(UserClaims.EditorObserver)]
        public async Task<string> SerializePanel(string id)
        {
            return await Task.Run(() =>
            {
                if (engine == null)
                    return null;

                PanelNode node = engine.GetPanelNode(id);
                if (node == null)
                    return null;

                return NodesEngineSerializer.SerializePanel(id, engine);
            });
        }



        [Authorize(UserClaims.EditorObserver)]
        public async Task<IActionResult> SerializePanelToFile(string id)
        {
            return await Task.Run(() =>
            {
                if (engine == null)
                    return null;

                PanelNode node = engine.GetPanelNode(id);
                if (node == null)
                    return null;

                string json = NodesEngineSerializer.SerializePanel(id, engine);

                return File(Encoding.UTF8.GetBytes(json), "text/plain", node.Name + ".json");
            });
        }










        [Authorize(UserClaims.EditorEditor)]
        public async Task<bool> ImportPanelJson(string json, int x, int y, string ownerPanelId)
        {
            return await Task.Run(() =>
            {
                if (engine == null)
                    return false;

                try
                {
                    List<Node> nodes;
                    List<Link> links;
                    NodesEngineSerializer.DeserializePanel(json, out nodes, out links);

                    //set position to panel
                    nodes[0].Position = new Position(x, y);
                    nodes[0].PanelId = ownerPanelId;

                    engine.GenerateNewIds(ref nodes, ref links);

                    foreach (var node in nodes)
                        engine.AddNode(node);

                    foreach (var link in links)
                        engine.AddLink(link.OutputId, link.InputId);

                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }



  
        //todo authorize?

        public async Task<NodesEngineInfo> GetNodesEngineInfo()
        {
            return await Task.Run(() =>
            {
                if (engine == null)
                    return null;

                NodesEngineInfo info = new NodesEngineInfo();
                info.Started = engine.IsStarted();
                info.LinksCount = engine.GetLinks().Count;
                info.AllNodesCount = engine.GetNodes().Count;
                info.PanelsNodesCount = engine.GetNodes().OfType<PanelNode>().Count();
                info.HardwareNodesCount = engine.GetNodes().OfType<MySensorsNode>().Count();
                info.InputsOutputsNodesCount = engine.GetNodes().Count(x => x is PanelInputNode || x is PanelOutputNode);
                info.UiNodesCount = engine.GetNodes().OfType<UiNode>().Count();
                info.OtherNodesCount = info.AllNodesCount
                                       - info.PanelsNodesCount
                                       - info.HardwareNodesCount
                                       - info.InputsOutputsNodesCount
                                       - info.UiNodesCount;

                return info;
            });
        }




        [Authorize(UserClaims.EditorEditor)]
        public async Task<bool> RemoveAllNodesAndLinks()
        {
            return await Task.Run(() =>
            {
                if (engine == null)
                    return false;

                engine.RemoveAllNodesAndLinks();
                return true;
            });
        }




        [Authorize(UserClaims.EditorEditor)]
        public bool ReceiverSettings(string id, int channel)
        {
            Node n = engine.GetNode(id);
            if (n == null)
            {
                engine.LogEngineError($"Can`t set settings for Node [{id}]. Does not exist.");
                return false;
            }

            ConnectionReceiverNode node = (ConnectionReceiverNode) n;
            node.SetChannel(channel);
            engine.UpdateNode(node);
            engine.UpdateNodeInDb(node);

            return true;
        }



        [Authorize(UserClaims.EditorEditor)]
        public bool TransmitterSettings(string id, int channel)
        {
            Node n = engine.GetNode(id);
            if (n == null)
            {
                engine.LogEngineError($"Can`t set settings for Node [{id}]. Does not exist.");
                return false;
            }

            ConnectionTransmitterNode node = (ConnectionTransmitterNode) n;
            node.SetChannel(channel);
            engine.UpdateNode(node);
            engine.UpdateNodeInDb(node);

            return true;
        }




        public async Task<int> ReceiverSetValue(string value, string channel, string password)
        {
            return await Task.Run(() =>
            {
                if (engine == null)
                    return 2;

                List<ConnectionRemoteReceiverNode> receivers = engine.GetNodes()
                    .OfType<ConnectionRemoteReceiverNode>()
                    .Where(x => x.GetChannel().ToString() == channel)
                    .ToList();

                if (!receivers.Any())
                {
                    engine.LogNodesError(
                        $"Received a value for Remote Receiver, but no receivers with channel [{channel}]");
                    return 2;
                }

                var ip = HttpContext.Connection.RemoteIpAddress;
                string address = ip?.ToString();

                bool received = false;

                foreach (var receiver in receivers)
                {
                    if (receiver.ReceiveValue(value, channel, password, address))
                        received = true;
                }

                return received ? 0 : 1;
            });
        }
    }
}