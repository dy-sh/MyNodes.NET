//planer-pro copyright 2015 GPL - license.

using System.Timers;

namespace MyNetSensors.Nodes
{
    public class OperationEventsFreqMeterNode : Node
    {
        private int count;
        private int countWas;
        private readonly Timer timer = new Timer();

        public OperationEventsFreqMeterNode() : base("Operation", "Events Freq Meter", 1, 1)
        {
            Outputs[0].Name = "Events/sec";

            Inputs[0].Type = DataType.Text;
            Outputs[0].Type = DataType.Number;

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