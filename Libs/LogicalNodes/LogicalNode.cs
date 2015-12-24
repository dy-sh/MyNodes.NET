using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.LogicalNodes
{
    public class LogicalNode
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public Position Position { get; set; }
        public Size Size { get; set; }
        //        public string flags { get; set; }
        public List<Input> Inputs { get; set; }
        public List<Output> Outputs { get; set; }

        public LogicalNode()
        {
            Position=new Position();
            Size=new Size();
        }

        public virtual void Loop()
        {
            Console.WriteLine($"{DateTime.Now} {Id}");
        }
    }

    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class Size
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Input
    {

    }

    public class Output
    {

    }


}
