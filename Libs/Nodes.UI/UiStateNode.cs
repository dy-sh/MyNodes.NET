/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
  public class UiStateNode : UiNode
    {
      public string Value { get; set; }

      public UiStateNode() : base(1, 0)
      {
            this.Title = "UI State";
            this.Type = "UI/State";
            this.Name = "State";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            Value = input.Value;
            CallNodeUpdatedEvent(false);
        }


    }
}
