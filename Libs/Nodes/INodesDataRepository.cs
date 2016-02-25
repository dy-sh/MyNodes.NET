/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.Nodes
{


    public interface INodesDataRepository
    {
        event LogEventHandler OnLogInfo;
        event LogEventHandler OnLogError;
        void SetWriteInterval(int ms);

        void AddNodeData(NodeData data, int? maxDbRecords = null);
        List<NodeData> GetAllNodeDataForNode(string nodeId);
        NodeData GetNodeData(int id);
        void RemoveAllNodeDataForNode(string nodeId);
        void RemoveNodeData(int id);
        void RemoveAllNodesData();
    }
}
