using System;

namespace MyNetSensors.Nodes
{
    public class Link
    {
        public string Id { get; set; }

        public string InputId { get; set; }
        public string OutputId { get; set; }
        public string PanelId { get; set; }


        //public Link( Output output, Input input)
        //{
        //    InputId = input.Id;
        //    OutputId = output.Id;
        //}

        public Link(string outputId, string inputId)
        {
            Id = Guid.NewGuid().ToString();
            InputId = inputId;
            OutputId = outputId;
        }

        public Link(){}
    }
}
