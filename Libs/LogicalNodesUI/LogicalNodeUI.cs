/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using MyNetSensors.LogicalNodes;

namespace MyNetSensors.LogicalNodesUI
{
    public abstract class LogicalNodeUI : LogicalNode
    {
        public int PanelOrderIndex { get; set; }
        public string Name { get; set; }

        public bool ShowOnMainPage { get; set; }

        public LogicalNodeUI(int inputsCount, int outputsCount) : base(inputsCount, outputsCount)
        {
            ShowOnMainPage = true;
        }

    }
}
