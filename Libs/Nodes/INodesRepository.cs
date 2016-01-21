/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.Nodes
{
    public interface INodesRepository
    {
        void CreateDb();
        bool IsDbExist();
        string AddOrUpdateNode(Node node);
        string AddNode(Node node);
        void UpdateNode(Node node);
        Node GetNode(string id);
        List<Node> GetAllNodes();
        void RemoveNode(string id);
        void RemoveAllNodes();

        string AddOrUpdateLink(Link link);
        string AddLink(Link link);
        void UpdateLink(Link link);
        Link GetLink(string id);
        List<Link> GetAllLinks();
        void RemoveLink(string id);
        void RemoveAllLinks();
    }
}
