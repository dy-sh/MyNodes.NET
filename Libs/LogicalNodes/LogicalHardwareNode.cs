using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNetSensors.Gateways;

namespace MyNetSensors.LogicalNodes
{
    public class LogicalHardwareNode : LogicalNode
    {
        public int nodeId;
        private Gateway gateway;

        public LogicalHardwareNode(Node node) : base(0, 0)
        {
            this.nodeId = node.nodeId;
            this.Title = node.GetSimpleName1();
            this.gateway = LogicalHardwareNodesEngine.gateway;
            this.Type = "Nodes/HardwareNode";
            CreateInputsOutputs(node);
        }

        public LogicalHardwareNode() : base()
        {
            this.gateway = LogicalHardwareNodesEngine.gateway;
        }



        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            HardwareInput hardwareInput = (HardwareInput)input;

            Debug($"Hardware Node{hardwareInput.nodeId} Sensor{hardwareInput.sensorId} input: {input.Value}");

            gateway.SendSensorState(hardwareInput.nodeId, hardwareInput.sensorId, input.Value);

        }

        public override void OnOutputChange(Output output)
        {
            HardwareOutput hardwareOutput = (HardwareOutput)output;

            Debug($"Hardware Node{hardwareOutput.nodeId} Sensor{hardwareOutput.sensorId} output: {output.Value}");
        }

        private void CreateInputsOutputs(Node node)
        {
            foreach (var sensor in node.sensors)
            {
                AddInputOutput(sensor);
            }
        }


        public void AddInputOutput(Sensor sensor)
        {
            HardwareInput input = new HardwareInput { Name = sensor.GetSimpleName1() };
            input.sensorId = sensor.sensorId;
            input.nodeId = sensor.nodeId;
            Inputs.Add(input);

            HardwareOutput output = new HardwareOutput { Name = sensor.GetSimpleName1() };
            output.sensorId = sensor.sensorId;
            output.nodeId = sensor.nodeId;
           //todo output.Value = sensor.state;
            Outputs.Add(output);
        }
    }
}
