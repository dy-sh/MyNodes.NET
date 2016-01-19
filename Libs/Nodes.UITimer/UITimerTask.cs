/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.Nodes
{
    public class UITimerTask
    {

        public int Id { get; set; }
        public string NodeId { get; set; }
        public bool Enabled { get; set; }
        public bool IsCompleted { get; set; }
        public string Description { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string ExecutionValue { get; set; }
        public bool IsRepeating { get; set; }
        public int RepeatingInterval { get; set; }
        public string RepeatingAValue { get; set; }
        public string RepeatingBValue { get; set; }
        //if repeatingNeededCount==0, then will run indefinitely
        public int RepeatingNeededCount { get; set; }
        public int RepeatingDoneCount { get; set; }

    }
}
