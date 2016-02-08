/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
  public class UiSwitchNode : UiNode
    {
      public string Value { get; set; }

      public UiSwitchNode() : base(0, 1)
      {
            this.Title = "UI Switch";
            this.Type = "UI/Switch";
            this.DefaultName = "Switch";
           Value = "0";
            Outputs[0].Value = Value;
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
        }

        public void Toggle()
        {
            Value = Value == "0" ? "1" : "0";
            LogInfo($"UI Switch [{Settings["Name"].Value}]: [{Value}]");
            Outputs[0].Value = Value;

            UpdateMe();
            UpdateMeInDb();
        }
    }
}
