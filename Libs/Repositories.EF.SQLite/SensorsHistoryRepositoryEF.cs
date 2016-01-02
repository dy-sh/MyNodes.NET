/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Timers;
using Microsoft.Data.Entity;
using MyNetSensors.Gateways;

namespace MyNetSensors.Repositories.EF.SQLite
{
    /// <summary>
    /// Repository can read sensors history. If gateway connected, it will store updated sensors history.
    /// </summary>
    public class SensorsHistoryRepositoryEF : ISensorsHistoryRepository
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
        private NodesDbContext db;

        public SensorsHistoryRepositoryEF(NodesDbContext nodesDbContext)
        {
            this.db = nodesDbContext;
            CreateDb();
        }

        public void CreateDb()
        {
            db.Database.EnsureCreated();
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

                            SensorData data = new SensorData(sensor);
                            db.SensorsData.Add(data);
                        }
                    }
                }
                db.SaveChanges();
            }
            catch { }
            updateDbTimer.Start();
        }


        public List<SensorData> GetSensorHistory(int nodeId,int sensorId)
        {
            return db.SensorsData.Where(x => x.nodeId == nodeId && x.sensorId == sensorId).ToList();
        }



        public void DropSensorHistory(int nodeId, int sensorId)
        {
            List<SensorData> data = GetSensorHistory(nodeId, sensorId);
            db.SensorsData.RemoveRange(data);
            db.SaveChanges();
        }

        public void DropHistory()
        {
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [SensorHistory]");
        }


        private void WriteSensorDataToHistory(Sensor sensor)
        {
            SensorData data=new SensorData(sensor);
            db.SensorsData.Add(data);
            db.SaveChanges();
        }

      
    }
}