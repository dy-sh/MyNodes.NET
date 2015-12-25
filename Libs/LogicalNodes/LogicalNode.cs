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
            
            Inputs = new List<Input>();
            for (int i = 0; i < inputsCount; i++)
            {
                Inputs.Add(new Input(this,$"In {i}"));
            }

            Outputs = new List<Output>();
            for (int i = 0; i < outputsCount; i++)
            {
                Outputs.Add(new Output(this, $"Out {i}"));
            }
        }

        public abstract void Loop();
        public abstract void OnInputChange(Input input);
    }

    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class Size
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Input
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public event OnInputChangeEventArgs OnInputChange;

        private string val;
        private LogicalNode node;

        public Input(LogicalNode node,string name)
        {
            this.node = node;
            this.Name = name;
        }

        public string Value
        {
            get { return val; }
            set
            {
                val = value;
                if (OnInputChange != null)
                    OnInputChange(value);
                node.OnInputChange(this);
            }
        }

        public void SetValue(string value)
        {
            Value = value;
        }
    }

    public class Output
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public event OnOutputChangeEventArgs OnOutputChange;

        private string val;
        private LogicalNode node;

        public Output(LogicalNode node,string name)
        {
            this.node = node;
            this.Name = name;
        }

        public string Value
        {
            get { return val; }
            set
            {
                val = value;
                if (OnOutputChange != null)
                    OnOutputChange(value);
            }
        }
    }

    public delegate void OnInputChangeEventArgs(string value);
    public delegate void OnOutputChangeEventArgs(string value);
}
