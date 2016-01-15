/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.LogicalNodes
{
    public abstract class LogicalNode
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

        public LogicalNode(int inputsCount, int outputsCount)
        {
            Id = Guid.NewGuid().ToString();

            Outputs = new List<Output>();
            for (int i = 0; i < outputsCount; i++)
            {
                if (outputsCount==1)
                Outputs.Add(new Output { Name = "Out" });
                else
                Outputs.Add(new Output { Name = $"Out {i+1}" });
            }

            Inputs = new List<Input>();
            for (int i = 0; i < inputsCount; i++)
            {
                if (inputsCount == 1)
                    Inputs.Add(new Input { Name = "In" });
                else
                    Inputs.Add(new Input { Name = $"In {i+1}" });
            }

            PanelId = "Main";
        }

        public LogicalNode()
        {
            Inputs = new List<Input>();
            Outputs = new List<Output>();
            PanelId = "Main";
        }



        public void LogInfo(string message)
        {
            LogicalNodesEngine.logicalNodesEngine.LogNodesInfo(message);
        }

        public void LogError(string message)
        {
            LogicalNodesEngine.logicalNodesEngine.LogNodesInfo(message);
        }

        public abstract void Loop();
        public abstract void OnInputChange(Input input);
        public virtual void OnOutputChange(Output output) { }

        public virtual void OnDeserialize() { }
    }





}
