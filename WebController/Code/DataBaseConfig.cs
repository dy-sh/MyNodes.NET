using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNetSensors.WebController.Code
{
    public class DataBaseConfig
    {
        public bool Enable { get; set; }
        public bool UseInternalDb { get; set; }
        public string ExternalDbConnectionString { get; set; }
        public int WriteInterval { get; set; }
        public bool LogState { get; set; }
    }
}
