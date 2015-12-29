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
        string AddOrUpdateNode(LogicalNode node);
        string AddNode(LogicalNode node);
        void UpdateNode(LogicalNode node);
        LogicalNode GetNode(string id);
        List<LogicalNode> GetAllNodes();
        void DeleteNode(string id);
        void DropNodes();

        string AddOrUpdateLink(LogicalLink link);
        string AddLink(LogicalLink link);
        void UpdateLink(LogicalLink link);
        LogicalLink GetLink(string id);
        List<LogicalLink> GetAllLinks();
        void DeleteLink(string id);
        void DropLinks();
    }
}
