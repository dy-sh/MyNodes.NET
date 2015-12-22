using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyNetSensors.NodesEditor
{
    public class Graph
    {
        public int iteration { get; set; }
        public int last_node_id { get; set; }
        public int last_link_id { get; set; }
        public IDictionary<int, Link> links { get; set; }
        //public string[] config { get; set; }
        public List<Node> nodes { get; set; }



    }


}
