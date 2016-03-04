/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Timers;

namespace MyNodes.Nodes
{
    public class TimeFrequencyMeterNode : Node
    {
        private int count;
        private int countWas;
        private readonly Timer timer = new Timer();

        public TimeFrequencyMeterNode() : base("Time", "Frequency Meter")
        {
            AddInput();
            AddOutput("Events/sec", DataType.Number);

            Outputs[0].Value = "0";

            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += TimerElapsed;
            timer.Start();
        }


        public override void OnInputChange(Input input)
        {
            count++;
        }


        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            //dont start if engine is not started
            if (engine == null || !engine.IsStarted())
                return;

            if (count == countWas)
                return;

            countWas = count;
            count = 0;
            Outputs[0].Value = countWas.ToString();
        }

        public override string GetNodeDescription()
        {
            return "This node measures the rate at which events arrive at the input. <br/>" +
                   "Any value including null will be taken.";
        }
    }
}