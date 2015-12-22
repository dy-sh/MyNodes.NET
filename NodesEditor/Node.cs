using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNetSensors.NodesEditor
{
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
}
