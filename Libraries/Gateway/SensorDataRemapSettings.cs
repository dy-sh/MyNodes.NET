using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyNetSensors.Gateway
{
    public class SensorDataRemapSettings
    {
        public bool invertValue { get; set; }
        public bool remapEnabled { get; set; }
        public string remapFromMin { get; set; }
        public string remapFromMax { get; set; }
        public string remapToMin { get; set; }
        public string remapToMax { get; set; }

        public void ParseFromJson(string json)
        {
            SensorDataRemapSettings result= JsonConvert.DeserializeObject<SensorDataRemapSettings>(json);
            invertValue = result.invertValue;
            remapEnabled = result.remapEnabled;
            remapFromMin = result.remapFromMin;
            remapFromMax = result.remapFromMax;
            remapToMin = result.remapToMin;
            remapToMax = result.remapToMax;
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }


}
