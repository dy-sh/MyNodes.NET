//planer-pro copyright 2015 GPL - license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyNetSensors.Nodes
{

    public class SystemJsonFileNode : Node
    {

        public SystemJsonFileNode() : base(6, 1)
        {
            this.Title = "Json File";
            this.Type = "System/Json File";

            Inputs[0].Name = "File Name";
            Inputs[1].Name = "Key";
            Inputs[2].Name = "Value";
            Inputs[3].Name = "Read";
            Inputs[4].Name = "Write";
            Inputs[5].Name = "Delete File";
            Outputs[0].Name = "Value";

            Inputs[0].Type = DataType.Text;
            Inputs[1].Type = DataType.Text;
            Inputs[2].Type = DataType.Text;
            Inputs[3].Type = DataType.Logical;
            Inputs[4].Type = DataType.Logical;
            Inputs[5].Type = DataType.Logical;
            Outputs[0].Type = DataType.Text;

            options.LogOutputChanges = false;
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            //delete
            if (input == Inputs[5] && input.Value == "1")
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
            if (input == Inputs[4] && input.Value == "1")
            {
                string fileName = Inputs[0].Value;
                string key = Inputs[1].Value;
                string value = Inputs[2].Value;
                JObject json=null;
                try
                {
                    string text = File.ReadAllText(fileName);
                    json = JObject.Parse(text);
                }
                catch { }

                try
                {
                    if (json==null)
                        json=new JObject();
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
                string fileName = Inputs[0].Value;
                string key = Inputs[1].Value;
                try
                {
                    string text = File.ReadAllText(fileName);
                    JObject json = JObject.Parse(text);
                    Outputs[0].Value = json.GetValue(key).ToString();
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