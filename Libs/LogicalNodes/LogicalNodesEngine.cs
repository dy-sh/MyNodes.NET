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

        public static Gateway gateway;
        private ILogicalNodesRepository db;

        private Timer updateNodesTimer = new Timer();
        public List<LogicalNode> nodes = new List<LogicalNode>();
        public List<LogicalLink> links = new List<LogicalLink>();

        private bool started = false;

        public static LogicalNodesEngine logicalNodesEngine;

        public event DebugMessageEventHandler OnDebugNodeMessage;
        public event DebugMessageEventHandler OnDebugEngineMessage;

        public LogicalNodesEngine(Gateway gateway, ILogicalNodesRepository db = null)
        {
            this.db = db;
            LogicalNodesEngine.gateway = gateway;
            LogicalNodesEngine.logicalNodesEngine = this;
            gateway.OnSensorUpdatedEvent += OnMySensorsNodeUpdated;


            gateway.OnClearNodesListEvent += OnClearNodesListEvent;

            updateNodesTimer.Elapsed += UpdateNodes;
            updateNodesTimer.Interval = updateNodesInterval;

            if (db != null)
            {
                db.CreateDb();
                GetNodesFromRepository();
            }

            Start();


        }

        private void OnMySensorsNodeUpdated(Sensor sensor)
        {
            foreach (var node in nodes)
            {
                if (node is LogicalNodeMySensors)
                {
                    if (((LogicalNodeMySensors)node).nodeId != sensor.nodeId)
                        continue;

                    foreach (var output in node.Outputs)
                    {
                        if (((OutputMySensors)output).sensorId != sensor.sensorId)
                            continue;

                        output.Value = sensor.state;
                    }
                }
            }
        }


        private void Start()
        {
            started = true;
            updateNodesTimer.Start();

            UpdateStatesFromLinks();

            DebugEngine("Started");
        }


        private void Stop()
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

        private void OnClearNodesListEvent()
        {
            RemoveAllNodesAndLinks();
            if (db != null)
                db.DropNodes();
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
        }

        public void RemoveNode(LogicalNode node)
        {
            DebugEngine($"Remove node {node.GetType().Name}");

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

            if (!started)
                return;

            input.Value = output.Value;
        }

        private void UpdateStatesFromLinks()
        {
            if (links==null)
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


        public List<LogicalNodeMySensors> CreateAndAddMySensorsNodes()
        {
            var list = new List<LogicalNodeMySensors>();

            foreach (var node in gateway.GetNodes())
            {
                LogicalNodeMySensors newNode = new LogicalNodeMySensors(node);
                list.Add(newNode);
                AddNode(newNode);
            }
            return list;
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

            if(state)
                Start();
        }

        public void OnOutputChange(Output output)
        {
            if (!started)
                return;

            LogicalNode owner= GetOutputOwner(output);
            owner.OnOutputChange(output);

            List<LogicalLink> list = links.Where(x => x.OutputId == output.Id).ToList();

            foreach (var link in list)
            {
                Input input = GetInput(link.InputId);
                if (input != null)
                    input.Value = output.Value;
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
        }


        public void RemoveAllNodesAndLinks()
        {
            DebugEngine("Remove all nodes and links");

            links=new List<LogicalLink>();
            nodes=new List<LogicalNode>();
        }

        public void RemoveAllNonHardwareNodes()
        {
            DebugEngine("Remove all non-hardware nodes");
            List<LogicalNode> list=new List<LogicalNode>();
            foreach (var node in nodes)
            {
                if (node is LogicalNodeMySensors)
                    list.Add(node);
            }
            nodes = list;
        }

        public void RemoveAllLinks()
        {
            DebugEngine("Remove all links");

            links = new List<LogicalLink>();
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

        public LogicalNode GetNode(string id)
        {
            return nodes.FirstOrDefault(x => x.Id == id);
        }
    }
}
