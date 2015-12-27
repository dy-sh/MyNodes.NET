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
            CreateInputsOutputs();
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
            InputHardware inputHardware = (InputHardware)input;

            Debug($"Hardware Node{inputHardware.nodeId} Sensor{inputHardware.sensorId} input: {input.Value}");

            gateway.SendSensorState(inputHardware.nodeId, inputHardware.sensorId, input.Value);

        }

        public override void OnOutputChange(Output output)
        {
            OutputHardware outputHardware = (OutputHardware)output;

            Debug($"Hardware Node{outputHardware.nodeId} Sensor{outputHardware.sensorId} output: {output.Value}");
        }

        public void CreateInputsOutputs()
        {

            Node node = gateway.GetNode(nodeId);

            for (int i = 0; i < node.sensors.Count; i++)
            {
                InputHardware input = new InputHardware { Name = node.sensors[i].GetSimpleName1() };
                input.sensorId = node.sensors[i].sensorId;
                input.nodeId = node.sensors[i].nodeId;
                Inputs.Add(input);

                OutputHardware output = new OutputHardware { Name = node.sensors[i].GetSimpleName1() };
                output.sensorId = node.sensors[i].sensorId;
                output.nodeId = node.sensors[i].nodeId;
                Outputs.Add(output);
            }
        }
    }
}
