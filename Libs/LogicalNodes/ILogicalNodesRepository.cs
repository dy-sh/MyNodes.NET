using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyNetSensors.LogicalNodes
{
    public interface ILogicalNodesRepository
    {
        void CreateDb();
        bool IsDbExist();
        int AddOrUpdateNode(LogicalNode node);
        int AddNode(LogicalNode node);
        void UpdateNode(LogicalNode node);
        LogicalNode GetNode(int Id);
        List<LogicalNode> GetAllNodes();
        void DeleteNode(int Id);
        void DropNodes();
    }
}
