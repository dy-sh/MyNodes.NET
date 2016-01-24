using System.Collections.Generic;
using System.Linq;
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
            List<MySensorsNode> nodes = engine.GetNodes().OfType<MySensorsNode>().ToList();
            foreach (var node in nodes)
            {
                engine.RemoveNode(node);
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
                oldNode.Title = node.GetSimpleName2();
                engine.UpdateNode(oldNode, true);
            }
        }

        private void CreateOrUpdateSensor(Sensor sensor)
        {
            MySensorsNodeOutput output = GetHardwarOutput(sensor);
            if (output == null)
            {
                MySensorsNode node = GetHardwareNode(sensor.nodeId);
                node.AddInputOutput(sensor);
                engine.UpdateNode(node, true);
            }
            else
            {
                engine.UpdateOutput(output.Id, sensor.state, sensor.GetSimpleName3());
            }

        }




        public MySensorsNode GetHardwareNode(int nodeId)
        {
            return engine.GetNodes()
                .OfType<MySensorsNode>()
                .FirstOrDefault(node => node.nodeId == nodeId);
        }

        public MySensorsNodeOutput GetHardwarOutput(int nodeId, int sensorId)
        {
            MySensorsNode oldNode = GetHardwareNode(nodeId);
            if (oldNode == null)
                return null;

            return oldNode.Outputs
                .Cast<MySensorsNodeOutput>()
                .FirstOrDefault(output => output.sensorId == sensorId);
        }



        public MySensorsNodeInput GetHardwareInput(int nodeId, int sensorId)
        {
            MySensorsNode oldNode = GetHardwareNode(nodeId);
            if (oldNode == null)
                return null;

            return oldNode.Inputs
                .Cast<MySensorsNodeInput>()
                .FirstOrDefault(input => input.sensorId == sensorId);
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
                if (GetHardwareNode(node.Id) != null)
                    continue;

                MySensorsNode newMySensorsNode = new MySensorsNode(node);
                list.Add(newMySensorsNode);
                engine.AddNode(newMySensorsNode);
            }
            return list;
        }

    }
}
