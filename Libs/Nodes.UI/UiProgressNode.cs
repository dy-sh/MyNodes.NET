/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
  public class UiProgressNode : UiNode
    {
      public string Value { get; set; }

      public UiProgressNode() : base(1, 0)
      {
            this.Title = "UI Progress";
            this.Type = "UI/Progress";
            this.DefaultName = "Progress";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            Value = input.Value;
            UpdateMe();
        }


    }
}
