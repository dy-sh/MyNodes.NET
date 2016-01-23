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


