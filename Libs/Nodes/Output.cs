/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.Nodes
{
    public delegate void OutputEventHandler(Output output);
    public class Output
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DataType Type { get; set; }
        public int SlotIndex { get; set; }

        public event OutputEventHandler OnOutputChange;

        private string val;
        public string Value
        {
            get { return val; }
            set
            {
                val = value;
                OnOutputChange?.Invoke(this);
            }
        }

        public Output()
        {
            Id = Guid.NewGuid().ToString();
            Type = DataType.Text;
        }

        public Output(string name, DataType type = DataType.Text)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Type = type;
        }
    }
}
