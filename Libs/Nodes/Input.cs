/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNodes.Nodes
{
    public delegate void InputEventHandler(Input input);

    public class Input
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DataType Type { get; set; }
        public int SlotIndex { get; set; }
        public bool IsOptional { get; set; }

        public event InputEventHandler OnInputChange;

        private string val;

        public string Value
        {
            get { return val; }
            set
            {
                val = value;
                OnInputChange?.Invoke(this);
            }
        }

        public Input()
        {
            Id = Guid.NewGuid().ToString();
            Type = DataType.Text;
        }

        public Input(string name, DataType type= DataType.Text, bool isOptional=false)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Type = type;
            IsOptional = isOptional;
        }

        public void SetValueWithoutUpdate(string value)
        {
            val = value;
        }
    }
}
