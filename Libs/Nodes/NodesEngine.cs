/*  MyNetSensors 
    Copyright (C) 2015-2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Newtonsoft.Json;

namespace MyNetSensors.Nodes
{
    public delegate void LogEventHandler(string message);
    public delegate void LogMessageEventHandler(string message);

    public class NodesEngine
    {
        public readonly string MAIN_PANEL_ID = "Main";

        //If you have tons of nodes, and system perfomance decreased, increase this value,
        //and you will get less nodes updating frequency 
        private int updateNodesInterval = 1;

        private INodesRepository nodesDb;

        private Timer updateNodesTimer = new Timer();
        private List<Node> nodes = new List<Node>();
        private List<Link> links = new List<Link>();

        private bool started = false;

        public static NodesEngine nodesEngine;

        public event LogMessageEventHandler OnLogNodeInfo;
        public event LogMessageEventHandler OnLogNodeError;
        public event LogMessageEventHandler OnLogEngineInfo;
        public event LogMessageEventHandler OnLogEngineError;

        public event NodeEventHandler OnNewNode;
        public event NodeEventHandler OnRemoveNode;
        public event NodeEventHandler OnNodeUpdated;
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




        public NodesEngine(INodesRepository nodesDb = null)
        {
            //var x= AppDomain.CurrentDomain.GetAssemblies()
            //           .SelectMany(assembly => assembly.GetTypes())
            //           .Where(type => type.IsSubclassOf(typeof(Node))).ToList();


            NodesEngine.nodesEngine = this;

            this.nodesDb = nodesDb;

            updateNodesTimer.Elapsed += UpdateNodes;
            updateNodesTimer.Interval = updateNodesInterval;

            if (nodesDb != null)
            {
                GetNodesFromRepository();
                GetLinksFromRepository();
            }
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

            updateNodesTimer.Start();
            OnStart?.Invoke();
            LogEngineInfo("Started");
        }


        public void Stop()
        {
            if (!started)
                return;

            started = false;
            starting = false;

            updateNodesTimer.Stop();
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
            if (nodesDb != null)
                nodes = nodesDb.GetAllNodes();

            //to prevent changing of collection while writing to db is not yet finished
            Node[] nodesArray = new Node[nodes.Count];
            nodes.CopyTo(nodesArray);

            foreach (var node in nodesArray)
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



        private void GetLinksFromRepository()
        {
            if (nodesDb != null)
                links = nodesDb.GetAllLinks();

            //remove link if node is not exist
            Link[] oldLinks = links.ToArray();
            foreach (var link in oldLinks)
            {
                if (GetInput(link.InputId) == null || GetOutput(link.OutputId) == null)
                {
                    nodesDb?.RemoveLink(link.Id);

                    links.Remove(link);
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

            if (changedInputsStack.Contains(input))
            {
                changedInputsStack.Remove(input);
                LogEngineError($"Infinite loop detected in Node [{node.Type}] [{node.Id}].");
                return;
            }
            changedInputsStack.Add(input);

            node.OnInputChange(input);

            OnInputStateUpdated?.Invoke(input);

            try
            {
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

            OnOutputStateUpdated?.Invoke(output);

            node.OnOutputChange(output);
        }


        private void UpdateNodes(object sender, ElapsedEventArgs e)
        {
            if (nodes == null || !nodes.Any())
                return;

            updateNodesTimer.Stop();

            try
            {
                //to prevent changing of collection while writing to db is not yet finished
                Node[] nodesTemp = new Node[nodes.Count];
                nodes.CopyTo(nodesTemp);

                foreach (var node in nodesTemp)
                {
                    node.Loop();
                    if (!started)
                        break;
                }

                OnUpdateLoop?.Invoke();
            }
            catch { }

            if (started)
                updateNodesTimer.Start();
        }




        private void LogNodeInfo(string message)
        {
            OnLogNodeInfo?.Invoke(message);
        }

        private void LogNodeError(string message)
        {
            OnLogNodeError?.Invoke(message);
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
            updateNodesInterval = ms;
            updateNodesTimer.Stop();
            updateNodesTimer.Interval = updateNodesInterval;
            updateNodesTimer.Start();
        }

        public int GetUpdateInterval()
        {
            return updateNodesInterval;
        }


        public Node GetNode(string id)
        {
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

            nodes.Add(node);

            nodesDb?.AddNode(node);

            LogEngineInfo($"New node [{node.GetType().Name}]");

            OnNewNode?.Invoke(node);
        }










        public void RemoveNode(Node node)
        {
            if (!nodes.Contains(node))
            {
                LogEngineError($"Can`t remove node [{node.GetType().Name}]. Node [{node.Id}] does not exist.");
                return;
            }

            List<Link> links = GetLinksForNode(node);
            foreach (var link in links)
            {
                RemoveLink(link);
            }

            node.OnRemove();
            nodes.Remove(node);
            nodesDb?.RemoveNode(node.Id);

            LogEngineInfo($"Remove node [{node.GetType().Name}]");
            OnRemoveNode?.Invoke(node);
        }



        public List<Node> GetNodesForPanel(string panelId, bool includeSubPanels)
        {
            if (!includeSubPanels)
            {
                return nodes.Where(n => n.PanelId == panelId).ToList();
            }
            else
            {
                List<Node> nodesList = nodes.Where(n => n.PanelId == panelId).ToList();
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
                return links.Where(n => n.PanelId == panelId).ToList();
            }
            else
            {
                List<Link> linksList = links.Where(n => n.PanelId == panelId).ToList();
                List<PanelNode> panels = GetNodesForPanel(panelId, true).OfType<PanelNode>().ToList();
                foreach (PanelNode panel in panels)
                {
                    linksList.AddRange(links.Where(n => n.PanelId == panel.Id).ToList());
                }
                return linksList;
            }
        }


        public List<PanelNode> GetPanelNodes()
        {
            return nodes.Where(n => n is PanelNode).Cast<PanelNode>().ToList();
        }





        public void UpdateNode(Node node)
        {
            LogEngineInfo($"Update node [{node.GetType().Name}]");
            OnNodeUpdated?.Invoke(node);
        }

        public void UpdateNodeInDb(Node node)
        {
            Node oldNode = GetNode(node.Id);

            if (oldNode == null)
            {
                LogEngineError($"Can`t update node [{node.GetType().Name}]. Node [{node.Id}] does not exist.");
                return;
            }

            LogEngineInfo($"Update node in DB [{node.GetType().Name}]");
            nodesDb?.UpdateNode(node);
        }

   
        public PanelNode GetPanelNode(string panelId)
        {
            return (PanelNode)nodes.FirstOrDefault(n => n is PanelNode && n.Id == panelId);
        }


        public void UpdateOutput(string outputId, string value, string name = null)
        {
            Output oldOutput = GetOutput(outputId);

            if (oldOutput == null)
            {
                LogEngineError($"Can`t update output [{outputId}]. Does not exist.");
                return;
            }


            if (name != null && name != oldOutput.Name)
            {
                oldOutput.Name = name;
                Node node = GetOutputOwner(oldOutput);
                nodesDb?.UpdateNode(node);
                OnNodeUpdated?.Invoke(node);
            }

            oldOutput.Value = value;
        }

        public void UpdateInput(string inputId, string value, string name = null)
        {
            Input oldInput = GetInput(inputId);

            if (oldInput == null)
            {
                LogEngineError($"Can`t update input [{inputId}]. Does not exist.");
                return;
            }


            if (name != null && name != oldInput.Name)
            {
                oldInput.Name = name;
                Node node = GetInputOwner(oldInput);
                nodesDb?.UpdateNode(node);
                OnNodeUpdated?.Invoke(node);
            }

            oldInput.Value = value;
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



            //prevent two links to one input
            Link oldLink = GetLinkForInput(input);
            if (oldLink != null)
                RemoveLink(oldLink);

            LogEngineInfo($"New link from [{outputNode.GetType().Name}] to [{inputNode.GetType().Name}]");

            Link link = new Link(output.Id, input.Id, inputNode.PanelId);
            links.Add(link);

            nodesDb?.AddLink(link);

            OnNewLink?.Invoke(link);

            if (!started)
                return;

            input.Value = output.Value;

        }



        public void RemoveLink(Output output, Input input)
        {
            Link link = GetLink(output, input);

            if (link == null)
            {
                LogEngineError($"Can`t remove link from [{output.Id}] to [{input.Id}]. Does not exist.");
                return;
            }

            Node inputNode = GetInputOwner(input);
            Node outputNode = GetOutputOwner(output);

            links.Remove(link);
            nodesDb?.RemoveLink(link.Id);
            LogEngineInfo($"Remove link from [{outputNode.GetType().Name}] to [{inputNode.GetType().Name}]");

            input.Value = null;
            OnRemoveLink?.Invoke(link);

        }

        public void RemoveLink(Link link)
        {
            Output output = GetOutput(link.OutputId);
            Input input = GetInput(link.InputId);

            if (output == null || input == null)
            {
                LogEngineError($"Can`t create link from [{link.OutputId}] to [{link.InputId}]. Does not exist.");
                return;
            }

            RemoveLink(output, input);
        }

        public Link GetLink(Output output, Input input)
        {
            return links.FirstOrDefault(x => x.InputId == input.Id && x.OutputId == output.Id);
        }

        public Link GetLinkForInput(Input input)
        {
            return links.FirstOrDefault(x => x.InputId == input.Id);
        }

        public List<Link> GetLinksForOutput(Output output)
        {
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
                node.OnInputChange(input);
            }
        }



        public Input GetInput(string id)
        {
            return nodes.SelectMany(node => node.Inputs).FirstOrDefault(input => input.Id == id);
        }

        public Output GetOutput(string id)
        {
            return nodes.SelectMany(node => node.Outputs).FirstOrDefault(output => output.Id == id);
        }


        public Node GetInputOwner(Input input)
        {
            return nodes.FirstOrDefault(node => node.Inputs.Contains(input));
        }

        public Node GetOutputOwner(Output output)
        {
            return nodes.FirstOrDefault(node => node.Outputs.Contains(output));
        }

        public Node GetInputOwner(string inputId)
        {
            return (from node in nodes from input in node.Inputs where input.Id == inputId select node).FirstOrDefault();
        }

        public Node GetOutputOwner(string outputId)
        {
            return (from node in nodes from output in node.Outputs where output.Id == outputId select node).FirstOrDefault();
        }




        public void RemoveAllNodesAndLinks()
        {

            links = new List<Link>();
            nodes = new List<Node>();

            LogEngineInfo("All nodes and links have been removed.");

            nodesDb?.RemoveAllNodes();
            nodesDb?.RemoveAllLinks();

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
            }
            else
            {
                string json = NodesEngineSerializer.SerializeNode(oldNode);
                Node newNode = NodesEngineSerializer.DeserializeNode(json);

                GenerateNewIds(newNode);

                newNode.Position = new Position { X = oldNode.Position.X + 5, Y = oldNode.Position.Y + 20 };
                AddNode(newNode);
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


    }
}

