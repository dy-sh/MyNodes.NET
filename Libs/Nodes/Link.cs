/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNodes.Nodes
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

        public Link(string outputId, string inputId, string panelId)
        {
            Id = Guid.NewGuid().ToString();
            InputId = inputId;
            OutputId = outputId;
            PanelId = panelId;
        }

        public Link(){}
    }
}
