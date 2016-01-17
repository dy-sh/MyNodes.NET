/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.LogicalNodes
{


    public interface ILogicalNodesStatesRepository
    {
        void AddState(NodeState state);
        List<NodeState> GetStatesForNode(string nodeId);
        void RemoveStatesForNode(string nodeId);
        void RemoveAllStates();
    }
}
