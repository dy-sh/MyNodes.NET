/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using MyNetSensors.LogicalNodes;

namespace MyNetSensors.LogicalNodesUI
{
  public class UISlider : LogicalNodeUI
    {
      public string Value { get; set; }

      public UISlider() : base(0, 1)
      {
            this.Title = "UI Slider";
            this.Type = "UI/Slider";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            Value = input.Value;
        }


    }
}
