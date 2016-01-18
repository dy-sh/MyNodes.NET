using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNetSensors.LogicalNodes;

namespace MyNetSensors.NodesTasks
{
    public class LogicalNodeTask:LogicalNode
    {
        public LogicalNodeTask():base(0,1)
        {
            
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
        }

        public void SetState(string state)
        {
            Outputs[0].Value = state;
        }
    }
}
