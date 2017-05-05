/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using System.Linq;

namespace MyNodes.Nodes
{
    public class ConnectionHubNode : Node
    {
        public ConnectionHubNode() : base("Connection", "Hub")
        {
            AddInput("In 1", DataType.Text, true);
            AddInput("In 2", DataType.Text, true);
            AddOutput();

            options.ResetOutputsIfAnyInputIsNull = true;

            Settings.Add("inputs", new NodeSetting(NodeSettingType.Number, "Inputs count", Inputs.Count.ToString()));
            Settings.Add("outputs", new NodeSetting(NodeSettingType.Number, "Outputs count", Outputs.Count.ToString()));
        }

        public override void OnInputChange(Input input)
        {
            foreach (var output in Outputs)
            {
                output.Value = input.Value;
            }
        }

        public override bool SetSettings(Dictionary<string, string> data)
        {
            int ins = int.Parse(data["inputs"]);
            int outs = int.Parse(data["outputs"]);

            if (ins < 1)
            {
                ins = 1;
            }
            else if (ins > 1000)
            {
                ins = 1000;
            }

            if (outs < 1)
            {
                outs = 1;
            }
            else if (outs > 1000)
            {
                outs = 1000;
            }

            data["inputs"] = ins.ToString();
            data["outputs"] = outs.ToString();

            ChangeInputsCount(ins);
            ChangeOutputsCount(outs);

            UpdateMeInEditor();
            UpdateMeInDb();

            return base.SetSettings(data);
        }

        private void ChangeOutputsCount(int count)
        {
            if (count > Outputs.Count)
            {
                int addCount = count - Outputs.Count;

                for (int i = 0; i < addCount; i++)
                {
                    AddOutput();
                }

                LogInfo($"Added {addCount} new outputs");
            }
            else if (count < Outputs.Count)
            {
                int remCount = Outputs.Count - count;

                for (int i = 0; i < remCount; i++)
                {
                    RemoveOutput(Outputs.Last());
                }

                LogInfo($"Removed {remCount} outputs");
            }
        }

        public void ChangeInputsCount(int count)
        {
            if (count > Inputs.Count)
            {
                int addCount = count - Inputs.Count;

                for (int i = 0; i < addCount; i++)
                {
                    AddInput($"In {Inputs.Count + 1}", DataType.Text, true);
                }

                LogInfo($"Added {addCount} new inputs");
            }
            else if (count < Inputs.Count)
            {
                int remCount = Inputs.Count - count;

                for (int i = 0; i < remCount; i++)
                {
                    RemoveInput(Inputs.Last());
                }

                LogInfo($"Removed {remCount} inputs");
            }
        }

        public override string GetNodeDescription()
        {
            return "This node sends a value from any of its inputs to all its outputs. <br/>" +
                   "The usage of this node can be very wide. Lets consider a few examples. <br/><br/>" +

                   "Connecting many-to-one. <br/>" +
                   "For example, you want to connect several different nodes to " +
                   "the input of one node. By default, this is impossible, " +
                   "because one input can only have one connection. " +
                   "But you can work around this limitation by using a hub. " +
                   "Connect multiple devices to the hub, then connect " +
                   "the hub to the input of the node.<br/><br/>" +

                   "Connecting one-to-many.  <br/>" +
                   "If you connect multiple nodes to one output of a node, " +
                   "node will send the value to all nodes that are connected, " +
                   "but you can't control the order in which it will do it. " +
                   "But you can work around this limitation by using a hub. " +
                   "Connect the output of the node to the hub and then " +
                   "connect the other nodes to the hub's outputs " +
                   "in the order in which they should receive the value. " +
                   "The hub sends a value on the outputs starting with the first output. <br/><br/>" +

                   "Connecting many-to-many.  <br/>" +
                   "Create many inputs and outputs in the hub to link several nodes. " +
                   "All devices on the outputs of the hub " +
                   "will receive a message from any node in the input.";
        }
    }
}