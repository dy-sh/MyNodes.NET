/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.ComponentModel.DataAnnotations;
using MyNetSensors.Gateway;

namespace MyNetSensors.NodesLinks
{
    public class SensorLink
    {
        [Key]
        public int db_Id { get; set; }

        public int fromSensorDbId { get; set; }
        public int fromNodeId { get; set; }
        public int fromSensorId { get; set; }
        public SensorDataType? fromDataType { get; set; }

        public int toSensorDbId { get; set; }
        public int toNodeId { get; set; }
        public int toSensorId { get; set; }
        public SensorDataType? toDataType { get; set; }

    }
}
