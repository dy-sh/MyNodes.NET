using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.LogicalNodes
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
                LogicalNodesEngine.logicalNodesEngine.OnInputChange(this);
            }
        }

        public Input()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
