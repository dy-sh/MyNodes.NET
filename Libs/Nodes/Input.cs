using System;

namespace MyNetSensors.Nodes
{
    public class Input
    {
        public string Id { get; set; }
        public string Name { get; set; }

        private string val;

        public string Value
        {
            get { return val; }
            set
            {
                val = value;
                NodesEngine.nodesEngine.OnInputChange(this);
            }
        }

        public Input()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
