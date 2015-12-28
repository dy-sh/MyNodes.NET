using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.LogicalNodes
{
    public class LogicalLink
    {
        public string Id { get; set; }

        public string InputId { get; set; }
        public string OutputId { get; set; }

        
        //public LogicalLink( Output output, Input input)
        //{
        //    InputId = input.Id;
        //    OutputId = output.Id;
        //}

        public LogicalLink(string outputId, string inputId)
        {
            Id = Guid.NewGuid().ToString();
            InputId = inputId;
            OutputId = outputId;
        }

        public LogicalLink(){}
    }
}
