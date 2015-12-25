using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNetSensors.Gateways;

namespace MyNetSensors.LogicalNodes
{
    public class LogicalNodesLink
    {
        public int Id { get; set; }
        public Input Input { get; set; }
        public Output Output { get; set; }

        
        public LogicalNodesLink(Input input, Output output)
        {
            this.Input = input;
            this.Output = output;
        }
    }
}
