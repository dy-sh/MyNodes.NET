using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.LogicalNodes
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
                LogicalNodesEngine.logicalNodesEngine.OnOutputChange(this);
            }
        }

        public Output()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
