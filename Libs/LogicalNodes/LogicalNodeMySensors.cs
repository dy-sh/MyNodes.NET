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
        public int nodeId;
        private Gateway gateway;

        public LogicalNodeMySensors( int nodeId) : base(0, 0)
        {
            this.nodeId = nodeId;
            this.gateway = LogicalNodesEngine.gateway;
            gateway.OnSensorUpdatedEvent += OnSensorUpdatedEvent;
            CreateInputsOutputs();
        }

        public LogicalNodeMySensors() : base()
        {
            this.gateway = LogicalNodesEngine.gateway;
            gateway.OnSensorUpdatedEvent += OnSensorUpdatedEvent;
        }

        private void OnSensorUpdatedEvent(Sensor sensor)
        {
            if (sensor.nodeId!=nodeId)
                return;

            Output output = Outputs.FirstOrDefault(x => x.Id == sensor.sensorId);
            output.Value = sensor.state;
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            gateway.SendSensorState(nodeId, input.Id, input.Value);
        }

        public void CreateInputsOutputs()
        {

            Node node = gateway.GetNode(nodeId);

            for (int i = 0; i < node.sensors.Count; i++)
            {
                Input input = new Input {Name = node.sensors[i].GetSimpleName1()};
                input.Id = node.sensors[i].sensorId;
                Inputs.Add(input);

                Output output = new Output { Name = node.sensors[i].GetSimpleName1()};
                output.Id = node.sensors[i].sensorId;
                Outputs.Add(output);
            }
            ConnectInputs();
        }
    }
}
