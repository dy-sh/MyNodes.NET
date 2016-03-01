using System;

namespace MyNodes.Nodes
{
    public class ChartData
    {
        public string x;
        public string y;

        public ChartData(DateTime time, string value)
        {
            x = $"{time:yyyy-MM-dd HH:mm:ss.fff}";
            y = value; //"0" ? "-0.01" : value;  to set zero visible
        }
    }
}