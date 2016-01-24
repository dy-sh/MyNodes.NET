/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Microsoft.Data.Entity;
using MyNetSensors.Gateways;
using MyNetSensors.Gateways.MySensors.Serial;

namespace MyNetSensors.Repositories.EF.SQLite
{




    public class MySensorsRepositoryEf : IMySensorsRepository
    {
        private string connectionString;

        //if writeInterval==0, every message will be instantly writing to DB
        //and this will increase the reliability of the system, 
        //but this greatly slows down the performance.
        //If you set writeInterval>0, the state of all sensors 
        //will be writed to DB with this interval.
        //writeInterval should be large enough (3000 ms is ok)
        private int writeInterval = 5000;

        private Timer updateDbTimer = new Timer();

        //store updated nodes, to write to db by timer
        private List<Node> updatedNodes = new List<Node>();
        private List<Sensor> updatedSensors = new List<Sensor>();

        private MySensorsNodesDbContext db;

        public event LogEventHandler OnLogInfo;
        public event LogEventHandler OnLogError;

        public MySensorsRepositoryEf(MySensorsNodesDbContext mySensorsNodesDbContext)
        {
            updateDbTimer.Elapsed += UpdateDbTimerEvent;

            this.db = mySensorsNodesDbContext;
            CreateDb();

            if (writeInterval > 0)
            {
                updateDbTimer.Interval = writeInterval;
                updateDbTimer.Start();
            }
        }





        public void CreateDb()
        {
            try
            {
                db.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public void RemoveAllNodesAndSensors()
        {
            updatedNodes.Clear();
            updatedSensors.Clear();

            db.Sensors.RemoveRange(db.Sensors);
            db.Nodes.RemoveRange(db.Nodes);
            db.SaveChanges();
        }








        public List<Node> GetNodes()
        {
            return db.Nodes.Include(x => x.sensors).ToList();
        }




        public int AddNode(Node node)
        {
            //todo EF change nodeID automaticaly!!!!
            db.Nodes.Add(node);
            db.SaveChanges();

            foreach (var sensor in node.sensors)
            {
                AddSensor(sensor);
            }

            return node.Id;
        }

        public void UpdateNode(Node node)
        {
            if (writeInterval == 0)
            {
                db.Nodes.Update(node);
                db.SaveChanges();
            }
            else
            {
                if (!updatedNodes.Contains(node))
                    updatedNodes.Add(node);
            }
        }

        public int AddSensor(Sensor sensor)
        {
            db.Sensors.Add(sensor);
            db.SaveChanges();
            return sensor.Id;
        }

        public void UpdateSensor(Sensor sensor)
        {
            if (writeInterval == 0)
            {
                db.Sensors.Update(sensor);
                db.SaveChanges();
            }
            else
            {
                if (!updatedSensors.Contains(sensor))
                    updatedSensors.Add(sensor);
            }
        }






        private void WriteUpdated()
        {
            if (!updatedNodes.Any() && !updatedSensors.Any())
                return;

            if (updatedNodes.Any())
            {

                //to prevent changing of collection while writing to db is not yet finished
                Node[] nodesTemp = new Node[updatedNodes.Count];
                updatedNodes.CopyTo(nodesTemp);
                updatedNodes.Clear();

                db.Nodes.UpdateRange(nodesTemp);
            }

            if (updatedSensors.Any())
            {

                //to prevent changing of collection while writing to db is not yet finished
                Sensor[] sensorsTemp = new Sensor[updatedSensors.Count];
                updatedSensors.CopyTo(sensorsTemp);
                updatedSensors.Clear();

                db.Sensors.UpdateRange(sensorsTemp);
            }

            db.SaveChanges();
        }



        public Node GetNode(int id)
        {
            return db.Nodes.FirstOrDefault(x => x.Id == id);
        }

        public Sensor GetSensor(int id)
        {
            try
            {
                Sensor sensor = db.Sensors.FirstOrDefault(x => x.Id == id);
                return sensor;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Sensor GetSensor(int nodeId, int sensorId)
        {
            return db.Sensors.FirstOrDefault(x => x.nodeId == nodeId && x.sensorId == sensorId);
        }



        public void RemoveNode(int id)
        {
            Node node = db.Nodes.FirstOrDefault(x => x.Id == id);
            if (node == null)
                return;

            db.Nodes.Remove(node);
            db.SaveChanges();

            List<Sensor> sensors = db.Sensors.Where(x => x.nodeId == node.Id).ToList();
            db.Sensors.RemoveRange(sensors);
            db.SaveChanges();

        }




        private void UpdateDbTimerEvent(object sender, object e)
        {
            updateDbTimer.Stop();
            try
            {
                int messages = updatedNodes.Count + updatedSensors.Count;
                if (messages == 0)
                {
                    updateDbTimer.Start();
                    return;
                }

                Stopwatch sw = new Stopwatch();
                sw.Start();

                WriteUpdated();

                sw.Stop();
                long elapsed = sw.ElapsedMilliseconds;
                float messagesPerSec = (float)messages / (float)elapsed * 1000;
                LogInfo($"Writing hardware nodes: {elapsed} ms ({messages} inserts, {(int)messagesPerSec} inserts/sec)");
            }
            catch { }

            updateDbTimer.Start();
        }

        public void SetWriteInterval(int ms)
        {
            writeInterval = ms;
            updateDbTimer.Stop();
            if (writeInterval > 0)
            {
                updateDbTimer.Interval = writeInterval;
                updateDbTimer.Start();
            }
        }

        public void LogInfo(string message)
        {
            OnLogInfo?.Invoke(message);
        }

        public void LogError(string message)
        {
            OnLogError?.Invoke(message);
        }
    }



}
