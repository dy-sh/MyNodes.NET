//planer-pro copyright 2015 GPL - license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MyNetSensors.Nodes
{

    public class OperationEventsFreqCounterNode : Node
    {
        private int count;
        private int countWas;
        private Timer timer = new Timer();

        public OperationEventsFreqCounterNode() : base(1, 1)
        {
            this.Title = "Events Freq Counter";
            this.Type = "Operation/Events Freq Counter";

            Inputs[0].Name = "Value";

            Inputs[0].Type = DataType.Text;
            Outputs[0].Type = DataType.Number;

            Outputs[0].Value = "0";

            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += TimerElapsed;
            timer.Start();
        }

        public override void Loop()
        {
        }


        public override void OnInputChange(Input input)
        {
            count++;
        }


        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (count == countWas)
                return;

            countWas = count;
            count = 0;
            LogInfo($"[{countWas}]");
            Outputs[0].Value = countWas.ToString();
        }
    }
}