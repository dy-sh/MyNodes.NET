/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Timers;
using Dapper;
using MyNetSensors.Gateways;

namespace MyNetSensors.Repositories.Dapper
{
    public class NodesHistoryRepositoryDapper : INodesHistoryRepository
    {
        //This value is interval for updateDbTimer in ms. 
        //When timer will elapsed, program will check all nodes,
        //which need to store in db (sensor.storeHistoryWithInterval), and will write them.
        //If storeHistoryWithInterval will be less then writeInterval, 
        //storeHistoryWithInterval will be equal to writeInterval
        //If you have tons of data, and db perfomance decreased, increase this value,
        //and you will get less writing to db frequency 
        public int writeInterval = 1000;


        private Timer updateDbTimer = new Timer();

        private Gateway gateway;

        private string connectionString;

        public NodesHistoryRepositoryDapper(string connectionString)
        {
            this.connectionString = connectionString;
            // db = new SqlConnection(connectionString);
        }


        public bool IsDbExist()
        {
            //todo check if db exist
            return true;
        }

        public void ConnectToGateway(Gateway gateway)
        {
            this.gateway = gateway;

            gateway.OnSensorUpdatedEvent += OnSensorUpdatedEvent;

            updateDbTimer.Elapsed += UpdateDbTimer;

            updateDbTimer.Interval = writeInterval;
            updateDbTimer.Start();

        }

        public void SetWriteInterval(int ms)
        {
            writeInterval = ms;
            updateDbTimer.Stop();
            updateDbTimer.Interval = writeInterval;
            updateDbTimer.Start();
        }

        private void OnSensorUpdatedEvent(Sensor sensor)
        {
            if (sensor.storeHistoryEnabled && sensor.storeHistoryEveryChange)
                WriteSensorDataToHistory(sensor);
        }

        private void UpdateDbTimer(object sender, ElapsedEventArgs e)
        {
            updateDbTimer.Stop();
            try
            {

                List<Node> nodes = gateway.GetNodes();
                foreach (var node in nodes)
                {
                    foreach (var sensor in node.sensors)
                    {
                        if (!sensor.storeHistoryEnabled || sensor.storeHistoryWithInterval == 0)
                            continue;

                        TimeSpan elapsedTime = DateTime.Now.Subtract(sensor.storeHistoryLastDate);
                        if (elapsedTime.TotalSeconds >= sensor.storeHistoryWithInterval)
                        {
                            sensor.storeHistoryLastDate = DateTime.Now;
                            WriteSensorDataToHistory(sensor);
                        }
                    }
                }
            }
            catch { }
            updateDbTimer.Start();
        }


        public List<SensorData> GetSensorHistory(int nodeId, int sensorId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                string req = $"SELECT * FROM [SensorHistory-{nodeId}-{sensorId}]";

                List<SensorData> list = null;
                try
                {
                    list = db.Query<SensorData>(req).ToList();
                }
                catch
                {
                }

                return list;
            }
        }



        public void ClearSensorHistory(int nodeId, int sensorId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                try
                {
                    db.Query($"DROP TABLE [SensorHistory-{nodeId}-{sensorId}]");
                }
                catch { }
            }
        }





        public void ClearAllHistory()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                try
                {
                    db.Query(
                        @"  declare @sql varchar(8000) 
                            set @sql='' 
                            select @sql=@sql+' drop table '+table_name from INFORMATION_SCHEMA.TABLES where table_name like 'SensorHistory-%[0-9.]-%[0-9.]' 
                            exec(@sql)");
                }
                catch { }
            }
        }


        private void WriteSensorDataToHistory(Sensor sensor)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                CreateTableForSensor(sensor);

                if (sensor.state == null)
                    return;


                var sqlQuery = $"INSERT INTO [SensorHistory-{sensor.nodeId}-{sensor.sensorId}] (dataType, state, dateTime) "
                    + "VALUES(@dataType,@state, @dateTime); "
                    + "SELECT CAST(SCOPE_IDENTITY() as int)";
                db.Execute(sqlQuery,new
                {
                    dataType= sensor.dataType,
                    state = sensor.state,
                    dateTime =DateTime.Now
                });

            }
        }

        private void CreateTableForSensor(Sensor sensor)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                try
                {
                    string req = $@"CREATE TABLE [dbo].[SensorHistory-{sensor.nodeId}-{sensor.sensorId}](
	            [Id] [int] IDENTITY(1,1) NOT NULL,
	            [dataType] [int] NULL,	        
	            [state] [nvarchar](max) NULL,	        
	            [dateTime] [datetime] NOT NULL ) ON [PRIMARY] ";

                    db.Query(req);
                }
                catch
                {
                }
            }
        }
    }
}