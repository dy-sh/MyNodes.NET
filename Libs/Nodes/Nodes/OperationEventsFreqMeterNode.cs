//planer-pro copyright 2015 GPL - license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MyNetSensors.Nodes
{

    public class OperationEventsFreqMeterNode : Node
    {
        private int count;
        private int countWas;
        private Timer timer = new Timer();

        public OperationEventsFreqMeterNode() : base(1, 1)
        {
            this.Title = "Events Freq Meter";
            this.Type = "Operation/Events Freq Meter";

            Outputs[0].Name = "Events/sec";

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