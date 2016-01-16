/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.LogicalNodes
{
    public class NodeData:ICloneable
    {
        public int Id { get; set; }
        public string NodeId { get; set; }
        public string State { get; set; }
        public DateTime DateTime { get; set; }


        public NodeData()
        {
            DateTime = DateTime.Now;
        }

        public NodeData(string nodeId, string state)
        {
            NodeId = nodeId;
            State = state;
            DateTime = DateTime.Now;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}