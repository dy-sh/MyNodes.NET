/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Newtonsoft.Json;

namespace MyNetSensors.LogicalNodes
{
    public delegate void LogMessageEventHandler(string message);
    public class LogicalNodesEngine
    {
        const string MAIN_PANEL_ID = "Main";

        //If you have tons of logical nodes, and system perfomance decreased, increase this value,
        //and you will get less nodes updating frequency 
        private int updateNodesInterval = 1;

        private ILogicalNodesRepository nodesDb;

        private Timer updateNodesTimer = new Timer();
        public List<LogicalNode> nodes = new List<LogicalNode>();
        public List<LogicalLink> links = new List<LogicalLink>();

        private bool started = false;

        public static LogicalNodesEngine logicalNodesEngine;

        public event LogMessageEventHandler OnLogNodeInfo;
        public event LogMessageEventHandler OnLogNodeError;
        public event LogMessageEventHandler OnLogEngineInfo;
        public event LogMessageEventHandler OnLogEngineError;

        public event LogicalNodesEventHandler OnNodesUpdatedEvent;
        public event LogicalNodeEventHandler OnNewNodeEvent;
        public event LogicalNodeEventHandler OnRemoveNodeEvent;
        public event LogicalNodeEventHandler OnNodeUpdatedEvent;
        public event LogicalInputEventHandler OnInputUpdatedEvent;
        public event LogicalOutputEventHandler OnOutputUpdatedEvent;
        public event LogicalLinkEventHandler OnNewLinkEvent;
        public event LogicalLinkEventHandler OnRemoveLinkEvent;
        public event LogicalLinksEventHandler OnLinksUpdatedEvent;

        public delegate void LogicalNodeEventHandler(LogicalNode node);
        public delegate void LogicalNodesEventHandler(List<LogicalNode> nodes);
        public delegate void LogicalInputEventHandler(Input input);
        public delegate void LogicalOutputEventHandler(Output output);
        public delegate void LogicalLinkEventHandler(LogicalLink link);
        public delegate void LogicalLinksEventHandler(List<LogicalLink> link);


        public LogicalNodesEngine(ILogicalNodesRepository nodesDb = null)
        {
            //var x= AppDomain.CurrentDomain.GetAssemblies()
            //           .SelectMany(assembly => assembly.GetTypes())
            //           .Where(type => type.IsSubclassOf(typeof(LogicalNode))).ToList();


            LogicalNodesEngine.logicalNodesEngine = this;

            this.nodesDb = nodesDb;




            updateNodesTimer.Elapsed += UpdateNodes;
            updateNodesTimer.Interval = updateNodesInterval;

            if (nodesDb != null)
            {
                nodesDb.CreateDb();
                GetNodesFromRepository();
                GetLinksFromRepository();
            }
        }




        public void Start()
        {
            started = true;
            updateNodesTimer.Start();

            UpdateStatesFromLinks();

            LogEngineInfo("Started");
        }


        public void Stop()
        {
            started = false;
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

            OnNodesUpdatedEvent?.Invoke(nodes);
        }

