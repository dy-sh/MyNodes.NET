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
            JsonData = SerializeNode(node);
        }

        public Node GetDeserializedNode()
        {
            return DeserializeNode(JsonData);
        }

        private string SerializeNode(Node node)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;

            return JsonConvert.SerializeObject(node, settings);
        }

        private Node DeserializeNode(string json)
        {

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.ObjectCreationHandling = ObjectCreationHandling.Replace;

            return JsonConvert.DeserializeObject<Node>(json, settings);
        }
    }
}


