/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.WebController.Code
{
    public class NodesEngineInfo
    {
        public bool Started { get; set; }
        public int LinksCount { get; set; }
        public int AllNodesCount { get; set; }
        public int PanelsNodesCount { get; set; }
        public int InputsOutputsNodesCount { get; set; }
        public int UiNodesCount { get; set; }
        public int HardwareNodesCount { get; set; }
        public int OtherNodesCount { get; set; }
    }
}