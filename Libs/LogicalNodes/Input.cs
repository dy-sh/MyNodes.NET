using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.LogicalNodes
{
    public delegate void OnInputChangeEventArgs(Input input);



    public class Input
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public event OnInputChangeEventArgs OnInputChange;

        private string val;

        public string Value
        {
            get { return val; }
            set
            {
                val = value;
                if (OnInputChange != null)
                    OnInputChange(this);
            }
        }

        public void Subscribe(OnInputChangeEventArgs subscriber)
        {
            if (OnInputChange != subscriber)
                OnInputChange += subscriber;
        }
    }
}
