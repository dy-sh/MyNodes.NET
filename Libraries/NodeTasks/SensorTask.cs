using System;
using System.ComponentModel.DataAnnotations;
using MyNetSensors.Gateway;

namespace MyNetSensors.NodeTasks
{
    public class SensorTask
    {
        [Key]
        public int db_Id { get; set; }
        public string description { get; set; }
        public int nodeId { get; set; }
        public int sensorId { get; set; }
        public int sensorDbId { get; set; }
        public DateTime executionDate { get; set; }
        public SensorData executionValue { get; set; }
        public bool isCompleted { get; set; }

        public bool isRepeating { get; set; }
        public int repeatingInterval { get; set; }
        public SensorData repeatingAValue { get; set; }
        public SensorData repeatingBValue { get; set; }
        //if repeatingCount==-1, then will run indefinitely
        public int repeatingCount { get; set; }
        public int executionsDoneCount { get; set; }
    }
}
