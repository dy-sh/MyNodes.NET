/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;

namespace MyNodes.Nodes
{
    public delegate void LogEventHandler(string message);
    public delegate void LogMessageEventHandler(string message);

    public class NodesEngine
    {
        public readonly string MAIN_PANEL_ID = "Main";

        //If you have tons of nodes, and system perfomance decreased, increase this value,
        //and you will get less nodes updating frequency 
        private int UPDATE_NODES_INTEVAL = 1;

        private INodesRepository nodesDb;
        public INodesDataRepository dataDb;

        //  private Timer updateNodesTimer = new Timer();
        private List<Node> nodes = new List<Node>();
        private List<Link> links = new List<Link>();

        public Object nodesLock = new object();
        public Object linksLock = new object();

        private bool started = false;

        public static NodesEngine nodesEngine;

        public event LogMessageEventHandler OnLogNodeInfo;
        public event LogMessageEventHandler OnLogNodeError;
        public event LogMessageEventHandler OnLogEngineInfo;
        public event LogMessageEventHandler OnLogEngineError;

        public event NodeEventHandler OnNewNode;
        public event NodeEventHandler OnRemoveNode;
        public event NodeEventHandler OnNodeUpdatedOnDashboard;
        public event NodeEventHandler OnNodeUpdatedInEditor;
        public event NodeEventHandler OnNodeActivity;
        public event InputEventHandler OnInputStateUpdated;
        public event OutputEventHandler OnOutputStateUpdated;
        public event LinkEventHandler OnNewLink;
        public event LinkEventHandler OnRemoveLink;
        public event Action OnStart;
        public event Action OnStop;
        public event Action OnUpdateLoop;
        public event Action OnRemoveAllNodesAndLinks;


        // public delegate void NodesEventHandler(List<Node> nodes);
        //public delegate void LinksEventHandler(List<Link> link);
        public delegate void InputEventHandler(Input input);
        public delegate void OutputEventHandler(Output output);
        public delegate void LinkEventHandler(Link link);

        private DateTime lastUpdateTime;



        public NodesEngine(INodesRepository nodesDb = null, INodesDataRepository dataDb = null)
        {

            NodesEngine.nodesEngine = this;

            this.nodesDb = nodesDb;
            this.dataDb = dataDb;

            lastUpdateTime = DateTime.Now;

            activityTimer.Elapsed += UpdateShowActivity;
            activityTimer.Interval = SHOW_ACTIVITY_INTERVAL;
            activityTimer.Start();

            if (nodesDb != null)
            {
                GetNodesFromRepository();
                GetLinksFromRepository();
            }

            UpdateNodesLoop();
        }




        private bool starting;

        public void Start()
        {
            if (started || starting)
                return;


            LogEngineInfo("Starting...");
            starting = true;

            changedInputsStack.Clear();
            UpdateStatesFromLinks();

            started = true;

            //     updateNodesTimer.Start();
            OnStart?.Invoke();
            LogEngineInfo("Started");
        }


        public void Stop()
        {
            if (!started)
                return;

            started = false;
            starting = false;

            //       updateNodesTimer.Stop();
            changedInputsStack.Clear();

            OnStop?.Invoke();

            LogEngineInfo("Stopped");
        }

        public bool IsStarted()
        {
            return started;
        }

        public void GetNodesFromRepository()
        {
            lock (nodesLock)
            {
                if (nodesDb != null)
                    nodes = nodesDb.GetAllNodes();

                foreach (var node in nodes.ToArray())
                {
                    bool checkNodeCanBeAdded = node.OnAddToEngine(this);
                    if (!checkNodeCanBeAdded)
                    {
                        LogEngineError($"Can`t create node [{node.GetType().Name}]. Aborted by node.");
                        nodes.Remove(node);
                        continue;
                    }
                }
            }
        }



        private void GetLinksFromRepository()
        {
            lock (linksLock)
            {
                if (nodesDb != null)
                    links = nodesDb.GetAllLinks();

                foreach (var link in links.ToArray())
                {
                    if (GetInput(link.InputId) == null || GetOutput(link.OutputId) == null)
                    {
                        nodesDb?.RemoveLink(link.Id);

                        links.Remove(link);
                    }
                }
            }
        }




        // this list used for infinite loop detection
        List<Input> changedInputsStack = new List<Input>();

