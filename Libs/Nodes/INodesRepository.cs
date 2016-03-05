/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNodes.Nodes
{
    public interface INodesRepository
    {
        event LogEventHandler OnLogInfo;
        event LogEventHandler OnLogError;
        void SetWriteInterval(int ms);


        void AddNode(Node node);
        void AddNodes(List<Node> nodes);
        void UpdateNode(Node node);
        Node GetNode(string id);
        List<Node> GetAllNodes();
        void RemoveNode(string id);
        void RemoveNodes(List<Node> nodes);
        void RemoveAllNodes();

        void AddLink(Link link);
        void AddLinks(List<Link> links);
        Link GetLink(string id);
        List<Link> GetAllLinks();
        void RemoveLink(string id);
        void RemoveLinks(List<Link> links);
        void RemoveAllLinks();
    }
}
