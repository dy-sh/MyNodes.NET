//planer-pro copyright 2015 GPL - license.

using System;
using System.IO;

namespace MyNetSensors.Nodes
{
    public class SystemFileNode : Node
    {
        public SystemFileNode() : base("System", "File", 5, 1)
        {
            Inputs[0].Name = "File Name";
            Inputs[1].Name = "Text";
            Inputs[2].Name = "Read";
            Inputs[3].Name = "Write";
            Inputs[4].Name = "Clear";
            Outputs[0].Name = "Text";

            Inputs[0].Type = DataType.Text;
            Inputs[1].Type = DataType.Text;
            Inputs[2].Type = DataType.Logical;
            Inputs[3].Type = DataType.Logical;
            Inputs[4].Type = DataType.Logical;
            Outputs[0].Type = DataType.Text;

            options.LogOutputChanges = false;
            options.ProtectedAccess = true;
        }

        public override void OnInputChange(Input input)
        {
            //delete
            if (input == Inputs[4] && input.Value == "1")
            {
                var fileName = Inputs[0].Value;
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception)
                {
                    LogError($"Failed to delete file [{fileName}]");
                }
                Outputs[0].Value = null;
            }

            //write
            if (input == Inputs[3] && input.Value == "1")
            {
                var fileName = Inputs[0].Value;
                try
                {
                    var text = Inputs[1].Value;
                    File.AppendAllText(fileName, text);
                }
                catch (Exception)
                {
                    LogError($"Failed to write file [{fileName}]");
                }
            }

            //read
            if (input == Inputs[2] && input.Value == "1")
            {
                var fileName = Inputs[0].Value;
                try
                {
                    var text = File.ReadAllText(fileName);
                    Outputs[0].Value = text;
                }
                catch (Exception)
                {
                    LogError($"Failed to read file [{fileName}]");
                    Outputs[0].Value = null;
                }
            }
        }
    }
}