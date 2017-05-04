/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNodes.Nodes;
using MyNodes.Users;
using MyNodes.WebController.Code;
using Newtonsoft.Json;

namespace MyNodes.WebController.Controllers
{

    public class NodeEditorAPIController : Controller
    {
        const string MAIN_PANEL_ID = "Main";

        private NodesEngine engine = SystemController.nodesEngine;
        private UiNodesEngine uiEngine = SystemController.uiNodesEngine;



        [Authorize(UserClaims.EditorObserver)]
        public async Task<List<LiteGraph.Node>> GetNodesForPanel(string panelId)
        {
            return await Task.Run(() =>
            {
                if (engine == null)
                    return null;

                if (panelId == null)
                    panelId = MAIN_PANEL_ID;


                lock (engine.nodesLock)
                {
                    List<Nodes.Node> nodes = engine.GetNodes();
                    if (nodes == null || !nodes.Any())
                        return null;

                    return (
                        from node in nodes
                        where node.PanelId == panelId
                        select ConvertNodeToLiteGraphNode(node)).ToList();
                }
            });
        }


        public LiteGraph.Node ConvertNodeToLiteGraphNode(Nodes.Node node)
        {
            LiteGraph.Node litegraphNode = new LiteGraph.Node
            {
                title = node.Type,
                type = node.Category + "/" + node.Type,
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
            {
                foreach (var input in node.Inputs)
                {
                    litegraphNode.inputs.Add(new LiteGraph.Input
                    {
                        name = input.Name,
                        type = (int)input.Type,
                        link = engine.GetLinkForInput(input)?.Id,
                        isOptional = input.IsOptional
                    });
                }
            }

            if (node.Outputs != null)
            {
                foreach (var output in node.Outputs)
                {
                    List<Nodes.Link> links = engine.GetLinksForOutput(output);
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
                            type = (int)output.Type,
                            links = linksIds
                        });
                    }
                    else
                    {
                        litegraphNode.outputs.Add(new LiteGraph.Output
                        {
                            name = output.Name,
                            type = (int)output.Type
                        });
                    }
                }
            }

            if (node.Settings != null && node.Settings.Count > 0)
                litegraphNode.properties["Settings"] = JsonConvert.SerializeObject(node.Settings);


            if (node.Settings.ContainsKey("Name")
                && !string.IsNullOrEmpty(node.Settings["Name"].Value))
                litegraphNode.title += " [" + node.Settings["Name"].Value + "]";

            if (node is PanelNode)
            {
                litegraphNode.title = node.Settings["Name"].Value;
            }



