/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNetSensors.Nodes
{
    public delegate void NodeEventHandler(Node node);
    public delegate void NodeUpdateEventHandler(Node node, bool writeNodeToDb);

    public enum DataType
    {
        Text,
        Number,
        Logical
    }

    public abstract class Node
    {
        public string Id { get; set; }
        public string PanelId { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public Position Position { get; set; }
        public Size Size { get; set; }
        //        public string flags { get; set; }
        public List<Input> Inputs { get; set; }
        public List<Output> Outputs { get; set; }

        protected NodesEngine engine;

        protected NodeOptions options = new NodeOptions();

        public Dictionary<string, NodeSetting> Settings { get; set; }


        public string PanelName
        {
            get
            {
                if (PanelId == engine?.MAIN_PANEL_ID)
                    return "Main Panel";

                return engine?.GetPanelNode(PanelId)?.Settings["Name"].Value;
            }
        }

        public Node(string category, string type)
        {
            Id = Guid.NewGuid().ToString();

            Type = type;
            Category = category;

            Outputs = new List<Output>();
            Inputs = new List<Input>();
            Settings = new Dictionary<string, NodeSetting>();


            PanelId = "Main";
        }



        public void LogInfo(string message)
        {
            engine?.LogNodesInfo($"{PanelName}: {Type}: {message}");
        }

        public void LogError(string message)
        {
            engine?.LogNodesError($"{PanelName}: {Type}: {message}");
        }

        public void LogIncorrectInputValueError(Input input)
        {
            LogError($"Incorrect value in [{input.Name}]: [{input.Value}]");
        }

        public virtual void Loop() { }
        public virtual void OnInputChange(Input input) { }

        public virtual void OnOutputChange(Output output)
        {
            if (options.LogOutputChanges)
                LogInfo($"{output.Name}: [{output.Value ?? "NULL"}]");

            //send state to linked nodes
            List<Link> list = engine?.GetLinksForOutput(output);
            foreach (var link in list)
            {
                Input input = engine?.GetInput(link.InputId);
                if (input != null)
                {
                    input.Value = output.Value;
                }
            }
        }

        public virtual void OnRemove()
        {
            foreach (var input in Inputs)
                input.OnInputChange -= engine.OnInputChange;

            foreach (var output in Outputs)
                output.OnOutputChange -= engine.OnOutputChange;

            engine = null;
        }

        public virtual bool OnAddToEngine(NodesEngine engine)
        {
            this.engine = engine;

            foreach (var input in Inputs)
                input.OnInputChange += engine.OnInputChange;

            foreach (var output in Outputs)
                output.OnOutputChange += engine.OnOutputChange;

            return true;
        }

        public void UpdateMe()
        {
            engine?.UpdateNode(this);
        }

        public void UpdateMeInDb()
        {
            engine?.UpdateNodeInDb(this);
        }


        public void AddInput(Input input)
        {
            Inputs.Add(input);

            if (engine != null)
                input.OnInputChange += engine.OnInputChange;
        }

        public void AddOutput(Output output)
        {
            Outputs.Add(output);

            if (engine != null)
                output.OnOutputChange += engine.OnOutputChange;
        }

        public void AddInput(string name, DataType type = DataType.Text, bool isOptional = false)
        {
            if (name == null)
            {
                name = !Inputs.Any() ? "In" : "In " + (Inputs.Count + 1);
                if (Inputs.Count == 1 && Inputs[0].Name == "In")
                    Inputs[0].Name = "In 1";
            }



            AddInput(new Input(name, type, isOptional));
        }

        public void AddOutput(string name, DataType type = DataType.Text)
        {
            if (name == null)
            {
                name = !Outputs.Any() ? "Out" : "Out " + (Outputs.Count + 1);
                if (Outputs.Count == 1 && Outputs[0].Name == "Out")
                    Outputs[0].Name = "Out 1";
            }

            AddOutput(new Output(name, type));
        }

        public void AddInput(DataType type = DataType.Text, bool isOptional = false)
        {
            AddInput(null, type, isOptional);
        }

        public void AddOutput(DataType type = DataType.Text)
        {
            AddOutput(null, type);
        }


        public void RemoveInput(Input input)
        {
            if (input == null || !Inputs.Contains(input))
            {
                LogError("Can`t remove input. Does not exist.");
                return;
            }

            var link = engine.GetLinkForInput(input);
            if (link != null)
                engine.RemoveLink(link);

            Inputs.Remove(input);
        }

        public void RemoveOutput(Output output)
        {
            if (output == null || !Outputs.Contains(output))
            {
                LogError("Can`t remove output. Does not exist.");
                return;
            }

            var links = engine.GetLinksForOutput(output);
            foreach (var link in links)
                engine.RemoveLink(link);

            Outputs.Remove(output);
        }


        public void RemoveOutput(string name)
        {
            Output output = Outputs.FirstOrDefault(x => x.Name == name);
            RemoveOutput(output);
        }


        public void RemoveInput(string name)
        {
            Input input = Inputs.FirstOrDefault(x => x.Name == name);
            RemoveInput(input);
        }


        public Output GetOutput(string name)
        {
            Output output = Outputs.FirstOrDefault(x => x.Name == name);
            return output;
        }


        public Input GetInput(string name)
        {
            Input input = Inputs.FirstOrDefault(x => x.Name == name);
            return input;
        }


        public void ShowActivity()
        {
            engine.ShowNodeActivity(this);
        }

        public void ResetInputs()
        {
            foreach (var input in Inputs)
                input.Value = null;
        }

        public void ResetOutputs()
        {
            foreach (var output in Outputs)
                if (output.Value != null)
                    output.Value = null;
        }

        public virtual void CheckInputDataTypeIsCorrect(Input input)
        {
            if (input.Value == null)
                return;

            if (input.Type == DataType.Text)
                return;

            if (input.Type == DataType.Logical)
            {
                if (input.Value != null && input.Value != "0" && input.Value != "1")
                {
                    LogIncorrectInputValueError(input);
                    input.SetValueWithoutUpdate(null);
                }
            }

            if (input.Type == DataType.Number)
            {
                double num;

                if (!double.TryParse(input.Value, out num))
                {
                    LogIncorrectInputValueError(input);
                    input.SetValueWithoutUpdate(null);
                }
            }
        }

        public virtual bool SetSettings(Dictionary<string, string> data)
        {
            foreach (var d in data)
            {
                Settings[d.Key].Value = d.Value;
            }

            UpdateMe();
            UpdateMeInDb();


            LogInfo($"Settings changed");

            return true;
        }

        public virtual string GetJsListGenerationScript()
        {
            var t = this.GetType();
            string className = this.GetType().Name;
            string fullClassName = this.GetType().FullName;
            string assembly = this.GetType().Assembly.ToString();
            return @"

            //" + className + @"
            function " + className + @" () {
                this.properties = {
                    'ObjectType': '" + fullClassName + @"',
                    'Assembly': '" + assembly + @"'
                };
            }
            " + className + @".title = '" + this.Type + @"';
            LiteGraph.registerNodeType('" + this.Category + "/" + this.Type + "', " + className + @");

            ";
        }

        public virtual string GetNodeDescription()
        {
            return "This node does not have a description.";
        }

        public NodeOptions GetNodeOptions()
        {
            return options;
        }



        public void AddNodeData(string data, int? maxDbRecords = null)
        {
            engine?.dataDb?.AddNodeData(new NodeData(Id, data), maxDbRecords);
        }

        public List<NodeData> GetAllNodeData()
        {
            return engine?
                .dataDb?
                .GetAllNodeDataForNode(Id)?
                .OrderBy(x=>x.DateTime)
                .ToList();
        }

        public void RemoveAllNodeData()
        {
            engine?.dataDb?.RemoveAllNodeDataForNode(Id);
        }
    }





}