        public void OnInputChange(Input input)
        {
            if (!started)
                return;

            Node node = GetInputOwner(input.Id);

            if (node == null)
                return;

            ShowNodeActivity(node);

            if (changedInputsStack.Contains(input))
            {
                try
                {
                    changedInputsStack.Remove(input);
                }
                catch { }
                LogEngineError($"Event dropped in [{node.PanelName}: {node.Type}].");
                return;
            }
            changedInputsStack.Add(input);

            node.CheckInputDataTypeIsCorrect(input);

            OnInputStateUpdated?.Invoke(input);

            if (node.GetNodeOptions().ResetOutputsIfAnyInputIsNull
                && node.Inputs.Any(i => !i.IsOptional && i.Value == null))
                node.ResetOutputs();
            else
                node.OnInputChange(input);

            try
            {
                if (changedInputsStack.Contains(input))
                    changedInputsStack.Remove(input);
            }
            catch { }

        }


        public void OnOutputChange(Output output)
        {
            if (!started)
                return;

            Node node = GetOutputOwner(output);
            if (node == null)
                return;

            ShowNodeActivity(node);

            OnOutputStateUpdated?.Invoke(output);

            node.OnOutputChange(output);
        }


        private void UpdateNodesLoop()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if ((DateTime.Now - lastUpdateTime).TotalMilliseconds < UPDATE_NODES_INTEVAL)
                        continue;

                    lastUpdateTime = DateTime.Now;

