using System.Collections.Generic;
using MyNetSensors.Gateways;
using MyNetSensors.Gateways.MySensors.Serial;
using Node = MyNetSensors.Nodes.Node;

namespace MyNetSensors.Nodes
{
    public class MySensorsNodesEngine
    {
        public static Gateway gateway;

        public static MySensorsNodesEngine mySensorsNodesEngine;
        private static NodesEngine engine;


        public MySensorsNodesEngine(Gateway gateway, NodesEngine engine)
        {
            MySensorsNodesEngine.gateway = gateway;
            MySensorsNodesEngine.mySensorsNodesEngine = this;
            MySensorsNodesEngine.engine = engine;

            gateway.OnNewNodeEvent += CreateOrUpdateNode;
            gateway.OnNodeUpdatedEvent += CreateOrUpdateNode;
            gateway.OnNewSensorEvent += CreateOrUpdateSensor;
            gateway.OnSensorUpdatedEvent += CreateOrUpdateSensor;
            gateway.OnRemoveAllNodesEvent += OnRemoveAllNodesEvent;
            gateway.OnRemoveNodeEvent += OnRemoveNodeEvent;
            CreateAndAddHardwareNodes();
        }

        private void OnRemoveNodeEvent(Gateways.MySensors.Serial.Node node)
        {
            MySensorsNode oldNode = GetHardwareNode(node.Id);
            if (oldNode != null)
                engine.RemoveNode(oldNode);
        }

        private void OnRemoveAllNodesEvent()
        {
            engine.RemoveAllNodesAndLinks();
        }

        private void CreateOrUpdateSensor(Sensor sensor)
        {
            MySensorsNodeOutput output = GetHardwarOutput(sensor);
            if (output == null)
            {
                MySensorsNode node = GetHardwareNode(sensor.nodeId);
                node.AddInputOutput(sensor);
                engine.UpdateNode(node,true);//for call event
            }
            else
            {
                engine.UpdateOutput(output.Id, sensor.state, sensor.GetSimpleName1());
            }

        }

        private void CreateOrUpdateNode(Gateways.MySensors.Serial.Node node)
        {
            MySensorsNode oldNode = GetHardwareNode(node.Id);
            if (oldNode == null)
            {
                MySensorsNode newMySensorsNode = new MySensorsNode(node);
                engine.AddNode(newMySensorsNode);
            }
            else
            {
                //todo update inputs names
                oldNode.Title = node.GetSimpleName1();
                engine.UpdateNode(oldNode,true);
            }
        }


        public MySensorsNode GetHardwareNode(int nodeId)
        {
            foreach (var n in engine.nodes)
            {
                if (n is MySensorsNode)
                {
                    MySensorsNode node = (MySensorsNode)n;
                    if (node.nodeId == nodeId)
                        return node;
                }
            }
            return null;
        }

        public MySensorsNodeOutput GetHardwarOutput(int nodeId, int sensorId)
        {
            MySensorsNode oldNode = GetHardwareNode(nodeId);
            if (oldNode == null)
                return null;

            foreach (MySensorsNodeOutput output in oldNode.Outputs)
            {
                if (output.sensorId == sensorId)
                    return output;
            }
            return null;
        }



        public MySensorsNodeInput GetHardwareInput(int nodeId, int sensorId)
        {
            MySensorsNode oldNode = GetHardwareNode(nodeId);
            if (oldNode == null)
                return null;

            foreach (MySensorsNodeInput input in oldNode.Inputs)
            {
                if (input.sensorId == sensorId)
                    return input;
            }
            return null;
        }

        public MySensorsNodeOutput GetHardwarOutput(Sensor sensor)
        {
            return GetHardwarOutput(sensor.nodeId, sensor.sensorId);
        }



        public MySensorsNodeInput GetHardwareInput(Sensor sensor)
        {
            return GetHardwareInput(sensor.nodeId, sensor.sensorId);
        }

        public List<MySensorsNode> CreateAndAddHardwareNodes()
        {
            var list = new List<MySensorsNode>();

            foreach (var node in gateway.GetNodes())
            {
                if (GetHardwareNode(node.Id)!=null)
                    continue;

                MySensorsNode newMySensorsNode = new MySensorsNode(node);
                list.Add(newMySensorsNode);
                engine.AddNode(newMySensorsNode);
            }
            return list;
        }



        public void RemoveAllNonHardwareNodes()
        {
            engine.LogEngineInfo("Remove all non-hardware nodes");

            //to prevent changing of collection while writing 
            Node[] nodes = engine.nodes.ToArray();

            for (int i = 0; i < nodes.Length; i++)
            {
                if (!(nodes[i] is MySensorsNode))
                    engine.RemoveNode(nodes[i]);
            }
        }
    }
}
