/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LiteGraph
{
    public class Graph
    {
        public IDictionary<int, Link> links { get; set; }
        //public string[] config { get; set; }
        public List<Node> nodes { get; set; }

    }


    public class Node
    {
        public string id { get; set; }
        public string panel_id { get; set; }
        
        public string title { get; set; }
        public string type { get; set; }
        public int[] pos { get; set; }
        public int[] size { get; set; }
        //        public string flags { get; set; }
        public List<Input> inputs { get; set; }
        public List<Output> outputs { get; set; }
        public IDictionary<string, string> properties { get; set; }


        public Node()
        {
            properties=new Dictionary<string, string>();
        }
    }

    public class Input
    {
        public string name { get; set; }
        public int type { get; set; }
        public string link { get; set; }
        public bool isOptional { get; set; }
    }

    public class Output
    {
        public string name { get; set; }
        public int type { get; set; }
        public string[] links { get; set; }
        public bool isOptional { get; set; }

    }

    public class Link
    {
        public string id { get; set; }
        public string origin_id { get; set; }
        public int origin_slot { get; set; }
        public string target_id { get; set; }
        public int target_slot { get; set; }
        public string panel_id { get; set; }
        //public string data { get; set; }

    }


}
