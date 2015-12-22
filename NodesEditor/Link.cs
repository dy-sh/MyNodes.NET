using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNetSensors.NodesEditor
{
    public class Link
    {
        public int id { get; set; }
        public int origin_id { get; set; }
        public int origin_slot { get; set; }
        public int target_id { get; set; }
        public int target_slot { get; set; }
        //public string data { get; set; }

    }
}
