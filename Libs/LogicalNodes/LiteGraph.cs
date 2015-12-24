using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LiteGraph
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


    public class Node
    {
        public int id { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public int[] pos { get; set; }
        public int[] size { get; set; }
        //        public string flags { get; set; }
        public List<Input> inputs { get; set; }
        public List<Output> outputs { get; set; }
        //  public int[] properties { get; set; }

    }

    public class Input
    {
        public string name { get; set; }
        public string type { get; set; }
        public int? link { get; set; }
    }

    public class Output
    {
        public string name { get; set; }
        public string type { get; set; }
        public int[] links { get; set; }

    }

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
