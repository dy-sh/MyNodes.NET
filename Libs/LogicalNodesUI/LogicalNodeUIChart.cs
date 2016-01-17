/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using MyNetSensors.LogicalNodes;

namespace MyNetSensors.LogicalNodesUI
{
    public class LogicalNodeUIChart : LogicalNodeUI
    {

        public int? State { get; set; }

        public bool WriteInDatabase { get; set; }
        public int WriteInDatabaseMinInterval { get; set; }
        public DateTime WriteInDatabaseLastDate { get; set; }


        private List<NodeState> NodeStates { get; set; }
        private string LastStateCached { get; set; }
        private bool LastStateUpdated { get; set; }


        public LogicalNodeUIChart() : base(1, 0)
        {
            this.Title = "UI Chart";
            this.Type = "UI/Chart";
            this.Name = "Chart";

            NodeStates = new List<NodeState>();
            WriteInDatabase = false;
            WriteInDatabaseMinInterval = 1000;
            WriteInDatabaseLastDate = DateTime.Now;
        }

        public override void Loop()
        {
            if (!LastStateUpdated)
                return;

            if ((DateTime.Now - WriteInDatabaseLastDate).TotalMilliseconds < WriteInDatabaseMinInterval)
                return;

            LastStateUpdated = false;
            WriteInDatabaseLastDate = DateTime.Now;

            if (LastStateCached == null)
            {
                State = null;

                //call update event without writing to db node state
                CallNodeUpdatedEvent(false);
            }

            try
            {
                int val = Int32.Parse(LastStateCached);
                NodeStates.Add(new NodeState(Id, val.ToString()));
                State = val;
                CallNodeUpdatedEvent(false);
            }
            catch (Exception)
            {
                State = null;
                LogError($"Incorrect input data in UI Chart [{Name}]");
            }
        }

        public override void OnInputChange(Input input)
        {
            LastStateCached = input.Value;
            LastStateUpdated = true;
        }

        public List<NodeState> GetStates()
        {
            return NodeStates;
        }

        public void SetStates(List<NodeState> states)
        {
            NodeStates = states ?? new List<NodeState>();
            LastStateUpdated = false;
        }

        public void RemoveStates()
        {
            NodeStates.Clear();
            State = null;
            LastStateUpdated = false;
        }
    }
}
