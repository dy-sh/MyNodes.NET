/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.LogicalNodes
{


    public interface ILogicalNodesDataRepository
    {
        void AddNodeData(NodeData data);
        List<NodeData> GetNodeData(string nodeId);
        void ClearNodeData(string nodeId);
        void ClearAllNodesData();
    }
}
