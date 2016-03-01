/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyNodes.Nodes
{
    public static class NodesEngineSerializer
    {

        private static string SerializeNodesAndLinks(List<Node> nodesList, List<Link> linksList)
        {
            List<Object> list = new List<Object>();
            list.AddRange(nodesList);
            list.AddRange(linksList);

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;

            return JsonConvert.SerializeObject(list, settings);
        }

        private static void DeserializeNodesAndLinks(string json, out List<Node> nodesList, out List<Link> linksList)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.ObjectCreationHandling = ObjectCreationHandling.Replace;

            List<object> objects = (List<object>)JsonConvert.DeserializeObject<object>(json, settings);

            nodesList = objects.OfType<Node>().ToList();
            linksList = objects.OfType<Link>().ToList();
        }


        public static string SerializePanel(string panelId, NodesEngine engine)
        {
            Node panel = engine.GetNode(panelId) as PanelNode;
            if (panel == null)
            {
                engine.LogEngineError($"Can`t serialize Panel [{panelId}]. Does not exist.");
                return null;
            }

            List<Node> nodesList = new List<Node>();
            nodesList.Add(panel);
            nodesList.AddRange(engine.GetNodesForPanel(panelId, true));

            List<Link> linksList = engine.GetLinksForPanel(panelId, true);

            return SerializeNodesAndLinks(nodesList, linksList);
        }

        public static void DeserializePanel(string json, out List<Node> nodesList, out List<Link> linksList)
        {
            DeserializeNodesAndLinks(json, out nodesList, out linksList);
        }

        public static string SerializeNode(Node node)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;

            return JsonConvert.SerializeObject(node, settings);
        }

        public static Node DeserializeNode(string json)
        {

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.ObjectCreationHandling = ObjectCreationHandling.Replace;

            return JsonConvert.DeserializeObject<Node>(json, settings);
        }
    }
}
