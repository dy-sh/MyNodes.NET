/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNodes.Nodes
{


    public interface INodesDataRepository
    {
        event LogEventHandler OnLogInfo;
        event LogEventHandler OnLogError;
        void SetWriteInterval(int ms);

        void AddNodeData(NodeData nodeData, int? maxDbRecords = null);
        int AddNodeDataImmediately(NodeData nodeData, int? maxDbRecords = null);
        void UpdateNodeData(NodeData nodeData);
        void UpdateNodeDataImmediately(NodeData nodeData);
        List<NodeData> GetAllNodeDataForNode(string nodeId);
        NodeData GetNodeData(int id);
        void RemoveAllNodeDataForNode(string nodeId);
        void RemoveNodeData(int id);
        void RemoveAllNodesData();
    }
}
