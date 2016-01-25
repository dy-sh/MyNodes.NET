using System;

namespace MyNetSensors.Nodes
{
    public delegate void OutputEventHandler(Output output);
    public class Output
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int SlotIndex { get; set; }

        private string val;

        public event OutputEventHandler OnOutputChange;

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
        }
    }
}