            return litegraphNode;
        }


        public LiteGraph.Link ConvertLinkToLiteGraphLink(Nodes.Link link)
        {
            if (link == null)
                return null;
            LiteGraph.Link liteGraphLink = new LiteGraph.Link
            {
                origin_id = engine.GetOutputOwner(link.OutputId)?.Id,
                target_id = engine.GetInputOwner(link.InputId)?.Id,
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

                List<Nodes.Link> links = engine.GetLinks();
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
            lock (engine.nodesLock)
            {
                foreach (Nodes.Node node in engine.GetNodes())
                {
                    for (int i = 0; i < node.Inputs.Count; i++)
                    {
                        if (node.Inputs[i].Id == inputId)
                            return i;
                    }
                }
                return -1;
            }
        }

        private int GetOutputSlot(string outputId)
        {
            lock (engine.nodesLock)
            {
                foreach (Nodes.Node node in engine.GetNodes())
                {
                    for (int i = 0; i < node.Outputs.Count; i++)
                    {
                        if (node.Outputs[i].Id == outputId)
                            return i;
                    }
                }
                return -1;
            }
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

                Nodes.Node outNode = SystemController.nodesEngine.GetNode(link.origin_id);
                Nodes.Node inNode = SystemController.nodesEngine.GetNode(link.target_id);
                if (outNode == null || inNode == null)
                {
                    engine.LogEngineError(
                        $"Can`t remove link from [{link.origin_id}] to [{link.target_id}]. Does not exist.");
                    return false;
                }

                if (outNode.GetNodeOptions().ProtectedAccess || inNode.GetNodeOptions().ProtectedAccess)
                {
                    if (!User.HasClaim(x => x.Type == UserClaims.EditorProtectedAccess))
                    {
                        engine.LogEngineError(
                            $"Can`t remove link from [{link.origin_id}] to [{link.target_id}]. No permissions for protected access.");
                        return false;
                    }
                }

                engine.RemoveLink(outNode.Outputs[link.origin_slot], inNode.Inputs[link.target_slot], true);

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

                Nodes.Node outNode = SystemController.nodesEngine.GetNode(link.origin_id);
                Nodes.Node inNode = SystemController.nodesEngine.GetNode(link.target_id);

                if (outNode == null || inNode == null)
                {
                    engine.LogEngineError(
                        $"Can`t create link from [{link.origin_id}] to [{link.target_id}]. Does not exist.");
                    return false;
                }

                if (outNode.GetNodeOptions().ProtectedAccess || inNode.GetNodeOptions().ProtectedAccess)
                {
                    if (!User.HasClaim(x => x.Type == UserClaims.EditorProtectedAccess))
                    {
                        engine.LogEngineError(
                            $"Can`t create link from [{link.origin_id}] to [{link.target_id}]. No permissions for protected access.");
                        return false;
                    }
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

                Nodes.Node newNode = CreateNode(type, assemblyName);

                if (newNode == null)
                {
                    engine.LogEngineError($"Can`t create node [{node.properties["ObjectType"]}]. Type does not exist.");
                    return false;
                }

                if (newNode.GetNodeOptions().ProtectedAccess)
                {
                    if (!User.HasClaim(x => x.Type == UserClaims.EditorProtectedAccess))
                    {
                        engine.LogEngineError(
                            $"Can`t create node [{node.properties["ObjectType"]}]. No permissions for protected access.");
                        return false;
                    }
                }

                newNode.Position = new Position { X = node.pos[0], Y = node.pos[1] };
                if (node.size.Length == 2)
                    newNode.Size = new Size { Width = node.size[0], Height = node.size[1] };
                //newNode.Id = node.id;
                newNode.PanelId = node.panel_id ?? MAIN_PANEL_ID;

                engine.AddNode(newNode);

                return true;
            });
        }

        private Nodes.Node CreateNode(string type, string assemblyName)
        {
            try
            {
                var newObject = Activator.CreateInstance(assemblyName, type);
                return (Nodes.Node)newObject.Unwrap();
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

                Nodes.Node node = engine.GetNode(id);

                if (node == null)
                {
                    engine.LogEngineError($"Can`t clone node [{id}]. Does not exist.");
                    return false;
                }

                if (node.GetNodeOptions().ProtectedAccess)
                {
                    if (!User.HasClaim(x => x.Type == UserClaims.EditorProtectedAccess))
                    {
                        engine.LogEngineError(
                            $"Can`t clone node [{node.Category}/{node.Type}]. No permissions for protected access.");
                        return false;
                    }
                }

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

                Nodes.Node oldNode = engine.GetNode(node.id);
                if (oldNode == null)
                {
                    engine.LogEngineError($"Can`t remove node [{node.id}]. Does not exist.");
                    return false;
                }

                if (oldNode.GetNodeOptions().ProtectedAccess)
                {
                    if (!User.HasClaim(x => x.Type == UserClaims.EditorProtectedAccess))
                    {
                        engine.LogEngineError(
                            $"Can`t remove node [{oldNode.Category}/{oldNode.Type}]. No permissions for protected access.");
                        return false;
                    }
                }

                engine.RemoveNode(oldNode);
                return true;
            });
        }


        [Authorize(UserClaims.EditorEditor)]
        public async Task<bool> RemoveNodes(List<string> nodes)
        {
            return await Task.Run(() =>
            {
                foreach (var id in nodes)
                {
                    Nodes.Node oldNode = engine.GetNode(id);
                    if (oldNode == null)
                    {
                        engine.LogEngineError($"Can`t remove node [{id}]. Does not exist.");
                        return false;
                    }

                    if (oldNode.GetNodeOptions().ProtectedAccess)
                    {
                        if (!User.HasClaim(x => x.Type == UserClaims.EditorProtectedAccess))
                        {
                            engine.LogEngineError(
                                $"Can`t remove node [{oldNode.Category}/{oldNode.Type}]. No permissions for protected access.");
                            continue;
                        }
                    }

                    engine.RemoveNode(oldNode);
                }

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

                Nodes.Node oldNode = engine.GetNode(node.id);
                if (oldNode == null)
                {
                    engine.LogEngineError($"Can`t update node [{node.id}]. Does not exist.");
                    return false;
                }

                if (oldNode.Position == null || oldNode.Size == null
                    || oldNode.Position.X != node.pos[0] || oldNode.Position.Y != node.pos[1]
                    || oldNode.Size.Width != node.size[0] || oldNode.Size.Height != node.size[1])
                {
                    if (node.pos != null)
                        oldNode.Position = new Position { X = node.pos[0], Y = node.pos[1] };

                    if (node.size != null)
                        oldNode.Size = new Size { Width = node.size[0], Height = node.size[1] };

                    engine.UpdateNodeInEditor(oldNode);
                    engine.UpdateNodeInDb(oldNode);
                }

                return true;
            });
        }





        [Authorize(UserClaims.EditorEditor)]
        public bool SetNodeSettings(string id, Dictionary<string, string> data)
        {
            Nodes.Node node = engine.GetNode(id);
            if (node == null)
            {
                engine.LogEngineError($"Can`t set settings for node [{id}]. Does not exist.");
                return false;
            }

            if (node.GetNodeOptions().ProtectedAccess)
            {
                if (!User.HasClaim(x => x.Type == UserClaims.EditorProtectedAccess))
                {
                    engine.LogEngineError(
                        $"Can`t  set settings for node [{node.Category}/{node.Type}]. No permissions for protected access.");
                    return false;
                }
            }

            return node.SetSettings(data);
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

                return File(Encoding.UTF8.GetBytes(json), "text/plain", node.Settings["Name"].Value + ".json");
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
                    List<Nodes.Node> nodes;
                    List<Nodes.Link> links;
                    NodesEngineSerializer.DeserializePanel(json, out nodes, out links);

                    foreach (var node in nodes)
                    {
                        if (node.GetNodeOptions().ProtectedAccess)
                        {
                            if (!User.HasClaim(с => с.Type == UserClaims.EditorProtectedAccess))
                            {
                                engine.LogEngineError(
                                    $"Can`t import panel. Panel contains the nodes with protected access. No permissions for this operation.");
                                return true;
                            }
                        }
                    }


                    //set position to panel
                    nodes[0].Position = new Position(x, y);
                    nodes[0].PanelId = ownerPanelId;

                    engine.GenerateNewIds(ref nodes, ref links);

                   
                    int adddedNodes = engine.AddNodes(nodes);
                    int adddedLinks = engine.AddLinks(links);

                    return (adddedNodes == nodes.Count && adddedLinks == links.Count);
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

                lock (engine.nodesLock)
                {
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
                }
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


        [Authorize(UserClaims.EditorObserver)]
        public string GetNodeDescription(string id)
        {
            Nodes.Node node = engine.GetNode(id);

            return node == null ? "" : node.GetNodeDescription();
        }



        public async Task<int> ReceiverSetValue(string value, string channel, string password)
        {
            return await Task.Run(() =>
            {
                if (engine == null)
                    return 2;

                List<ConnectionRemoteReceiverNode> receivers=null;

                lock (engine.nodesLock)
                    receivers=engine.GetNodes()
                        .OfType<ConnectionRemoteReceiverNode>()
                        .Where(x => x.GetChannel().ToString() == channel)
                        .ToList();

                if (receivers==null || !receivers.Any())
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