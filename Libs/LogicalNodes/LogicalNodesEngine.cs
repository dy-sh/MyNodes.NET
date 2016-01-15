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

        private ILogicalNodesRepository db;

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


        public LogicalNodesEngine(ILogicalNodesRepository db = null)
        {
            //var x= AppDomain.CurrentDomain.GetAssemblies()
            //           .SelectMany(assembly => assembly.GetTypes())
            //           .Where(type => type.IsSubclassOf(typeof(LogicalNode))).ToList();


            LogicalNodesEngine.logicalNodesEngine = this;

            this.db = db;


            updateNodesTimer.Elapsed += UpdateNodes;
            updateNodesTimer.Interval = updateNodesInterval;

            if (db != null)
            {
                db.CreateDb();
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
            if (db != null)
                nodes = db.GetAllNodes();

            OnNodesUpdatedEvent?.Invoke(nodes);
        }

        private void GetLinksFromRepository()
        {
            if (db != null)
                links = db.GetAllLinks();

            //remove link if node is not exist
            LogicalLink[] oldLinks = links.ToArray();
            foreach (var link in oldLinks)
            {
                if (GetInput(link.InputId) == null || GetOutput(link.OutputId) == null)
                {
                    db?.RemoveLink(link.Id);

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

            db?.AddNode(node);

            LogEngineInfo($"New node {node.GetType().Name}");

            OnNewNodeEvent?.Invoke(node);
        }


        private bool AddPanelInput(LogicalNodePanelInput node)
        {
            if (node.PanelId== MAIN_PANEL_ID)
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

            string name = "In " + new Random().Next(100);
            node.Title = name;

            Input input =new Input
            {
                Id= node.Id,
                Name = name
            };
            panel.Inputs.Add(input);

            UpdateNode(panel);

            return true;
        }

        private bool AddPanelOutput(LogicalNodePanelOutput node)
        {
            if (node.PanelId== MAIN_PANEL_ID)
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

            string name = "Out " + new Random().Next(100);
            node.Title = name;


            Output output =new Output
            {
                Id = node.Id,
                Name = name
            };
            panel.Outputs.Add(output);

            UpdateNode(panel);
            return true;
        }



        public void RemoveNode(LogicalNode node)
        {
            List<LogicalLink> links = GetLinksForNode(node);
            foreach (var link in links)
            {
                RemoveLink(link);
            }

            OnRemoveNodeEvent?.Invoke(node);
            LogEngineInfo($"Remove node {node.GetType().Name}");

            if (node is LogicalNodePanelInput)
                RemovePanelInput((LogicalNodePanelInput)node);
            else if (node is LogicalNodePanelOutput)
                RemovePanelOutput((LogicalNodePanelOutput)node);
            else if (node is LogicalNodePanel)
                RemovePanel((LogicalNodePanel)node);


            db?.RemoveNode(node.Id);

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


        private bool RemovePanelInput(LogicalNodePanelInput node)
        {
            LogicalNodePanel panel = GetPanelNode(node.PanelId);
            if (panel == null)
            {
                LogEngineError($"Can`t remove panel input. Panel [{node.PanelId}] does not exist.");
                return false;
            }
            Input input = GetInput(node.Id);
            panel.Inputs.Remove(input);
            UpdateNode(panel);
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
            panel.Outputs.Remove(output);
            UpdateNode(panel);
            return true;
        }


        public void UpdateNode(LogicalNode node)
        {
            LogEngineInfo($"Update node {node.GetType().Name}");

            LogicalNode oldNode = nodes.FirstOrDefault(x => x.Id == node.Id);

            oldNode.Inputs = node.Inputs;
            oldNode.Outputs = node.Outputs;
            oldNode.Position = node.Position;
            oldNode.Size = node.Size;
            oldNode.Title = node.Title;
            oldNode.Type = node.Type;

            db?.UpdateNode(oldNode);

            OnNodeUpdatedEvent?.Invoke(node);
        }



        public LogicalNodePanel GetPanelNode(string panelId)
        {
            return (LogicalNodePanel)nodes.FirstOrDefault(n => n is LogicalNodePanel && n.Id == panelId);
        }


        public void UpdateOutput(string outputId, string value, string name = null)
        {
            Output oldOutput = GetOutput(outputId);

            oldOutput.Value = value;

            if (name != null && name != oldOutput.Name)
            {
                oldOutput.Name = name;
                LogicalNode node = GetOutputOwner(oldOutput);
                db?.UpdateNode(node);
            }
        }

        public void UpdateInput(string inputId, string value, string name = null)
        {
            Input oldInput = GetInput(inputId);

            oldInput.Value = value;

            if (name != null && name != oldInput.Name)
            {
                oldInput.Name = name;
                LogicalNode node = GetInputOwner(oldInput);
                db?.UpdateNode(node);
            }

        }

        public void AddLink(string outputId, string inputId)
        {
            Input input = GetInput(inputId);
            Output output = GetOutput(outputId);
            AddLink(output, input);
        }

        public void AddLink(Output output, Input input)
        {
            LogicalNode inputNode = GetInputOwner(input);
            LogicalNode outputNode = GetOutputOwner(output);

            if (inputNode.PanelId != outputNode.PanelId)
            {
                LogEngineError($"Can`t create link from {outputNode.GetType().Name} to {inputNode.GetType().Name}. Nodes on different panels.");
                return;
            }

            //prevent two links to one input
            LogicalLink oldLink = GetLinkForInput(input);
            if (oldLink != null)
                RemoveLink(oldLink);

            LogEngineInfo($"New link from {outputNode.GetType().Name} to {inputNode.GetType().Name}");

            LogicalLink link = new LogicalLink(output.Id, input.Id);
            link.PanelId = inputNode.PanelId;
            links.Add(link);

            db?.AddLink(link);

            OnNewLinkEvent?.Invoke(link);

            if (!started)
                return;

            input.Value = output.Value;

        }

        public void RemoveLink(Output output, Input input)
        {
            LogicalNode inputNode = GetInputOwner(input);
            LogicalNode outputNode = GetOutputOwner(output);
            LogEngineInfo($"Remove link from {outputNode.GetType().Name} to {inputNode.GetType().Name}");

            LogicalLink link = GetLink(output, input);

            db?.RemoveLink(link.Id);

            OnRemoveLinkEvent?.Invoke(link);
            links.Remove(link);

            input.Value = null;
        }

        public void RemoveLink(LogicalLink link)
        {
            Output output = GetOutput(link.OutputId);
            Input input = GetInput(link.InputId);
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
                OnInputUpdatedEvent?.Invoke(input);
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
                LogEngineInfo($"New node {node.GetType().Name}");
            }

            if (state)
                Start();

            OnNodesUpdatedEvent?.Invoke(nodes);
        }

        public void OnOutputChange(Output output)
        {
            if (!started)
                return;

            LogicalNode owner = GetOutputOwner(output);
            if (owner == null)
                return;

            OnOutputUpdatedEvent?.Invoke(output);

            owner.OnOutputChange(output);

            List<LogicalLink> list = links.Where(x => x.OutputId == output.Id).ToList();

            foreach (var link in list)
            {
                Input input = GetInput(link.InputId);
                if (input != null)
                {
                    input.Value = output.Value;
                    OnInputUpdatedEvent?.Invoke(input);
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

        public void OnInputChange(Input input)
        {

            //LogNodes($"Input changed: {input.Name}");

            if (!started)
                return;

            LogicalNode node = GetInputOwner(input.Id);
            node.OnInputChange(input);

            if (node is LogicalNodePanel)
               GetNode(input.Id).Outputs[0].Value = input.Value;

            if (node is LogicalNodePanelOutput)
                GetOutput(node.Id).Value = input.Value;

            OnInputUpdatedEvent?.Invoke(input);
        }


        public void RemoveAllNodesAndLinks()
        {
            LogEngineInfo("Remove all nodes and links");

            db?.RemoveAllNodes();

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
