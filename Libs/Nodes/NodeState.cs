/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.Nodes
{
    public class NodeState
    {
        public int Id { get; set; }
        public string NodeId { get; set; }
        public string State { get; set; }
        public DateTime DateTime { get; set; }


        public NodeState()
        {
            DateTime = DateTime.Now;
        }

        public NodeState(string nodeId, string state)
        {
            NodeId = nodeId;
            State = state;
            DateTime = DateTime.Now;
        }
    }
}