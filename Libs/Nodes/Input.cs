using System;

namespace MyNetSensors.Nodes
{
    public delegate void InputEventHandler(Input input);

    public class Input
    {
        public string Id { get; set; }
        public string Name { get; set; }

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
        }
    }
}
