using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.SerialControllers
{
    public class LogMessage
    {
        public string Message { get; set; }
        public DateTime Date { get; set; }

        public LogMessage(){}

        public LogMessage(string message)
        {
            Date = DateTime.Now;
            Message = message;
        }

        public override string ToString()
        {
            return $"{Date}: {Message}";
        }
    }
}
