using MyNetSensors.LogicalNodes;
using Newtonsoft.Json;

namespace MyNetSensors.Repositories.Dapper
{
    public class LogicalNodeSerialized
    {
        public string Id { get; set; }
        public string JsonData { get; set; }

        public LogicalNodeSerialized()
        {
        }

        public LogicalNodeSerialized(LogicalNode node)
        {
            Id = node.Id;
            JsonData = SerializeNode(node);
        }

        public LogicalNode GetDeserializedNode()
        {
            return DeserializeNode(JsonData);
        }

        private string SerializeNode(LogicalNode node)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;

            return JsonConvert.SerializeObject(node, settings);
        }

        private LogicalNode DeserializeNode(string json)
        {

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.ObjectCreationHandling = ObjectCreationHandling.Replace;

            return JsonConvert.DeserializeObject<LogicalNode>(json, settings);
        }
    }
}


