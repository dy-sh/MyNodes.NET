/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using Newtonsoft.Json;

namespace MyNetSensors.Nodes
{
    public class SerializedNode
    {
        public string Id { get; set; }
        public string JsonData { get; set; }

        public SerializedNode()
        {
        }

        public SerializedNode(Node node)
        {
            Id = node.Id;
            JsonData = NodesEngineSerializer.SerializeNode(node);
        }

        public Node GetDeserializedNode()
        {
            return NodesEngineSerializer.DeserializeNode(JsonData);
        }


    }
}


