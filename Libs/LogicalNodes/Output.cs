using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.LogicalNodes
{
    public delegate void OnOutputChangeEventArgs(Output output);



    public class Output
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public event OnOutputChangeEventArgs OnOutputChange;

        private string val;

        public string Value
        {
            get { return val; }
            set
            {
                val = value;
                if (OnOutputChange != null)
                    OnOutputChange(this);
            }
        }
    }
}
