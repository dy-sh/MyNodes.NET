/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace MyNetSensors.Nodes
{
    public class SystemJsonFileNode : Node
    {
        public SystemJsonFileNode() : base("System", "Json File")
        {
            AddInput("File Name", DataType.Text);
            AddInput("Key", DataType.Text);
            AddInput("Value", DataType.Logical);
            AddInput("Read", DataType.Logical, true);
            AddInput("Write", DataType.Logical, true);
            AddInput("Delete File", DataType.Logical, true);

            AddOutput("Value");
            
            options.LogOutputChanges = false;
            options.ProtectedAccess = true;
        }


        public override void OnInputChange(Input input)
        {
            //delete
            if (input == Inputs[5] && input.Value == "1")
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
            if (input == Inputs[4] && input.Value == "1")
            {
                var fileName = Inputs[0].Value;
                var key = Inputs[1].Value;
                var value = Inputs[2].Value;
                JObject json = null;
                try
                {
                    var text = File.ReadAllText(fileName);
                    json = JObject.Parse(text);
                }
                catch
                {
                }

                try
                {
                    if (json == null)
                        json = new JObject();
                    json.Remove(key);
                    json.Add(key, value);
                    File.WriteAllText(fileName, json.ToString());
                }
                catch (Exception)
                {
                    LogError($"Failed to write file [{fileName}]");
                }
            }

            //read
            if (input == Inputs[3] && input.Value == "1")
            {
                var fileName = Inputs[0].Value;
                var key = Inputs[1].Value;
                try
                {
                    var text = File.ReadAllText(fileName);
                    var json = JObject.Parse(text);
                    Outputs[0].Value = json.GetValue(key).ToString();
                }
                catch (Exception)
                {
                    LogError($"Failed to read file [{fileName}]");
                    Outputs[0].Value = null;
                }
            }
        }

        public override string GetNodeDescription()
        {
            return "This node can read and write Json file on the disk. <br/>" +
                   "Send the file name To the input named File Name. The path can be omitted. <br/>" +
                   "With logic inputs named Read, Write, Delete File you can perform the requested operation. <br/>" +
                   "Specify the key that you want to read/write. <br/>" +
                   "The value that you want to write, send to Value input. <br/>" +
                   "Read value will be sent to the output.";
        }
    }
}