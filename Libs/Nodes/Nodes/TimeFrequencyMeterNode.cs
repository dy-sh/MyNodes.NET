//planer-pro copyright 2015 GPL - license.

using System.Timers;

namespace MyNetSensors.Nodes
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
            countWas = 0;
        }
    }
}