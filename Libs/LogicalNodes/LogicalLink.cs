using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNetSensors.Gateways;

namespace MyNetSensors.LogicalNodes
{
    public class LogicalLink
    {
        public int Id { get; set; }
        public Input Input { get; set; }
        public Output Output { get; set; }

        
        public LogicalLink( Output output, Input input)
        {
            this.Input = input;
            this.Output = output;
        }

        public LogicalLink(){}
    }
}
