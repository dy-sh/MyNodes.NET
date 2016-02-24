/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;

namespace MyNetSensors.Nodes
{
    public class UiLogNode : UiNode
    {
        public string Log { get; set; }

        public UiLogNode() : base("UI", "Log")
        {
            AddInput();
        }


        public override void OnInputChange(Input input)
        {
            Log += $"{DateTime.Now}: {input.Value ?? "NULL"}<br/>";
            UpdateMe();
        }


        public override bool SetValues(Dictionary<string, string> values)
        {
            //clear log

            Log = "";
            UpdateMe();

            return true;
        }

        public override string GetNodeDescription()
        {
            return "This is a UI node. It displays a log on the dashboard. " +
                   "The log displays all the values that it receives. " +
                   "It is very convenient to use for debugging your system or for the monitoring.";
        }
    }
}
