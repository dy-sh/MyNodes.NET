using System;

namespace MyNetSensors.Nodes
{
    public class Output
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
                NodesEngine.nodesEngine.OnOutputChange(this);
            }
        }

        public Output()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