        private void GetLinksFromRepository()
        {
            if (nodesDb != null)
                links = nodesDb.GetAllLinks();

            //remove link if node is not exist
            LogicalLink[] oldLinks = links.ToArray();
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


        private void UpdateNodes(object sender, ElapsedEventArgs e)
        {
            if (nodes == null)
                return;

            updateNodesTimer.Stop();

            try
            {
                //to prevent changing of collection while writing to db is not yet finished
                LogicalNode[] nodesTemp = new LogicalNode[nodes.Count];
                nodes.CopyTo(nodesTemp);

                foreach (var node in nodesTemp)
                {
                    node.Loop();
                }
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


        public LogicalNode GetNode(string id)
        {
            return nodes.FirstOrDefault(x => x.Id == id);
        }


        public void AddNode(LogicalNode node)
        {
            if (node.PanelId != MAIN_PANEL_ID && GetPanelNode(node.PanelId) == null)
            {
                LogEngineError($"Can`t create node [{node.GetType().Name}]. Panel [{node.PanelId}] does not exist.");
                return;
            }


            if (node is LogicalNodePanel)
            {
                if (!AddPanel((LogicalNodePanel)node))
                    return;
            }
            if (node is LogicalNodePanelInput)
            {
                if (!AddPanelInput((LogicalNodePanelInput)node))
                    return;
            }
            if (node is LogicalNodePanelOutput)
            {
                if (!AddPanelOutput((LogicalNodePanelOutput)node))
                    return;
            }

            nodes.Add(node);

            nodesDb?.AddNode(node);

            LogEngineInfo($"New node [{node.GetType().Name}]");

            OnNewNodeEvent?.Invoke(node);
        }



        private string GeneratePanelName(LogicalNodePanel node)
        {
            //auto naming
            List<LogicalNodePanel> nodes = GetPanelNodes();
            List<string> names = nodes.Select(x => x.Name).ToList();
            for (int i = 1; i <= names.Count + 1; i++)
            {
                if (!names.Contains($"{node.Name} {i}"))
                    return $"{node.Name} {i}";
            }
            return null;
        }

        private bool AddPanel(LogicalNodePanel node)
        {
            node.Name = GeneratePanelName(node);
            return true;
        }

        private bool AddPanelInput(LogicalNodePanelInput node)
        {
            if (node.PanelId == MAIN_PANEL_ID)
            {
                LogEngineError("Can`t create input for main panel.");
                return false;
            }

            LogicalNodePanel panel = GetPanelNode(node.PanelId);
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

            UpdateNode(panel,true);

            return true;
        }



        private bool AddPanelOutput(LogicalNodePanelOutput node)
        {
            if (node.PanelId == MAIN_PANEL_ID)
            {
                LogEngineError("Can`t create output for main panel.");
                return false;
            }

            LogicalNodePanel panel = GetPanelNode(node.PanelId);
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

            UpdateNode(panel, true);
            return true;
        }



        private string GenerateInputName(LogicalNodePanel panel, LogicalNodePanelInput node)
        {
            //auto naming
            List<string> names = panel.Inputs.Select(x => x.Name).ToList();
            for (int i = 1; i <= names.Count + 1; i++)
            {
                if (!names.Contains($"{node.Name} {i}"))
                    return $"{node.Name} {i}";
            }
            return null;
        }

        private string GenerateOutputName(LogicalNodePanel panel, LogicalNodePanelOutput node)
        {
            //auto naming
            List<string> names = panel.Outputs.Select(x => x.Name).ToList();
            for (int i = 1; i <= names.Count + 1; i++)
            {
                if (!names.Contains($"{node.Name} {i}"))
                    return $"{node.Name} {i}";
            }
            return null;
        }


        public void RemoveNode(LogicalNode node)
        {
            if (!nodes.Contains(node))
            {
                LogEngineError($"Can`t remove node [{node.GetType().Name}]. Node [{node.Id}] does not exist.");
                return;
            }

            List<LogicalLink> links = GetLinksForNode(node);
            foreach (var link in links)
            {
                RemoveLink(link);
            }

            OnRemoveNodeEvent?.Invoke(node);
            LogEngineInfo($"Remove node [{node.GetType().Name}]");

            if (node is LogicalNodePanelInput)
                RemovePanelInput((LogicalNodePanelInput)node);
            else if (node is LogicalNodePanelOutput)
                RemovePanelOutput((LogicalNodePanelOutput)node);
            else if (node is LogicalNodePanel)
                RemovePanel((LogicalNodePanel)node);


            nodesDb?.RemoveNode(node.Id);

            nodes.Remove(node);
        }

        private void RemovePanel(LogicalNodePanel node)
        {
            List<LogicalNode> nodes = GetNodesForPanel(node);
            foreach (var n in nodes)
            {
                RemoveNode(n);
            }
        }

        private List<LogicalNode> GetNodesForPanel(LogicalNodePanel node)
        {
            return nodes.Where(n => n.PanelId == node.Id).ToList();
        }

        private List<LogicalNodePanel> GetPanelNodes()
        {
            return nodes.Where(n => n is LogicalNodePanel).Cast<LogicalNodePanel>().ToList();
        }


        private bool RemovePanelInput(LogicalNodePanelInput node)
        {
            LogicalNodePanel panel = GetPanelNode(node.PanelId);
            if (panel == null)
            {
                LogEngineError($"Can`t remove panel input. Panel [{node.PanelId}] does not exist.");
                return false;
            }

            Input input = GetInput(node.Id);

            LogicalLink link = GetLinkForInput(input);
            if (link != null)
                RemoveLink(link);

            panel.Inputs.Remove(input);
            UpdateNode(panel, true);
            return true;
        }

        private bool RemovePanelOutput(LogicalNodePanelOutput node)
        {
            LogicalNodePanel panel = GetPanelNode(node.PanelId);
            if (panel == null)
            {
                LogEngineError($"Can`t remove panel input. Panel [{node.PanelId}] does not exist.");
                return false;
            }

            Output output = GetOutput(node.Id);

            List<LogicalLink> links = GetLinksForOutput(output);
            foreach (var link in links)
                RemoveLink(link);

            panel.Outputs.Remove(output);
            UpdateNode(panel, true);
            return true;
        }


        public void UpdateNode(LogicalNode node, bool writeNodeToDb)
        {
            if (writeNodeToDb)
            {
                LogicalNode oldNode = nodes.FirstOrDefault(x => x.Id == node.Id);

                if (oldNode == null)
                {
                    LogEngineError($"Can`t update node [{node.GetType().Name}]. Node [{node.Id}] does not exist.");
                    return;
                }

                LogEngineInfo($"Update node [{node.GetType().Name}]");

                if (node is LogicalNodePanelInput)
                    UpdatePanelInput((LogicalNodePanelInput) node);
                if (node is LogicalNodePanelOutput)
                    UpdatePanelOutput((LogicalNodePanelOutput) node);


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

        private void UpdatePanelInput(LogicalNodePanelInput node)
        {
            Input input = GetInput(node.Id);
            input.Name = node.Name;
            LogicalNode panel = GetPanelNode(node.PanelId);

            UpdateNode(panel, true);
        }
        private void UpdatePanelOutput(LogicalNodePanelOutput node)
        {
            Output output = GetOutput(node.Id);
            output.Name = node.Name;
            LogicalNode panel = GetPanelNode(node.PanelId);

            UpdateNode(panel, true);
        }



        public LogicalNodePanel GetPanelNode(string panelId)
        {
            return (LogicalNodePanel)nodes.FirstOrDefault(n => n is LogicalNodePanel && n.Id == panelId);
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
                LogicalNode node = GetOutputOwner(oldOutput);
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
                LogicalNode node = GetInputOwner(oldInput);
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
            LogicalNode inputNode = GetInputOwner(input);
            LogicalNode outputNode = GetOutputOwner(output);

            if (inputNode == null || outputNode == null)
            {
                LogEngineError($"Can`t create link from [{output.Id}] to [{input.Id}]. Does not exist.");
                return;
            }

            if (inputNode == outputNode )
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
            LogicalLink oldLink = GetLinkForInput(input);
            if (oldLink != null)
                RemoveLink(oldLink);

            LogEngineInfo($"New link from [{outputNode.GetType().Name}] to [{inputNode.GetType().Name}]");

            LogicalLink link = new LogicalLink(output.Id, input.Id);
            link.PanelId = inputNode.PanelId;
            links.Add(link);

            nodesDb?.AddLink(link);

            OnNewLinkEvent?.Invoke(link);

            if (!started)
                return;

            input.Value = output.Value;

        }



        public void RemoveLink(Output output, Input input)
        {
            LogicalLink link = GetLink(output, input);

            if (link == null)
            {
                LogEngineError($"Can`t remove link from [{output.Id}] to [{input.Id}]. Does not exist.");
                return;
            }

            LogicalNode inputNode = GetInputOwner(input);
            LogicalNode outputNode = GetOutputOwner(output);
            LogEngineInfo($"Remove link from [{outputNode.GetType().Name}] to [{inputNode.GetType().Name}]");

            nodesDb?.RemoveLink(link.Id);

            OnRemoveLinkEvent?.Invoke(link);
            links.Remove(link);

            input.Value = null;
        }

        public void RemoveLink(LogicalLink link)
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

        public LogicalLink GetLink(Output output, Input input)
        {
            return links.FirstOrDefault(x => x.InputId == input.Id && x.OutputId == output.Id);
        }

        public LogicalLink GetLinkForInput(Input input)
        {
            return links.FirstOrDefault(x => x.InputId == input.Id);
        }

        public List<LogicalLink> GetLinksForOutput(Output output)
        {
            return links.Where(x => x.OutputId == output.Id).ToList();
        }

        public List<LogicalLink> GetLinksForNode(LogicalNode node)
        {
            List<LogicalLink> list = new List<LogicalLink>();

            foreach (var input in node.Inputs)
            {
                LogicalLink link = GetLinkForInput(input);
                if (link != null)
                    list.Add(link);
            }

            foreach (var output in node.Outputs)
            {
                List<LogicalLink> links = GetLinksForOutput(output);
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


        public string SerializeLinks()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            return JsonConvert.SerializeObject(links, settings);
        }

        public void DeserializeLinks(string json)
        {
            links = new List<LogicalLink>();

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            List<LogicalLink> newLinks = JsonConvert.DeserializeObject<List<LogicalLink>>(json, settings);

            foreach (var link in newLinks)
            {
                AddLink(link.OutputId, link.InputId);
            }

            OnLinksUpdatedEvent?.Invoke(links);
        }


        public string SerializeNodes()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            return JsonConvert.SerializeObject(nodes, settings);
        }

        public void DeserializeNodes(string json)
        {
            bool state = started;
            if (state)
                Stop();

            RemoveAllNodesAndLinks();

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                ObjectCreationHandling = ObjectCreationHandling.Replace
            };

            nodes = JsonConvert.DeserializeObject<List<LogicalNode>>(json, settings);

            foreach (var node in nodes)
            {
                node.OnDeserialize();
                LogEngineInfo($"New node [{node.GetType().Name}]");
            }

            if (state)
                Start();

            OnNodesUpdatedEvent?.Invoke(nodes);
        }

        // this list used for infinite loop detection
        List<Input> changedInputsStack=new List<Input>();

        public void OnInputChange(Input input)
        {
            if (!started)
                return;

            LogicalNode node = GetInputOwner(input.Id);

            if (changedInputsStack.Contains(input))
            {
                changedInputsStack.Remove(input);
                LogEngineError($"Infinite loop detected in Node [{node.Type}] [{node.Id}].");
                return;
            }
            changedInputsStack.Add(input);

            node.OnInputChange(input);

            if (node is LogicalNodePanel)
                GetNode(input.Id).Outputs[0].Value = input.Value;

            if (node is LogicalNodePanelOutput)
                GetOutput(node.Id).Value = input.Value;

            OnInputUpdatedEvent?.Invoke(input);

            changedInputsStack.Remove(input);
        }

        public void OnOutputChange(Output output)
        {
            if (!started)
                return;

            LogicalNode node = GetOutputOwner(output);
            if (node == null)
                return;

            OnOutputUpdatedEvent?.Invoke(output);

            node.OnOutputChange(output);

            //send state to linked nodes
            List<LogicalLink> list = links.Where(x => x.OutputId == output.Id).ToList();
            foreach (var link in list)
            {
                Input input = GetInput(link.InputId);
                if (input != null)
                {
                    input.Value = output.Value;
                }
            }

        }

        public LogicalNode GetInputOwner(Input input)
        {
            return nodes.FirstOrDefault(node => node.Inputs.Contains(input));
        }

        public LogicalNode GetOutputOwner(Output output)
        {
            return nodes.FirstOrDefault(node => node.Outputs.Contains(output));
        }

        public LogicalNode GetInputOwner(string inputId)
        {
            return (from node in nodes from input in node.Inputs where input.Id == inputId select node).FirstOrDefault();
        }

        public LogicalNode GetOutputOwner(string outputId)
        {
            return (from node in nodes from output in node.Outputs where output.Id == outputId select node).FirstOrDefault();
        }

      


        public void RemoveAllNodesAndLinks()
        {
            LogEngineInfo("Remove all nodes and links");

            nodesDb?.RemoveAllNodes();

            links = new List<LogicalLink>();
            nodes = new List<LogicalNode>();

            OnNodesUpdatedEvent?.Invoke(nodes);
            OnLinksUpdatedEvent?.Invoke(links);

        }



        public void RemoveAllLinks()
        {
            LogEngineInfo("Remove all links");

            links = new List<LogicalLink>();
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
    }
}
