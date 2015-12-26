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

        public LogicalNodeMySensors(int nodeId) : base(0, 0)
        {
            this.nodeId = nodeId;
            this.gateway = LogicalNodesEngine.gateway;
            CreateInputsOutputs();
        }

        public LogicalNodeMySensors() : base()
        {
            this.gateway = LogicalNodesEngine.gateway;
        }



        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            InputMySensors inputMySensors = (InputMySensors)input;

            Debug($"Hardware Node{inputMySensors.nodeId} Sensor{inputMySensors.sensorId} input: {input.Value}");

            gateway.SendSensorState(inputMySensors.nodeId, inputMySensors.sensorId, input.Value);

        }

        public override void OnOutputChange(Output output)
        {
            OutputMySensors outputMySensors = (OutputMySensors)output;

            Debug($"Hardware Node{outputMySensors.nodeId} Sensor{outputMySensors.sensorId} output: {output.Value}");
        }

        public void CreateInputsOutputs()
        {

            Node node = gateway.GetNode(nodeId);

            for (int i = 0; i < node.sensors.Count; i++)
            {
                InputMySensors input = new InputMySensors { Name = node.sensors[i].GetSimpleName1() };
                input.sensorId = node.sensors[i].sensorId;
                input.nodeId = node.sensors[i].nodeId;
                Inputs.Add(input);

                OutputMySensors output = new OutputMySensors { Name = node.sensors[i].GetSimpleName1() };
                output.sensorId = node.sensors[i].sensorId;
                output.nodeId = node.sensors[i].nodeId;
                Outputs.Add(output);
            }
        }
    }
}
