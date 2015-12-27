using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNetSensors.Gateways;

namespace MyNetSensors.LogicalNodes
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

            CreateAndAddHardwareNodes();

        }

        private void OnClearNodesListEvent()
        {
            engine.RemoveAllNodesAndLinks();
        }

        private void CreateOrUpdateSensor(Sensor sensor)
        {
            foreach (var node in engine.nodes)
            {
                if (node is LogicalHardwareNode)
                {
                    if (((LogicalHardwareNode)node).nodeId != sensor.nodeId)
                        continue;

                    foreach (var output in node.Outputs)
                    {
                        if (((OutputHardware)output).sensorId != sensor.sensorId)
                            continue;

                        output.Value = sensor.state;
                    }
                }
            }
        }

        private void CreateOrUpdateNode(Node node)
        {
            LogicalHardwareNode newHardwareNode = new LogicalHardwareNode(node);
            engine.AddNode(newHardwareNode);
        }


        private LogicalHardwareNode GetHardwareNode(int nodeId)
        {
            foreach (var node in engine.nodes)
            {
                if (node is LogicalHardwareNode)
                {
                  //  if ((LogicalHardwareNode node).nodeId == nodeId)
                   // return node;
                }
            }
            return null;
        }





        public List<LogicalHardwareNode> CreateAndAddHardwareNodes()
        {
            var list = new List<LogicalHardwareNode>();

            foreach (var node in gateway.GetNodes())
            {
                LogicalHardwareNode newHardwareNode = new LogicalHardwareNode(node);
                list.Add(newHardwareNode);
                engine.AddNode(newHardwareNode);
            }
            return list;
        }

   

        public void RemoveAllNonHardwareNodes()
        {
            engine.DebugEngine("Remove all non-hardware nodes");

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
