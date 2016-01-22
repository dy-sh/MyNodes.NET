/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Newtonsoft.Json;

namespace MyNetSensors.Nodes
{
    public delegate void LogMessageEventHandler(string message);
    public class NodesEngine
    {
        const string MAIN_PANEL_ID = "Main";

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

        public event NodesEventHandler OnNodesUpdatedEvent;
        public event NodeEventHandler OnNewNodeEvent;
        public event NodeEventHandler OnRemoveNodeEvent;
        public event NodeEventHandler OnNodeUpdatedEvent;
        public event InputEventHandler OnInputUpdatedEvent;
        public event OutputEventHandler OnOutputUpdatedEvent;
        public event LinkEventHandler OnNewLinkEvent;
        public event LinkEventHandler OnRemoveLinkEvent;
        public event LinksEventHandler OnLinksUpdatedEvent;
        public event Action OnStartEvent;
        public event Action OnStopEvent;
        public event Action OnUpdateEvent;

        public delegate void NodesEventHandler(List<Node> nodes);
        public delegate void InputEventHandler(Input input);
        public delegate void OutputEventHandler(Output output);
        public delegate void LinkEventHandler(Link link);
        public delegate void LinksEventHandler(List<Link> link);




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




        public void Start()
        {
            started = true;
            updateNodesTimer.Start();
            UpdateStatesFromLinks();

            OnStartEvent?.Invoke();

            LogEngineInfo("Started");
        }


        public void Stop()
        {
            started = false;

            OnStopEvent?.Invoke();

            updateNodesTimer.Stop();
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

            foreach (var node in nodes)
            {
                foreach (var input in node.Inputs)
                    input.OnInputChange += OnInputChange;

                foreach (var output in node.Outputs)
                    output.OnOutputChange += OnOutputChange;

                node.OnLogError += LogNodeError;
                node.OnLogInfo += LogNodeInfo;
                node.OnUpdate += UpdateNode;
            }

            OnNodesUpdatedEvent?.Invoke(nodes);
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


            OnNodesUpdatedEvent?.Invoke(nodes);
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
                }

                OnUpdateEvent?.Invoke();
            }
            catch { }

            updateNodesTimer.Start();
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


            if (node is PanelNode)
            {
                if (!AddPanel((PanelNode)node))
                    return;
            }
            if (node is PanelInputNode)
            {
                if (!AddPanelInput((PanelInputNode)node))
                    return;
            }
            if (node is PanelOutputNode)
            {
                if (!AddPanelOutput((PanelOutputNode)node))
                    return;
            }

            foreach (var input in node.Inputs)
                input.OnInputChange += OnInputChange;

            foreach (var output in node.Outputs)
                output.OnOutputChange += OnOutputChange;


            node.OnLogError += LogNodeError;
            node.OnLogInfo += LogNodeInfo;
            node.OnUpdate += UpdateNode;

            nodes.Add(node);

            nodesDb?.AddNode(node);

            LogEngineInfo($"New node [{node.GetType().Name}]");

