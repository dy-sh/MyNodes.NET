using Newtonsoft.Json;

namespace MyNetSensors.Nodes
{
    public class SerializedLink
    {
        public string Id { get; set; }
        public string JsonData { get; set; }

        public SerializedLink()
        {
        }

        public SerializedLink(Link link)
        {
            Id = link.Id;
            JsonData = SerializeLink(link);
        }

        public Link GetDeserializedLink()
        {
            return DeserializeLink(JsonData);
        }

        private string SerializeLink(Link link)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;

            return JsonConvert.SerializeObject(link, settings);
        }

        private Link DeserializeLink(string json)
        {

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.ObjectCreationHandling = ObjectCreationHandling.Replace;

            return JsonConvert.DeserializeObject<Link>(json, settings);
        }
    }
}


