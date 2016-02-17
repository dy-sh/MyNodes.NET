using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNetSensors.WebController.Code
{
    public class NodeEditorConfig
    {
        public int Theme { get; set; }

        public List<string> GetThemesList()
        {
            return new List<string>
            {
                "Default",
                "Muted Tones",
                "Dark Nodes",
                "Pro",
                "R6",
                "Old Paper",
                "White",
                "Bright Nodes",
                "Green Field"
            };
        } 
    }
}
