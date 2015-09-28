/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodesLinks
{
    public interface INodesLinksRepository
    {
        void CreateDb();
        bool IsDbExist();
        int AddOrUpdateLink(NodeLink link);
        int AddLink(NodeLink link);
        void UpdateLink(NodeLink link);
        NodeLink GetLink(int db_Id);
        List<NodeLink> GetIncomingLinks(int nodeId, int sensorId);
        List<NodeLink> GetOutgoingLinks(int nodeId, int sensorId);
        List<NodeLink> GetIncomingLinks(int sensorDbId);
        List<NodeLink> GetOutgoingLinks(int sensorDbId);
        List<NodeLink> GetAllLinks();
        void DeleteLink(int db_Id);
        void DeleteIncomingLinks(int nodeId, int sensorId);
        void DeleteOutgoingLinks(int nodeId, int sensorId);
        void DeleteIncomingLinks(int sensorDbId);
        void DeleteOutgoingLinks(int sensorDbId);
        void DropAllLinks();
    }
}
