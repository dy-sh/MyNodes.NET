/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using MyNetSensors.Gateways;
using Newtonsoft.Json;

namespace MyNetSensors.LogicalNodes
{
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

        public event DebugMessageEventHandler OnDebugNodeMessage;
        public event DebugMessageEventHandler OnDebugEngineMessage;

        public event LogicalNodesEventHandler OnNodesUpdatedEvent;
        public event LogicalNodeEventHandler OnNewNodeEvent;
        public event LogicalNodeEventHandler OnNodeRemoveEvent;
        public event LogicalNodeEventHandler OnNodeUpdatedEvent;
        public event LogicalInputEventHandler OnInputUpdatedEvent;
        public event LogicalOutputEventHandler OnOutputUpdatedEvent;
        public event LogicalLinksEventHandler OnLinksUpdatedEvent;

        public delegate void LogicalNodeEventHandler(LogicalNode node);
        public delegate void LogicalNodesEventHandler(List<LogicalNode> nodes);
        public delegate void LogicalInputEventHandler(Input input);
        public delegate void LogicalOutputEventHandler(Output output);
        public delegate void LogicalLinksEventHandler(List<LogicalLink> output);

        public static LogicalHardwareNodesEngine hardwareNodesEngine;

        public LogicalNodesEngine(Gateway gateway, ILogicalNodesRepository db = null)
        {
            LogicalNodesEngine.logicalNodesEngine = this;

            this.db = db;

            hardwareNodesEngine = new LogicalHardwareNodesEngine(gateway, this);

            updateNodesTimer.Elapsed += UpdateNodes;
            updateNodesTimer.Interval = updateNodesInterval;

            if (db != null)
            {
                db.CreateDb();
                GetNodesFromRepository();
            }
        }


        public void Start()
        {
            hardwareNodesEngine.CreateAndAddHardwareNodes();

            started = true;
            updateNodesTimer.Start();

            UpdateStatesFromLinks();

            DebugEngine("Started");
        }


        public void Stop()
        {
            started = false;
            updateNodesTimer.Stop();
            DebugEngine("Stopped");
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

        public void AddNode(LogicalNode node)
        {
            nodes.Add(node);

            if (db != null)
                db.AddNode(node);

            DebugEngine($"New node {node.GetType().Name}");

            OnNewNodeEvent?.Invoke(node);
        }

        public void RemoveNode(LogicalNode node)
        {
            DebugEngine($"Remove node {node.GetType().Name}");
            OnNodeRemoveEvent?.Invoke(node);

            nodes.Remove(node);

            if (db != null)
                db.DeleteNode(node.Id);

        }

        public void UpdateNode(LogicalNode node)
        {
            DebugEngine($"Update node {node.GetType().Name}");

            LogicalNode oldNode = nodes.FirstOrDefault(x => x.Id == node.Id);

            oldNode.Inputs = node.Inputs;
            oldNode.Outputs = node.Outputs;
            oldNode.Position = node.Position;
            oldNode.Size = node.Size;
            oldNode.Title = node.Title;
            oldNode.Type = node.Type;

            if (db != null)
                db.UpdateNode(oldNode);

            OnNodeUpdatedEvent?.Invoke(node);
        }

        //public void UpdateOutput(Output output)
        //{

        //    Output oldOutput = GetOutput(output.Id);

        //    oldOutput.Name = output.Name;
        //    oldOutput.Value = output.Value;

        //}

        //public void UpdateInput(Input input)
        //{

        //    Input oldInput = GetInput(input.Id);

        //    oldInput.Name = input.Name;
        //    oldInput.Value = input.Value;

        //}

        public void UpdateOutput(string outputId, string value, string name = null)
        {
            Output oldOutput = GetOutput(outputId);

            if (name != null)
                oldOutput.Name = name;
            oldOutput.Value = value;

        }

        public void UpdateInput(string inputId, string value, string name = null)
        {
            Input oldInput = GetInput(inputId);

            if (name != null)
                oldInput.Name = name;
            oldInput.Value = value;

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
            DebugEngine($"New link from {outputNode.GetType().Name} to {inputNode.GetType().Name}");

            links.Add(new LogicalLink(output.Id, input.Id));

            OnLinksUpdatedEvent?.Invoke(links);

            if (!started)
                return;

            input.Value = output.Value;

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

        public LogicalNode GetNode(string id)
        {
            return nodes.FirstOrDefault(x => x.Id == id);
        }

        public Input GetInput(string id)
        {
            foreach (var node in nodes)
            {
                foreach (var input in node.Inputs)
                {
                    if (input.Id == id)
                        return input;
                }
            }
            return null;
        }

        public Output GetOutput(string id)
        {
            foreach (var node in nodes)
            {
                foreach (var output in node.Outputs)
                {
                    if (output.Id == id)
                        return output;
                }
            }
            return null;
        }






        public string SerializeLinks()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;
            return JsonConvert.SerializeObject(links, settings);
        }

        public void DeserializeLinks(string json)
        {
            links = new List<LogicalLink>();

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;

            List<LogicalLink> newLinks = JsonConvert.DeserializeObject<List<LogicalLink>>(json, settings);

            foreach (var link in newLinks)
            {
                AddLink(link.OutputId, link.InputId);
            }

            OnLinksUpdatedEvent?.Invoke(links);
        }


        public string SerializeNodes()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;
            return JsonConvert.SerializeObject(nodes, settings);
        }

        public void DeserializeNodes(string json)
        {
            bool state = started;
            if (state)
                Stop();

            RemoveAllNodesAndLinks();

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.ObjectCreationHandling = ObjectCreationHandling.Replace;

            nodes = JsonConvert.DeserializeObject<List<LogicalNode>>(json, settings);

            foreach (var node in nodes)
            {
                node.OnDeserialize();
                DebugEngine($"New node {node.GetType().Name}");
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
            foreach (var node in nodes)
            {
                if (node.Inputs.Contains(input))
                    return node;
            }
            return null;
        }

        public LogicalNode GetOutputOwner(Output output)
        {
            foreach (var node in nodes)
            {
                if (node.Outputs.Contains(output))
                    return node;
            }
            return null;
        }

        public LogicalNode GetInputOwner(string inputId)
        {
            foreach (var node in nodes)
            {
                foreach (var input in node.Inputs)
                {
                    if (input.Id == inputId)
                        return node;
                }
            }
            return null;
        }

        public LogicalNode GetOutputOwner(string outputId)
        {
            foreach (var node in nodes)
            {
                foreach (var output in node.Outputs)
                {
                    if (output.Id == outputId)
                        return node;
                }
            }
            return null;
        }

        public void OnInputChange(Input input)
        {

            //DebugNodes($"Input changed: {input.Name}");

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
            DebugEngine("Remove all nodes and links");

            if (db != null)
                db.DropNodes();

            links = new List<LogicalLink>();
            nodes = new List<LogicalNode>();

            OnNodesUpdatedEvent?.Invoke(nodes);
            OnLinksUpdatedEvent?.Invoke(links);

        }



        public void RemoveAllLinks()
        {
            DebugEngine("Remove all links");

            links = new List<LogicalLink>();
            OnLinksUpdatedEvent?.Invoke(links);
        }

        public void DebugNodes(string message)
        {
            if (OnDebugNodeMessage != null)
                OnDebugNodeMessage(message);
        }

        public void DebugEngine(string message)
        {
            if (OnDebugEngineMessage != null)
                OnDebugEngineMessage(message);
        }


    }
}
