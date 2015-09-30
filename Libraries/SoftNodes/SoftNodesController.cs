using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.SoftNodes
{
    public class SoftNodesController
    {
        private ISoftNodesServer server;
        public SoftNodesController(ISoftNodesServer server)
        {
            this.server = server;
        }

        
    }
}
