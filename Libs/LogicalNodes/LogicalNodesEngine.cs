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
        //If you have tons of logical nodes, and system perfomance decreased, increase this value,
        //and you will get less nodes updating frequency 
        private int updateNodesInterval = 1;

        private ILogicalNodesRepository db;

        private Timer updateNodesTimer = new Timer();
        public List<LogicalNode> nodes = new List<LogicalNode>();
        public List<LogicalLink> links = new List<LogicalLink>();

        private bool started = false;

        public static LogicalNodesEngine logicalNodesEngine;

        public event LogMessageEventHandler OnLogNodeMessage;
        public event LogMessageEventHandler OnLogEngineMessage;

        public event LogicalNodesEventHandler OnNodesUpdatedEvent;
        public event LogicalNodeEventHandler OnNewNodeEvent;
        public event LogicalNodeEventHandler OnNodeDeleteEvent;
        public event LogicalNodeEventHandler OnNodeUpdatedEvent;
        public event LogicalInputEventHandler OnInputUpdatedEvent;
        public event LogicalOutputEventHandler OnOutputUpdatedEvent;
        public event LogicalLinkEventHandler OnNewLinkEvent;
        public event LogicalLinkEventHandler OnLinkDeleteEvent;
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

            LogEngine("Started");
        }


        public void Stop()
        {
            started = false;
            updateNodesTimer.Stop();
            LogEngine("Stopped");
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

            //delete link if node is not exist
            LogicalLink[] oldLinks = links.ToArray();
            foreach (var link in oldLinks)
            {
                if (GetInput(link.InputId) == null || GetOutput(link.OutputId) == null)
                {
                    db?.DeleteLink(link.Id);

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
            nodes.Add(node);

            db?.AddNode(node);

            LogEngine($"New node {node.GetType().Name}");

            OnNewNodeEvent?.Invoke(node);
        }

        public void RemoveNode(LogicalNode node)
        {

            List<LogicalLink> links = GetLinksForNode(node);
            foreach (var link in links)
            {
                    DeleteLink(link);
            }

            OnNodeDeleteEvent?.Invoke(node);
            LogEngine($"Remove node {node.GetType().Name}");

            db?.DeleteNode(node.Id);

            nodes.Remove(node);
        }



        public void UpdateNode(LogicalNode node)
        {
            LogEngine($"Update node {node.GetType().Name}");

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



        public void UpdateOutput(string outputId, string value, string name = null)
        {
            Output oldOutput = GetOutput(outputId);

            oldOutput.Value = value;

            if (name != null && name!= oldOutput.Name)
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

            //prevent two links to one input
            LogicalLink oldLink = GetLinkForInput(input);
            if (oldLink!=null)
                DeleteLink(oldLink);

            LogEngine($"New link from {outputNode.GetType().Name} to {inputNode.GetType().Name}");

            LogicalLink link = new LogicalLink(output.Id, input.Id);
            links.Add(link);

            db?.AddLink(link);

            OnNewLinkEvent?.Invoke(link);

            if (!started)
                return;

            input.Value = output.Value;

        }

        public void DeleteLink(Output output, Input input)
        {
            LogicalNode inputNode = GetInputOwner(input);
            LogicalNode outputNode = GetOutputOwner(output);
            LogEngine($"Delete link from {outputNode.GetType().Name} to {inputNode.GetType().Name}");

            LogicalLink link = GetLink(output, input);

            db?.DeleteLink(link.Id);

            OnLinkDeleteEvent?.Invoke(link);
            links.Remove(link);
        }

        public void DeleteLink(LogicalLink link)
        {
            Output output=GetOutput(link.OutputId);
            Input input=GetInput(link.InputId);
            DeleteLink(output,input);
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
                LogEngine($"New node {node.GetType().Name}");
            }

            if (state)
                Start();

            OnNodesUpdatedEvent?.Invoke(nodes);
        }

        public void OnOutputChange(Output output)
        {
            if (!started)
                return;

            OnOutputUpdatedEvent?.Invoke(output);


            LogicalNode owner = GetOutputOwner(output);
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

            foreach (var node in nodes)
            {
                if (node.Inputs.Contains(input))
                    node.OnInputChange(input);
            }

            OnInputUpdatedEvent?.Invoke(input);
        }


        public void RemoveAllNodesAndLinks()
        {
            LogEngine("Remove all nodes and links");

            db?.DropNodes();

            links = new List<LogicalLink>();
            nodes = new List<LogicalNode>();

            OnNodesUpdatedEvent?.Invoke(nodes);
            OnLinksUpdatedEvent?.Invoke(links);

        }



        public void RemoveAllLinks()
        {
            LogEngine("Remove all links");

            links = new List<LogicalLink>();
            OnLinksUpdatedEvent?.Invoke(links);
        }

        public void LogNodes(string message)
        {
            OnLogNodeMessage?.Invoke(message);
        }

        public void LogEngine(string message)
        {
            OnLogEngineMessage?.Invoke(message);
        }
    }
}
