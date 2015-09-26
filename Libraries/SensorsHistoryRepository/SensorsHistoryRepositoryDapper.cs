using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using MyNetSensors.Gateway;

namespace MyNetSensors.SensorsHistoryRepository
{
    /// <summary>
    /// Repository can read sensors history. If gateway connected, it will store updated sensors to history.
    /// </summary>
    public class SensorsHistoryRepositoryDapper:ISensorsHistoryRepository
    {
        private SerialGateway gateway;

        private IDbConnection db;

        public SensorsHistoryRepositoryDapper(string connectionString)
        {
            db = new SqlConnection(connectionString);
        }

        public bool IsDbExist()
        {
            //todo check if db exist
            return true;
        }

        public void ConnectToGateway(SerialGateway gateway)
        {
            this.gateway = gateway;

            gateway.OnSensorUpdatedEvent += StoreSensorToHistory; ;
        }

 
        public List<SensorData> GetSensorHistory(int db_Id)
        {
            string req = String.Format("SELECT * FROM Sensor{0}", db_Id);

            List<SensorData> list=null;
            try
            {
                list = db.Query<SensorData>(req).ToList();
            }
            catch { }

            return list;
        }

    

        public void DropSensorHistory(int db_Id)
        {
            db.Query(String.Format("DROP TABLE [Sensor{0}]", db_Id));
        }


        private void StoreSensorToHistory(Sensor sensor)
        {
            //todo check sensor settings

            CreateTableForSensor(sensor);

            List<SensorData> data = sensor.GetAllData();

            if (data == null)
                return;

            foreach (var sensorData in data)
                sensorData.dateTime = DateTime.Now;

            var sqlQuery = String.Format(
                "INSERT INTO Sensor{0} (dataType, state, dateTime) "
                + "VALUES(@dataType,@state, @dateTime); "
                + "SELECT CAST(SCOPE_IDENTITY() as int)", sensor.db_Id);
            db.Execute(sqlQuery, data);

        }

        private void CreateTableForSensor(Sensor sensor)
        {
            try
            {
                string req = String.Format(
                    @"CREATE TABLE [dbo].[Sensor{0}](
	            [db_Id] [int] IDENTITY(1,1) NOT NULL,
	            [dataType] [int] NULL,	        
	            [state] [nvarchar](max) NULL,	        
	            [dateTime] [datetime] NOT NULL ) ON [PRIMARY] ", sensor.db_Id);

                db.Execute(req);
            }
            catch { }
        }
    }
}