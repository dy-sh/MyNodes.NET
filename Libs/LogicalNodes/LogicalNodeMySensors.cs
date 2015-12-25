using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNetSensors.Gateways;

namespace MyNetSensors.LogicalNodes
{
    public class LogicalNodeMySensors : LogicalNode
    {
        private Node node;
        private Gateway gateway;

        public LogicalNodeMySensors(Gateway gateway, Node node) : base(node.sensors.Count, node.sensors.Count)
        {
            this.node = node;
            this.gateway = gateway;
            for (int i = 0; i < node.sensors.Count; i++)
            {
                Inputs[i].Name = node.sensors[i].GetSimpleName1();
                Outputs[i].Name = node.sensors[i].GetSimpleName1();

                Inputs[i].Id = node.sensors[i].sensorId;
                Outputs[i].Id = node.sensors[i].sensorId;
            }


            gateway.OnSensorUpdatedEvent += OnSensorUpdatedEvent;
        }

        private void OnSensorUpdatedEvent(Sensor sensor)
        {
            if (!node.sensors.Contains(sensor))
                return;

            Output output = Outputs.FirstOrDefault(x => x.Id == sensor.sensorId);
            output.Value = sensor.state;
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            gateway.SendSensorState(node.nodeId, input.Id, input.Value);

        }
    }
}
