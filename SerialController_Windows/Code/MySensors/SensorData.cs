using System;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace SerialController_Windows.Code
{
    public class SensorData
    {
        //DB Propertys
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(Sensor))]
        public int SensorId { get; set; }



        public SensorDataType? dataType { get; set; }
        public string state { get; set; }


        public SensorData()
        {

        }



        public SensorData(SensorDataType? dataType, string state)
        {
            this.dataType = dataType;
            this.state = state;
        }

        public override string ToString()
        {
            string s="";

            if (dataType != null)
                s += String.Format("Data type: {0}, ", dataType.ToString());
            else
                s += String.Format("Data type: unknown, ");

            if (state != null)
                s += String.Format("State: {0}\r\n", state);
            else
                s += String.Format("State: unknown\r\n");

            return s;
        }
    }
}