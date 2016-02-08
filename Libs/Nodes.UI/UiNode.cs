/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public abstract class UiNode : Node
    {
        internal string DefaultName { get; set; }

        public UiNode(int inputsCount, int outputsCount) : base(inputsCount, outputsCount)
        {
            Settings.Add("Name", new NodeSetting(NodeSettingType.Text, "Name",""));
            Settings.Add("PanelIndex",new NodeSetting(NodeSettingType.Number, "Index on panel","0"));
            Settings.Add("ShowOnMainPage", new NodeSetting(NodeSettingType.Checkbox, "Show on Dashboard Main Page","true"));
        }

        public void SetDefaultName(string name)
        {
            DefaultName = name;
        }
    }
}
