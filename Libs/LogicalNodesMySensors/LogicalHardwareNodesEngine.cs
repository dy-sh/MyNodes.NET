using System.Collections.Generic;
using MyNetSensors.Gateways;
using MyNetSensors.LogicalNodes;

namespace MyNetSensors.LogicalNodesMySensors
{
    public class LogicalHardwareNodesEngine
    {
        public static Gateway gateway;

        public static LogicalHardwareNodesEngine logicalHardwareNodesEngine;
        private static LogicalNodesEngine engine;


        public LogicalHardwareNodesEngine(Gateway gateway, LogicalNodesEngine engine)
        {
            LogicalHardwareNodesEngine.gateway = gateway;
            LogicalHardwareNodesEngine.logicalHardwareNodesEngine = this;
            LogicalHardwareNodesEngine.engine = engine;

            gateway.OnNewNodeEvent += CreateOrUpdateNode;
            gateway.OnNodeUpdatedEvent += CreateOrUpdateNode;
            gateway.OnNewSensorEvent += CreateOrUpdateSensor;
            gateway.OnSensorUpdatedEvent += CreateOrUpdateSensor;
            gateway.OnClearNodesListEvent += OnClearNodesListEvent;
            gateway.OnDeleteNodeEvent += OnDeleteNodeEvent;
            CreateAndAddHardwareNodes();
        }

        private void OnDeleteNodeEvent(Node node)
        {
            LogicalHardwareNode oldNode = GetHardwareNode(node.Id);
            if (oldNode != null)
                engine.RemoveNode(oldNode);
        }

        private void OnClearNodesListEvent()
        {
            engine.RemoveAllNodesAndLinks();
        }

        private void CreateOrUpdateSensor(Sensor sensor)
        {
            HardwareOutput output = GetHardwarOutput(sensor);
            if (output == null)
            {
                LogicalHardwareNode node = GetHardwareNode(sensor.nodeId);
                node.AddInputOutput(sensor);
                engine.UpdateNode(node);//for call event
            }
            else
            {
                engine.UpdateOutput(output.Id, sensor.state, sensor.GetSimpleName1());
            }

        }

        private void CreateOrUpdateNode(Node node)
        {
            LogicalHardwareNode oldNode = GetHardwareNode(node.Id);
            if (oldNode == null)
            {
                LogicalHardwareNode newHardwareNode = new LogicalHardwareNode(node);
                engine.AddNode(newHardwareNode);
            }
            else
            {
                oldNode.Title = node.GetSimpleName1();
                engine.UpdateNode(oldNode);
            }
        }


        public LogicalHardwareNode GetHardwareNode(int nodeId)
        {
            foreach (var n in engine.nodes)
            {
                if (n is LogicalHardwareNode)
                {
                    LogicalHardwareNode node = (LogicalHardwareNode)n;
                    if (node.nodeId == nodeId)
                        return node;
                }
            }
            return null;
        }

        public HardwareOutput GetHardwarOutput(int nodeId, int sensorId)
        {
            LogicalHardwareNode oldNode = GetHardwareNode(nodeId);
            if (oldNode == null)
                return null;

            foreach (HardwareOutput output in oldNode.Outputs)
            {
                if (output.sensorId == sensorId)
                    return output;
            }
            return null;
        }



        public HardwareInput GetHardwareInput(int nodeId, int sensorId)
        {
            LogicalHardwareNode oldNode = GetHardwareNode(nodeId);
            if (oldNode == null)
                return null;

            foreach (HardwareInput input in oldNode.Inputs)
            {
                if (input.sensorId == sensorId)
                    return input;
            }
            return null;
        }

        public HardwareOutput GetHardwarOutput(Sensor sensor)
        {
            return GetHardwarOutput(sensor.nodeId, sensor.sensorId);
        }



        public HardwareInput GetHardwareInput(Sensor sensor)
        {
            return GetHardwareInput(sensor.nodeId, sensor.sensorId);
        }

        public List<LogicalHardwareNode> CreateAndAddHardwareNodes()
        {
            var list = new List<LogicalHardwareNode>();

            foreach (var node in gateway.GetNodes())
            {
                if (GetHardwareNode(node.Id)!=null)
                    continue;

                LogicalHardwareNode newHardwareNode = new LogicalHardwareNode(node);
                list.Add(newHardwareNode);
                engine.AddNode(newHardwareNode);
            }
            return list;
        }



        public void RemoveAllNonHardwareNodes()
        {
            engine.LogEngine("Remove all non-hardware nodes");

            //to prevent changing of collection while writing 
            LogicalNode[] nodes = engine.nodes.ToArray();

            for (int i = 0; i < nodes.Length; i++)
            {
                if (!(nodes[i] is LogicalHardwareNode))
                    engine.RemoveNode(nodes[i]);
            }
        }
    }
}
