/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNodes.Nodes
{
    public class NodeData
    {
        public int Id { get; set; }
        public string NodeId { get; set; }
        public DateTime DateTime { get; set; }
        public string Value { get; set; }


        public NodeData()
        {
            DateTime = DateTime.Now;
        }

        public NodeData(string nodeId, string value)
        {
            NodeId = nodeId;
            Value = value;
            DateTime = DateTime.Now;
        }
    }
}