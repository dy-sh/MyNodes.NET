/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

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
        LogicalNode GetNode(string Id);
        List<LogicalNode> GetAllNodes();
        void DeleteNode(string Id);
        void DropNodes();
    }
}
