/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNetSensors.Nodes
{
    public delegate void NodeEventHandler(Node node);
    public delegate void NodeUpdateEventHandler(Node node, bool writeNodeToDb);

    public abstract class Node
    {
        public string Id { get; set; }
        public string PanelId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public Position Position { get; set; }
        public Size Size { get; set; }
        //        public string flags { get; set; }
        public List<Input> Inputs { get; set; }
        public List<Output> Outputs { get; set; }

        protected NodesEngine engine { get; set; }
        
        public string PanelName
        {
            get
            {
                if (PanelId == engine?.MAIN_PANEL_ID)
                    return "Main Panel";

                return engine?.GetPanelNode(PanelId)?.Name;
            }
        }

        public Node(int inputsCount, int outputsCount)
        {
            Id = Guid.NewGuid().ToString();

            Outputs = new List<Output>();
            for (int i = 0; i < outputsCount; i++)
            {
                if (outputsCount == 1)
                    AddOutput(new Output { Name = "Out" });
                else
                    AddOutput(new Output { Name = $"Out {i + 1}" });
            }

            Inputs = new List<Input>();
            for (int i = 0; i < inputsCount; i++)
            {
                if (inputsCount == 1)
                    AddInput(new Input { Name = "In" });
                else
                    AddInput(new Input { Name = $"In {i + 1}" });
            }

            PanelId = "Main";
        }

        public Node()
        {
            Id = Guid.NewGuid().ToString();
            Inputs = new List<Input>();
            Outputs = new List<Output>();
            PanelId = "Main";
        }



        public void LogInfo(string message)
        {
            engine?.LogNodesInfo($"{PanelName}: {Title}: {message}");
        }

        public void LogError(string message)
        {
            engine?.LogNodesError($"{PanelName}: {Title}: {message}");
        }

        public void LogIncorrectInputValueError(Input input)
        {
            LogError($"Incorrect value in {input.Name}: [{input.Value}]");
        }

        public abstract void Loop();
        public abstract void OnInputChange(Input input);

        public virtual void OnOutputChange(Output output)
        {
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


        public virtual void AddInput(Input input)
        {
            Inputs.Add(input);

            if (engine != null)
                input.OnInputChange += engine.OnInputChange;
        }

        public virtual void AddOutput(Output output)
        {
            Outputs.Add(output);

            if (engine != null)
                output.OnOutputChange += engine.OnOutputChange;
        }

        public void ShowActivity()
        {
            engine.ShowNodeActivity(this);
        }

        public void ResetInputs()
        {
            foreach (var input in Inputs)
            {
                input.Value = null;
            }
        }

        public bool CheckIsBool(string value)
        {
            return value == "0" || value == "1";
        }

        public bool CheckIsDouble(string value)
        {
            double result;
            return Double.TryParse(value, out result);
        }

        public bool CheckIsInteger(string value)
        {
            int result;
            return Int32.TryParse(value, out result);
        }


        public bool CheckIsBoolOrNull(string value)
        {
            return value == "0" || value == "1" || value==null;
        }

        public bool CheckIsDoubleOrNull(string value)
        {
            if (value == null)
                return true;

            double result;
            return Double.TryParse(value, out result);
        }

        public bool CheckIsIntegerOrNull(string value)
        {
            if (value == null)
                return true;

            int result;
            return Int32.TryParse(value, out result);
        }
    }





}
