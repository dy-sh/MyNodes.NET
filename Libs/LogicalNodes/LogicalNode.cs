/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNetSensors.Gateways;

namespace MyNetSensors.LogicalNodes
{
    public abstract class LogicalNode
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public Position Position { get; set; }
        public Size Size { get; set; }
        //        public string flags { get; set; }
        public List<Input> Inputs { get; set; }
        public List<Output> Outputs { get; set; }

        public LogicalNode(int inputsCount, int outputsCount)
        {
            Position=new Position();
            Size=new Size();

            Outputs = new List<Output>();
            for (int i = 0; i < outputsCount; i++)
            {
                Outputs.Add(new Output { Name = $"Out {i}"});
            }

            Inputs = new List<Input>();
            for (int i = 0; i < inputsCount; i++)
            {
                Input input = new Input { Name = $"In {i}" };
                Inputs.Add(input);
            }
            ConnectInputs();
        }

        public LogicalNode()
        {
            Position = new Position();
            Size = new Size();
            Inputs = new List<Input>();
            Outputs = new List<Output>();
        }

        public void ConnectInputs()
        {
            foreach (var input in Inputs)
                input.Subscribe(OnInputChange);
        }



        public abstract void Loop();
        public abstract void OnInputChange(Input input);

        public virtual void OnDeserialize()
        {
            ConnectInputs();
        }
    }





}
