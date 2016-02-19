using System;
using System.Collections.Generic;
using System.Linq;
using MyNetSensors.Gateways;
using MyNetSensors.Gateways.MySensors;
using Node = MyNetSensors.Nodes.Node;

namespace MyNetSensors.Nodes
{
    public class MySensorsNode : Node
    {
        public int nodeId;


        public MySensorsNode(Gateways.MySensors.Node node) : base("Hardware","MySensors")
        {
            this.nodeId = node.Id;
            Settings.Add("Name", new NodeSetting(NodeSettingType.Text, "Name", node.GetSimpleName2()));
            CreateInputsOutputs(node);
        }


        public MySensorsNode() : base("Hardware", "MySensors")
        {
        }



        public override void OnInputChange(Input input)
        {
            if (input.Value == null)
                return;

            MySensorsNodeInput mySensorsNodeInput = (MySensorsNodeInput)input;

            //LogInfo($"Hardware Node{nodeId} Sensor{mySensorsNodeInput.sensorId} input: {input.Value}");

            if (MySensorsNodesEngine.gateway != null)
                MySensorsNodesEngine.gateway.SendSensorState(mySensorsNodeInput.nodeId, mySensorsNodeInput.sensorId,
                    input.Value);
            else
            {
               // LogError($"Can`t send message to Node[{mySensorsNodeInput.nodeId}] Sensor[{mySensorsNodeInput.sensorId}]. Gateway is not connected.");
            }
        }

        //public override void OnOutputChange(Output output)
        //{
        //    if (output is MySensorsNodeOutput)
        //    {
        //        MySensorsNodeOutput mySensorsNodeOutput = (MySensorsNodeOutput) output;
        //        LogInfo(
        //            $"Hardware Node{nodeId} Sensor{mySensorsNodeOutput.sensorId} output: {output.Value}");
        //    }
        //    else
        //    {
        //        LogInfo($"Hardware Node{nodeId} {output.Name} : {output.Value}");

        //    };


        //    base.OnOutputChange(output);
        //}

        private void CreateInputsOutputs(Gateways.MySensors.Node node)
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

        public override string GetJsListGenerationScript()
        {
            return @"

            function MySensorsNode() {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MySensorsNode',
                    'Assembly': 'Nodes.MySensors'
                };
                this.clonable = false;
            }
            MySensorsNode.title = 'MySensors Node';
            MySensorsNode.skip_list = true;
            LiteGraph.registerNodeType('Hardware/MySensors', MySensorsNode);

            ";
        }
    }
}
