/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNodes.Nodes
{
    public abstract class UiNode : Node
    {
        internal string DefaultName { get; set; }

        protected UiNodesEngine uiEngine;


        public UiNode(string category, string type) : base(category, type)
        {
            DefaultName = type;
            Settings.Add("Name", new NodeSetting(NodeSettingType.Text, "Name", ""));
            Settings.Add("PanelIndex", new NodeSetting(NodeSettingType.Number, "Index on panel", "0"));
            Settings.Add("ShowOnHomePage", new NodeSetting(NodeSettingType.Checkbox, "Show on Dashboard Home Page", "true"));
        }

        public virtual bool SetValues(Dictionary<string, string> values)
        {
            return false;
        }

        public virtual string GetValue(string name)
        {
            return null;
        }

        public virtual void OnAddToUiEngine(UiNodesEngine uiEngine)
        {
            this.uiEngine = uiEngine;
        }

        public override bool SetSettings(Dictionary<string, string> data)
        {
            if (data["ShowOnHomePage"]=="false" && Settings["ShowOnHomePage"].Value=="true")
                uiEngine.HideFromHomePage(this);

            bool result = base.SetSettings(data);
            UpdateMeOnDashboard();
            return result;
        }
    }
}
