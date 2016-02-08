//planer-pro copyright 2015 GPL - license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{

    public class SystemFileNode : Node
    {

        public SystemFileNode() : base(5, 1)
        {
            this.Title = "File";
            this.Type = "System/File";

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
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            //delete
            if (input == Inputs[4] && input.Value == "1")
            {
                string fileName = Inputs[0].Value;
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
                string fileName = Inputs[0].Value;
                try
                {
                    string text = Inputs[1].Value;
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
                string fileName = Inputs[0].Value;
                try
                {
                    string text = File.ReadAllText(fileName);
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