using MyNetSensors.Gateways;
using MyNetSensors.Gateways.MySensors.Serial;
using Node = MyNetSensors.Nodes.Node;

namespace MyNetSensors.Nodes
{
    public class MySensorsNode : Node
    {
        public int nodeId;


        public MySensorsNode(Gateways.MySensors.Serial.Node node) : base(0, 0)
        {
            this.nodeId = node.Id;
            this.Title = node.GetSimpleName1();
            this.Type = "Nodes/Hardware";
            CreateInputsOutputs(node);
        }


        public MySensorsNode() : base(0, 0)
        {

        }



        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (input.Value==null)
                return;
            
            MySensorsNodeInput mySensorsNodeInput = (MySensorsNodeInput)input;

            LogInfo($"Hardware Node{mySensorsNodeInput.nodeId} Sensor{mySensorsNodeInput.sensorId} input: {input.Value}");

            MySensorsNodesEngine.gateway.SendSensorState(mySensorsNodeInput.nodeId, mySensorsNodeInput.sensorId, input.Value);

        }

        public override void OnOutputChange(Output output)
        {
            MySensorsNodeOutput mySensorsNodeOutput = (MySensorsNodeOutput)output;

            LogInfo($"Hardware Node{mySensorsNodeOutput.nodeId} Sensor{mySensorsNodeOutput.sensorId} output: {output.Value}");
        }

        private void CreateInputsOutputs(Gateways.MySensors.Serial.Node node)
        {
            foreach (var sensor in node.sensors)
            {
                AddInputOutput(sensor);
            }
        }


        public void AddInputOutput(Sensor sensor)
        {
            MySensorsNodeInput input = new MySensorsNodeInput { Name = sensor.GetSimpleName1() };
            input.sensorId = sensor.sensorId;
            input.nodeId = sensor.nodeId;
            Inputs.Add(input);

            MySensorsNodeOutput output = new MySensorsNodeOutput { Name = sensor.GetSimpleName1() };
            output.sensorId = sensor.sensorId;
            output.nodeId = sensor.nodeId;
           //todo output.Value = sensor.state;
            Outputs.Add(output);
        }
    }
}
