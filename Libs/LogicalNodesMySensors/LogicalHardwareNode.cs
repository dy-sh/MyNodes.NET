using MyNetSensors.Gateways;
using MyNetSensors.LogicalNodes;

namespace MyNetSensors.LogicalNodesMySensors
{
    public class LogicalHardwareNode : LogicalNode
    {
        public int nodeId;


        public LogicalHardwareNode(Node node) : base(0, 0)
        {
            this.nodeId = node.Id;
            this.Title = node.GetSimpleName1();
            this.Type = "Nodes/HardwareNode";
            CreateInputsOutputs(node);
        }


        public LogicalHardwareNode() : base(0, 0)
        {

        }



        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (input.Value==null)
                return;
            
            HardwareInput hardwareInput = (HardwareInput)input;

            Log($"Hardware Node{hardwareInput.nodeId} Sensor{hardwareInput.sensorId} input: {input.Value}");

            LogicalHardwareNodesEngine.gateway.SendSensorState(hardwareInput.nodeId, hardwareInput.sensorId, input.Value);

        }

        public override void OnOutputChange(Output output)
        {
            HardwareOutput hardwareOutput = (HardwareOutput)output;

            Log($"Hardware Node{hardwareOutput.nodeId} Sensor{hardwareOutput.sensorId} output: {output.Value}");
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