                    lock (nodesLock)
                    {
                        if (nodes == null || !nodes.Any())
                            continue;

                        try
                        {
                            foreach (var node in nodes)
                            {
                                node.Loop();
                                if (!started)
                                    break;
                            }

                            if (started)
                                OnUpdateLoop?.Invoke();
                        }
                        catch
                        {
                        }
                    }
                }
            });
        }




        public List<Node> GetNodes()
        {
            return nodes;
        }

        public List<Link> GetLinks()
        {
            return links;
        }



        public void SetUpdateInterval(int ms)
        {
            UPDATE_NODES_INTEVAL = ms;
        }

        public int GetUpdateInterval()
        {
            return UPDATE_NODES_INTEVAL;
        }


        public Node GetNode(string id)
        {
            lock (nodesLock)
                return nodes.FirstOrDefault(x => x.Id == id);
        }


        public void AddNode(Node node)
        {
            if (node.PanelId != MAIN_PANEL_ID && GetPanelNode(node.PanelId) == null)
            {
                LogEngineError($"Can`t create node [{node.GetType().Name}]. Panel [{node.PanelId}] does not exist.");
                return;
            }

            bool checkNodeCanBeAdded = node.OnAddToEngine(this);
            if (!checkNodeCanBeAdded)
            {
                LogEngineError($"Can`t create node [{node.GetType().Name}]. Aborted by node.");
                return;
            }

            lock (nodesLock)
                nodes.Add(node);

            nodesDb?.AddNode(node);

            LogEngineInfo($"New node [{node.GetType().Name}]");

            OnNewNode?.Invoke(node);
        }





        public void RemoveNode(Node node)
        {
            lock (nodesLock)
                if (!nodes.Contains(node))
                {
                    LogEngineError($"Can`t remove node [{node.GetType().Name}]. Node [{node.Id}] does not exist.");
                    return;
                }

            List<Node> nodesToRemove = new List<Node>();
            nodesToRemove.Add(node);

            List<Link> linksToRemove = new List<Link>();
            List<Link> links = GetLinksForNode(node);
            linksToRemove.AddRange(links);

            foreach (var link in links)
            {
                RemoveLink(link, false);
            }

            node.OnRemove();

            if (node is PanelNode)
            {
                var nodesOnPanel = GetNodesForPanel(node.Id, true);
                nodesToRemove.AddRange(nodesOnPanel);

                foreach (var n in nodesOnPanel)
                {
                    List<Link> li = GetLinksForNode(n);
                    linksToRemove.AddRange(li);
                    foreach (var link in li)
                    {
                        RemoveLink(link, false);
                    }

                    lock (nodesLock)
                        nodes.Remove(n);
                }
            }

            lock (nodesLock)
                nodes.Remove(node);

            nodesDb?.RemoveNodes(nodesToRemove);
            nodesDb?.RemoveLinks(linksToRemove);

            LogEngineInfo($"Remove node [{node.GetType().Name}]");
            OnRemoveNode?.Invoke(node);
        }



        public List<Node> GetNodesForPanel(string panelId, bool includeSubPanels)
        {

            if (!includeSubPanels)
            {
                lock (nodesLock)
                    return nodes.Where(n => n.PanelId == panelId).ToList();
            }
            else
            {
                List<Node> nodesList;

                lock (nodesLock)
                    nodesList = nodes.Where(n => n.PanelId == panelId).ToList();

                List<PanelNode> panels = nodesList.OfType<PanelNode>().ToList();

                foreach (PanelNode panel in panels)
                {
                    nodesList.AddRange(GetNodesForPanel(panel.Id, true));
                }
                return nodesList;
            }
        }

        public List<Link> GetLinksForPanel(string panelId, bool includeSubPanels)
        {

            if (!includeSubPanels)
            {
                lock (linksLock)
                    return links.Where(n => n.PanelId == panelId).ToList();
            }
            else
            {
                List<Link> linksList;

                lock (linksLock)
                    linksList = links.Where(n => n.PanelId == panelId).ToList();

                List<PanelNode> panels = GetNodesForPanel(panelId, true).OfType<PanelNode>().ToList();

                lock (linksLock)
                {
                    foreach (PanelNode panel in panels)
                        linksList.AddRange(links.Where(n => n.PanelId == panel.Id).ToList());
                }
                return linksList;
            }
        }


        public List<PanelNode> GetPanelNodes()
        {
            lock (nodesLock)
                return nodes.Where(n => n is PanelNode).Cast<PanelNode>().ToList();
        }





        public void UpdateNodeOnDashboard(Node node)
        {
            OnNodeUpdatedOnDashboard?.Invoke(node);
        }


        public void UpdateNodeInEditor(Node node)
        {
            OnNodeUpdatedInEditor?.Invoke(node);
        }


        public void UpdateNodeInDb(Node node)
        {
            Node oldNode = GetNode(node.Id);

            if (oldNode == null)
            {
                LogEngineError($"Can`t update node [{node.GetType().Name}]. Node [{node.Id}] does not exist.");
                return;
            }

            //  LogEngineInfo($"Update node in DB [{node.GetType().Name}]");
            nodesDb?.UpdateNode(node);
        }


        public PanelNode GetPanelNode(string panelId)
        {
            lock (nodesLock)
                return (PanelNode)nodes.FirstOrDefault(n => n is PanelNode && n.Id == panelId);
        }


        public void UpdateOutput(string outputId, string value, string name = null)
        {
            Output output = GetOutput(outputId);

            if (output == null)
            {
                LogEngineError($"Can`t update output [{outputId}]. Does not exist.");
                return;
            }

            if (name != null && name != output.Name)
            {
                output.Name = name;
                Node node = GetOutputOwner(output);
                UpdateNodeInDb(node);
                UpdateNodeInEditor(node);
            }

            output.Value = value;
        }

        public void UpdateInput(string inputId, string value, string name = null)
        {
            Input input = GetInput(inputId);

            if (input == null)
            {
                LogEngineError($"Can`t update input [{inputId}]. Does not exist.");
                return;
            }


            if (name != null && name != input.Name)
            {
                input.Name = name;
                Node node = GetInputOwner(input);
                UpdateNodeInDb(node);
                UpdateNodeInEditor(node);
            }

            input.Value = value;
        }

        public void AddLink(string outputId, string inputId)
        {
            Input input = GetInput(inputId);
            Output output = GetOutput(outputId);

            if (input == null || output == null)
            {
                LogEngineError($"Can`t create link from [{outputId}] to [{inputId}]. Does not exist.");
                return;
            }

            AddLink(output, input);
        }

        public void AddLink(Output output, Input input)
        {
            Node inputNode = GetInputOwner(input);
            Node outputNode = GetOutputOwner(output);

            if (inputNode == null || outputNode == null)
            {
                LogEngineError($"Can`t create link from [{output.Id}] to [{input.Id}]. Does not exist.");
                return;
            }

            if (inputNode == outputNode)
            {
                LogEngineError($"Can`t create link from [{output.Id}] to [{input.Id}]. Input and output belong to the same node.");
                return;
            }

            if (inputNode.PanelId != outputNode.PanelId)
            {
                LogEngineError($"Can`t create link from {outputNode.GetType().Name} to {inputNode.GetType().Name}. Nodes are on different panels.");
                return;
            }


            Link link = new Link(output.Id, input.Id, inputNode.PanelId);

            //prevent two links to one input
            Link oldLink = GetLinkForInput(input);
            if (oldLink != null)
                RemoveLink(oldLink, true);

            LogEngineInfo($"New link from [{outputNode.GetType().Name}] to [{inputNode.GetType().Name}]");

            lock (linksLock)
                links.Add(link);


            nodesDb?.AddLink(link);

            OnNewLink?.Invoke(link);

            if (!started)
                return;

            input.Value = output.Value;

        }



        public void RemoveLink(Output output, Input input, bool writeInDb)
        {
            Link link = GetLink(output, input);

            if (link == null)
            {
                LogEngineError($"Can`t remove link from [{output.Id}] to [{input.Id}]. Does not exist.");
                return;
            }

            lock (linksLock)
                links.Remove(link);

            if (writeInDb)
                nodesDb?.RemoveLink(link.Id);

            Node inputNode = GetInputOwner(input);
            Node outputNode = GetOutputOwner(output);
            if (inputNode != null && outputNode != null)
                LogEngineInfo($"Remove link from [{outputNode.GetType().Name}] to [{inputNode.GetType().Name}]");
            else
                LogEngineInfo($"Remove link.");

            input.Value = null;
            OnRemoveLink?.Invoke(link);

        }

        public void RemoveLink(Link link, bool writeInDb)
        {
            Output output = GetOutput(link.OutputId);
            Input input = GetInput(link.InputId);

            if (output == null || input == null)
            {
                LogEngineError($"Can`t remove link from [{link.OutputId}] to [{link.InputId}]. Does not exist.");
                return;
            }

            RemoveLink(output, input, writeInDb);
        }

        public void RemoveLinks(List<Link> links, bool writeInDb)
        {
            foreach (var link in links)
            {
                RemoveLink(link, false);
            }

            nodesDb?.RemoveLinks(links);
        }

        public Link GetLink(Output output, Input input)
        {
            lock (linksLock)
                return links.FirstOrDefault(x => x.InputId == input.Id && x.OutputId == output.Id);
        }

        public Link GetLinkForInput(Input input)
        {
            lock (linksLock)
                return links.FirstOrDefault(x => x.InputId == input.Id);
        }

        public List<Link> GetLinksForOutput(Output output)
        {
            lock (linksLock)
                return links.Where(x => x.OutputId == output.Id).ToList();
        }

        public List<Link> GetLinksForNode(Node node)
        {
            List<Link> list = new List<Link>();

            foreach (var input in node.Inputs)
            {
                Link link = GetLinkForInput(input);
                if (link != null)
                    list.Add(link);
            }

            foreach (var output in node.Outputs)
            {
                List<Link> links = GetLinksForOutput(output);
                if (links != null)
                    list.AddRange(links);
            }

            return list;
        }



        private void UpdateStatesFromLinks()
        {
            if (links == null)
                return;

            foreach (var link in links)
            {
                if (!started && !starting)
                    return;

                Input input = GetInput(link.InputId);
                Output output = GetOutput(link.OutputId);
                input.Value = output.Value;

                //update node internal logic
                Node node = GetInputOwner(input);
                node.CheckInputDataTypeIsCorrect(input);

                if (node.GetNodeOptions().ResetOutputsIfAnyInputIsNull
                    && node.Inputs.Any(i => !i.IsOptional && i.Value == null))
                    node.ResetOutputs();
                else
                    node.OnInputChange(input);
            }
        }



        public Input GetInput(string id)
        {
            lock (nodesLock)
                return nodes.SelectMany(node => node.Inputs).FirstOrDefault(input => input.Id == id);
        }

        public Output GetOutput(string id)
        {
            lock (nodesLock)
                return nodes.SelectMany(node => node.Outputs).FirstOrDefault(output => output.Id == id);
        }


        public Node GetInputOwner(Input input)
        {
            lock (nodesLock)
                return nodes.FirstOrDefault(node => node.Inputs.Contains(input));
        }

        public Node GetOutputOwner(Output output)
        {
            lock (nodesLock)
                return nodes.FirstOrDefault(node => node.Outputs.Contains(output));
        }

        public Node GetInputOwner(string inputId)
        {
            lock (nodesLock)
                return (from node in nodes from input in node.Inputs where input.Id == inputId select node).FirstOrDefault();
        }

        public Node GetOutputOwner(string outputId)
        {
            lock (nodesLock)
                return (from node in nodes from output in node.Outputs where output.Id == outputId select node).FirstOrDefault();
        }




        public void RemoveAllNodesAndLinks()
        {
            lock (linksLock)
                links = new List<Link>();

            lock (nodesLock)
                nodes = new List<Node>();

            LogEngineInfo("All nodes and links have been removed.");

            nodesDb?.RemoveAllNodes();
            nodesDb?.RemoveAllLinks();
            dataDb?.RemoveAllNodesData();

            OnRemoveAllNodesAndLinks?.Invoke();
        }




        public void LogNodesInfo(string message)
        {
            OnLogNodeInfo?.Invoke(message);
        }

        public void LogNodesError(string message)
        {
            OnLogNodeError?.Invoke(message);
        }

        public void LogEngineInfo(string message)
        {
            OnLogEngineInfo?.Invoke(message);
        }

        public void LogEngineError(string message)
        {
            OnLogEngineError?.Invoke(message);
        }




        public void CloneNode(string id)
        {
            Node oldNode = GetNode(id);
            if (oldNode is PanelNode)
            {
                string json = NodesEngineSerializer.SerializePanel(id, this);

                List<Node> newNodes;
                List<Link> newLinks;
                NodesEngineSerializer.DeserializePanel(json, out newNodes, out newLinks);

                newNodes[0].Position = new Position { X = oldNode.Position.X + 5, Y = oldNode.Position.Y + 20 };

                GenerateNewIds(ref newNodes, ref newLinks);

                foreach (var node in newNodes)
                    AddNode(node);

                foreach (var link in newLinks)
                    AddLink(link.OutputId, link.InputId);

                newNodes[0].ResetInputs();
            }
            else
            {
                string json = NodesEngineSerializer.SerializeNode(oldNode);
                Node newNode = NodesEngineSerializer.DeserializeNode(json);

                GenerateNewIds(newNode);

                newNode.Position = new Position { X = oldNode.Position.X + 5, Y = oldNode.Position.Y + 20 };
                AddNode(newNode);
                newNode.ResetInputs();
            }


        }



        public void GenerateNewIds(ref List<Node> nodesList, ref List<Link> linksList)
        {
            foreach (var node in nodesList)
            {
                //generate id`s for inputs
                foreach (var input in node.Inputs)
                {
                    string oldId = input.Id;
                    input.Id = Guid.NewGuid().ToString();

                    //update links
                    foreach (var link in linksList.Where(x => x.InputId == oldId))
                        link.InputId = input.Id;

                    //for panel update input node id
                    if (node is PanelNode)
                        nodesList.FirstOrDefault(x => x.Id == oldId).Id = input.Id;
                }

                //generate id`s for outputs
                foreach (var output in node.Outputs)
                {
                    string oldId = output.Id;
                    output.Id = Guid.NewGuid().ToString();

                    //update links
                    foreach (var link in linksList.Where(x => x.OutputId == oldId))
                        link.OutputId = output.Id;

                    //for panel update output node id
                    if (node is PanelNode)
                        nodesList.FirstOrDefault(x => x.Id == oldId).Id = output.Id;
                }


                if (node is PanelNode)
                {
                    string oldId = node.Id;
                    node.Id = Guid.NewGuid().ToString();

                    foreach (var n in nodesList.Where(x => x.PanelId == oldId))
                        n.PanelId = node.Id;
                }
                else if (node is PanelInputNode || node is PanelOutputNode)
                {
                    //id already updated
                }
                else
                {
                    node.Id = Guid.NewGuid().ToString();
                }
            }
        }

        public void GenerateNewIds(Node node)
        {
            //generate id`s for inputs
            foreach (var input in node.Inputs)
                input.Id = Guid.NewGuid().ToString();

            //generate id`s for outputs
            foreach (var output in node.Outputs)
                output.Id = Guid.NewGuid().ToString();

            node.Id = Guid.NewGuid().ToString();
        }





        //NODES ACTIVITY

        public int SHOW_ACTIVITY_INTERVAL = 350;
        List<Node> activityList = new List<Node>();
        private Timer activityTimer = new Timer();


        public void ShowNodeActivity(Node node)
        {
            if (activityList.Contains(node))
                return;

            activityList.Add(node);
            OnNodeActivity?.Invoke(node);
        }

        private void UpdateShowActivity(object sender, ElapsedEventArgs e)
        {
            activityList.Clear();
        }
    }
}

