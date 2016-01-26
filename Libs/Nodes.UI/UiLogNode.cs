/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.Nodes
{
    public class UiLogNode : UiNode
    {
        public string Log { get; set; }

        public UiLogNode() : base(1, 0)
        {
            this.Title = "UI Log";
            this.Type = "UI/Log";
            this.DefaultName = "Log";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            Log += $"{DateTime.Now}: {input.Value ?? "NULL"}<br/>";
            UpdateMe();
        }


        public void ClearLog()
        {
            Log = "";
            UpdateMe();
        }
    }
}
