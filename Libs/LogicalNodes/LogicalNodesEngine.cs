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

        private Gateway gateway;
        private ILogicalNodesRepository db;

        private Timer updateNodesTimer = new Timer();
        public List<LogicalNode> nodes = new List<LogicalNode>();
        private List<LogicalNodesLink> links = new List<LogicalNodesLink>();

        private bool started = false;

        public LogicalNodesEngine(Gateway gateway, ILogicalNodesRepository db=null)
        {
            this.db = db;
            this.gateway = gateway;

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


        private void Start()
        {
            started = true;
            updateNodesTimer.Start();
        }
        private void Stop()
        {
            started = false;
            updateNodesTimer.Stop();
        }

        public bool IsStarted()
        {
            return started;
        }

        public void GetNodesFromRepository()
        {
            if (db!=null)
            nodes = db.GetAllNodes();
        }

        private void UpdateNodes(object sender, ElapsedEventArgs e)
        {
            if(nodes==null)
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
            nodes.Clear();
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
        }

        public void RemoveNode(LogicalNode node)
        {
            nodes.Remove(node);

            if (db != null)
                db.DeleteNode(node.Id);
        }

        public void UpdateNode(LogicalNode node)
        {
            LogicalNode oldNode = nodes.FirstOrDefault(x=>x.Id==node.Id);

            oldNode.Inputs = node.Inputs;
            oldNode.Outputs = node.Outputs;
            oldNode.Position = node.Position;
            oldNode.Size = node.Size;
            oldNode.Title = node.Title;
            oldNode.Type = node.Type;

            if (db != null)
                db.UpdateNode(oldNode);
        }

        public void AddLink(Output output, Input input)
        {
            output.OnOutputChange += input.SetValue;
            input.Value = output.Value;
        }


        public string GetJsonFromNodes()
        {
            JsonSerializerSettings settings=new JsonSerializerSettings();
            settings.TypeNameHandling=TypeNameHandling.All;
            return JsonConvert.SerializeObject(nodes, settings);
        }

        public void SetNodesFromJson(string json)
        {
            JsonSerializerSettings settings=new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;
            
            nodes = JsonConvert.DeserializeObject<List<LogicalNode>>(json, settings);

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is LogicalNodeMySensors)
                {
                    nodes[i] = new LogicalNodeMySensors(gateway, ((LogicalNodeMySensors)nodes[i]).nodeId);
                }
            }
        }
    }
}