            OnNewNodeEvent?.Invoke(node);
        }



        private string GeneratePanelName(PanelNode node)
        {
            //auto naming
            List<PanelNode> panels = GetPanelNodes();
            List<string> names = panels.Select(x => x.Name).ToList();
            for (int i = 1; i <= names.Count + 1; i++)
            {
                if (!names.Contains($"Panel {i}"))
                    return $"Panel {i}";
            }
            return null;
        }

        private bool AddPanel(PanelNode node)
        {
            node.Name = GeneratePanelName(node);
            return true;
        }

        private bool AddPanelInput(PanelInputNode node)
        {
            if (node.PanelId == MAIN_PANEL_ID)
            {
                LogEngineError("Can`t create input for main panel.");
                return false;
            }

            PanelNode panel = GetPanelNode(node.PanelId);
            if (panel == null)
            {
                LogEngineError($"Can`t create panel input. Panel [{node.PanelId}] does not exist.");
                return false;
            }

            node.Name = GenerateInputName(panel, node);

            Input input = new Input
            {
                Id = node.Id,
                Name = node.Name
            };
            panel.Inputs.Add(input);
            input.OnInputChange += OnInputChange;

            UpdateNode(panel, true);

            return true;
        }



        private bool AddPanelOutput(PanelOutputNode node)
        {
            if (node.PanelId == MAIN_PANEL_ID)
            {
                LogEngineError("Can`t create output for main panel.");
                return false;
            }

            PanelNode panel = GetPanelNode(node.PanelId);
            if (panel == null)
            {
                LogEngineError($"Can`t create panel output. Panel [{node.PanelId}] does not exist.");
                return false;
            }

            node.Name = GenerateOutputName(panel, node);


            Output output = new Output
            {
                Id = node.Id,
                Name = node.Name
            };
            panel.Outputs.Add(output);
            output.OnOutputChange += OnOutputChange;

            UpdateNode(panel, true);
            return true;
        }



        private string GenerateInputName(PanelNode panel, PanelInputNode node)
        {
            //auto naming
            List<string> names = panel.Inputs.Select(x => x.Name).ToList();
            for (int i = 1; i <= names.Count + 1; i++)
            {
                if (!names.Contains($"In {i}"))
                    return $"In {i}";
            }
            return null;
        }

        private string GenerateOutputName(PanelNode panel, PanelOutputNode node)
        {
            //auto naming
            List<string> names = panel.Outputs.Select(x => x.Name).ToList();
            for (int i = 1; i <= names.Count + 1; i++)
            {
                if (!names.Contains($"Out {i}"))
                    return $"Out {i}";
            }
            return null;
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

            OnRemoveNodeEvent?.Invoke(node);
            LogEngineInfo($"Remove node [{node.GetType().Name}]");

            foreach (var input in node.Inputs)
                input.OnInputChange -= OnInputChange;

            foreach (var output in node.Outputs)
                output.OnOutputChange -= OnOutputChange;

            node.OnLogError -= LogNodeError;
            node.OnLogInfo -= LogNodeInfo;
            node.OnUpdate -= UpdateNode;



            if (node is PanelInputNode)
                RemovePanelInput((PanelInputNode)node);
            else if (node is PanelOutputNode)
                RemovePanelOutput((PanelOutputNode)node);
            else if (node is PanelNode)
                RemovePanel((PanelNode)node);


            nodesDb?.RemoveNode(node.Id);

            nodes.Remove(node);
        }

        private void RemovePanel(PanelNode node)
        {
            List<Node> panels = GetNodesForPanel(node);
            foreach (var n in panels)
            {
                RemoveNode(n);
            }
        }

        public List<Node> GetNodesForPanel(PanelNode node)
        {
            return nodes.Where(n => n.PanelId == node.Id).ToList();
        }

        public List<Node> GetNodesForPanel(string panelId)
        {
            return nodes.Where(n => n.PanelId == panelId).ToList();
        }

        private List<PanelNode> GetPanelNodes()
        {
            return nodes.Where(n => n is PanelNode).Cast<PanelNode>().ToList();
        }


        private bool RemovePanelInput(PanelInputNode node)
        {
            PanelNode panel = GetPanelNode(node.PanelId);
            if (panel == null)
            {
                LogEngineError($"Can`t remove panel input. Panel [{node.PanelId}] does not exist.");
                return false;
            }

            Input input = GetInput(node.Id);

            Link link = GetLinkForInput(input);
            if (link != null)
                RemoveLink(link);

            panel.Inputs.Remove(input);
            UpdateNode(panel, true);
            return true;
        }

        private bool RemovePanelOutput(PanelOutputNode node)
        {
            PanelNode panel = GetPanelNode(node.PanelId);
            if (panel == null)
            {
                LogEngineError($"Can`t remove panel input. Panel [{node.PanelId}] does not exist.");
                return false;
            }

            Output output = GetOutput(node.Id);

            List<Link> links = GetLinksForOutput(output);
            foreach (var link in links)
                RemoveLink(link);

            panel.Outputs.Remove(output);
            UpdateNode(panel, true);
            return true;
        }


        public void UpdateNode(Node node, bool writeNodeToDb)
        {
            if (writeNodeToDb)
            {
                Node oldNode = GetNode(node.Id);

                if (oldNode == null)
                {
                    LogEngineError($"Can`t update node [{node.GetType().Name}]. Node [{node.Id}] does not exist.");
                    return;
                }

                LogEngineInfo($"Update node [{node.GetType().Name}]");

                if (node is PanelInputNode)
                    UpdatePanelInput((PanelInputNode)node);
                if (node is PanelOutputNode)
                    UpdatePanelOutput((PanelOutputNode)node);


                oldNode.Inputs = node.Inputs;
                oldNode.Outputs = node.Outputs;
                oldNode.Position = node.Position;
                oldNode.Size = node.Size;
                oldNode.Title = node.Title;
                oldNode.Type = node.Type;

                nodesDb?.UpdateNode(oldNode);
            }

            OnNodeUpdatedEvent?.Invoke(node);
        }

        private void UpdatePanelInput(PanelInputNode node)
        {
            Input input = GetInput(node.Id);
            input.Name = node.Name;
            Node panel = GetPanelNode(node.PanelId);

            UpdateNode(panel, true);
        }
        private void UpdatePanelOutput(PanelOutputNode node)
        {
            Output output = GetOutput(node.Id);
            output.Name = node.Name;
            Node panel = GetPanelNode(node.PanelId);

            UpdateNode(panel, true);
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

            oldOutput.Value = value;

            if (name != null && name != oldOutput.Name)
            {
                oldOutput.Name = name;
                Node node = GetOutputOwner(oldOutput);
                nodesDb?.UpdateNode(node);
            }
        }

        public void UpdateInput(string inputId, string value, string name = null)
        {
            Input oldInput = GetInput(inputId);

            if (oldInput == null)
            {
                LogEngineError($"Can`t update input [{inputId}]. Does not exist.");
                return;
            }

            oldInput.Value = value;

            if (name != null && name != oldInput.Name)
            {
                oldInput.Name = name;
                Node node = GetInputOwner(oldInput);
                nodesDb?.UpdateNode(node);
            }

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

            OnNewLinkEvent?.Invoke(link);

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
            LogEngineInfo($"Remove link from [{outputNode.GetType().Name}] to [{inputNode.GetType().Name}]");

            nodesDb?.RemoveLink(link.Id);

            OnRemoveLinkEvent?.Invoke(link);
            links.Remove(link);

            input.Value = null;
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

        public List<Link> GetLinksForPanel(string panelId)
        {
            return links.Where(x => x.PanelId == panelId).ToList();
        }

        private void UpdateStatesFromLinks()
        {
            if (links == null)
                return;

            foreach (var link in links)
            {
                Input input = GetInput(link.InputId);
                Output output = GetOutput(link.OutputId);
                input.Value = output.Value;
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

            if (node is PanelNode)
                GetNode(input.Id).Outputs[0].Value = input.Value;

            if (node is PanelOutputNode)
                GetOutput(node.Id).Value = input.Value;

            OnInputUpdatedEvent?.Invoke(input);

            changedInputsStack.Remove(input);
        }

        public void OnOutputChange(Output output)
        {
            if (!started)
                return;

            Node node = GetOutputOwner(output);
            if (node == null)
                return;

            OnOutputUpdatedEvent?.Invoke(output);

            node.OnOutputChange(output);

            //send state to linked nodes
            List<Link> list = links.Where(x => x.OutputId == output.Id).ToList();
            foreach (var link in list)
            {
                Input input = GetInput(link.InputId);
                if (input != null)
                {
                    input.Value = output.Value;
                }
            }

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
            LogEngineInfo("Remove all nodes and links");

            nodesDb?.RemoveAllNodes();

            foreach (var node in nodes)
            {
                node.OnRemove();

                foreach (var input in node.Inputs)
                    input.OnInputChange -= OnInputChange;

                foreach (var output in node.Outputs)
                    output.OnOutputChange -= OnOutputChange;

                node.OnLogError -= LogNodeError;
                node.OnLogInfo -= LogNodeInfo;
                node.OnUpdate -= UpdateNode;
            }

            links = new List<Link>();
            nodes = new List<Node>();

            OnNodesUpdatedEvent?.Invoke(nodes);
            OnLinksUpdatedEvent?.Invoke(links);

        }



        public void RemoveAllLinks()
        {
            LogEngineInfo("Remove all links");

            links = new List<Link>();
            OnLinksUpdatedEvent?.Invoke(links);
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



        private string SerializeNodesAndLinks(List<Node> nodesList, List<Link> linksList)
        {
            List<Object> list = new List<Object>();
            list.AddRange(nodesList);
            list.AddRange(linksList);

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;

            return JsonConvert.SerializeObject(list, settings);
        }

        private void DeserializeNodesAndLinks(string json, out List<Node> nodesList, out List<Link> linksList)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.ObjectCreationHandling = ObjectCreationHandling.Replace;

            List<object> objects = (List<object>)JsonConvert.DeserializeObject<object>(json, settings);

            nodesList = objects.OfType<Node>().ToList();
            linksList = objects.OfType<Link>().ToList();
        }


        public string SerializePanel(string id)
        {
            Node panel = GetNode(id) as PanelNode;
            if (panel == null)
            {
                LogEngineError($"Can`t serialize Panel [{id}]. Does not exist.");
                return null;
            }

            List<Node> nodesList = new List<Node>();
            nodesList.Add(panel);
            nodesList.AddRange(GetNodesForPanel(id));

            List<Link> linksList = GetLinksForPanel(id);

            return SerializeNodesAndLinks(nodesList, linksList);
        }

        public void DeserializePanel(string json, out List<Node> nodesList, out List<Link> linksList)
        {
            DeserializeNodesAndLinks(json, out nodesList, out linksList);
        }

        public void CloneNode(string id)
        {
            Node oldNode = GetNode(id);
            if (oldNode is PanelNode)
            {
                string json = SerializePanel(id);

                List<Node> newNodes;
                List<Link> newLinks;
                DeserializePanel(json, out newNodes, out newLinks);

                newNodes[0].Position = new Position { X = oldNode.Position.X + 10, Y = oldNode.Position.Y + 15 };

                GenerateNewIds(ref newNodes, ref newLinks);

                foreach (var node in newNodes)
                    AddNode(node);

                foreach (var link in newLinks)
                    AddLink(link.OutputId, link.InputId);
            }
            else
            {
                Node newNode = (Node)oldNode.Clone();
                newNode.Position = new Position { X = oldNode.Position.X + 10, Y = oldNode.Position.Y + 15 };
                AddNode(newNode);
            }


        }



        public void GenerateNewIds(ref List<Node> nodesList, ref List<Link> linksList)
        {
            foreach (var node in nodesList)
            {
                foreach (var input in node.Inputs)
                {
                    string oldId = input.Id;
                    input.Id = Guid.NewGuid().ToString();

                    foreach (var link in linksList.Where(x => x.InputId == oldId))
                        link.InputId = input.Id;
                }

                foreach (var output in node.Outputs)
                {
                    string oldId = output.Id;
                    output.Id = Guid.NewGuid().ToString();

                    foreach (var link in linksList.Where(x => x.OutputId == oldId))
                        link.OutputId = output.Id;
                }

                if (node is PanelNode)
                {
                    string oldId = node.Id;
                    node.Id = Guid.NewGuid().ToString();

                    foreach (var n in nodesList.Where(x => x.PanelId == oldId))
                        n.PanelId = node.Id;
                }
                else if (node is PanelInputNode)
                {
                    node.Id = Guid.NewGuid().ToString();
                    //todo
                }
                else if (node is PanelOutputNode)
                {
                    node.Id = Guid.NewGuid().ToString();
                    //todo
                }
                else
                {
                    node.Id = Guid.NewGuid().ToString();
                }
            }
        }
    }
}
