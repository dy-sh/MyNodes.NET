using System;
using System.Collections.Generic;
using System.Linq;
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
            this.Title = node.GetSimpleName2();
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
            if (input.Value == null)
                return;

            MySensorsNodeInput mySensorsNodeInput = (MySensorsNodeInput)input;

            LogInfo($"Hardware Node{nodeId} Sensor{mySensorsNodeInput.sensorId} input: {input.Value}");

            if (MySensorsNodesEngine.gateway != null)
                MySensorsNodesEngine.gateway.SendSensorState(mySensorsNodeInput.nodeId, mySensorsNodeInput.sensorId,
                    input.Value);
            else
            {
                LogError($"Can`t send message to Node[{mySensorsNodeInput.nodeId}] Sensor[{mySensorsNodeInput.sensorId}]. Gateway is not connected.");
            }
        }

        public override void OnOutputChange(Output output)
        {
            if (output is MySensorsNodeOutput)
            {
                MySensorsNodeOutput mySensorsNodeOutput = (MySensorsNodeOutput) output;
                LogInfo(
                    $"Hardware Node{nodeId} Sensor{mySensorsNodeOutput.sensorId} output: {output.Value}");
            }
            else
            {
                LogInfo($"Hardware Node{nodeId} {output.Name} : {output.Value}");

            };


            base.OnOutputChange(output);
        }

        private void CreateInputsOutputs(Gateways.MySensors.Serial.Node node)
        {
            foreach (var sensor in node.sensors)
            {
                AddInputAndOutput(sensor);
            }
        }




        public void AddInputAndOutput(Sensor sensor)
        {
            MySensorsNodeInput input = new MySensorsNodeInput { Name = sensor.sensorId.ToString() };
            input.sensorId = sensor.sensorId;
            input.nodeId = sensor.nodeId;
            input.SlotIndex = sensor.sensorId;
            AddInput(input);

            MySensorsNodeOutput output = new MySensorsNodeOutput { Name = sensor.GetSimpleName3() };
            output.sensorId = sensor.sensorId;
            output.nodeId = sensor.nodeId;
            //todo output.Value = sensor.state;
            output.SlotIndex = sensor.sensorId;
            AddOutput(output);
        }

        public void UpdateBattery(int? batteryLevel)
        {
            Output output = Outputs.FirstOrDefault(x => x.Name == "Battery");
            if (output == null)
            {
                output = new Output { Name = "Battery" };
                output.SlotIndex = Int32.MaxValue;
               AddOutput(output);
            }

            output.Value = batteryLevel.ToString();
            UpdateMe();
            UpdateMeInDb();
        }
    }
}
