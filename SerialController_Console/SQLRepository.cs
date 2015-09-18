/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Timers;
using System.IO;
using System.Linq;
using MyNetSensors.SerialGateway;

namespace MyNetSensors.SerialController_Console
{
    public class SQLRepository
    {
        //if store time==0, every message will be instantly recorded to DB
        //and this will increase the reliability of the system, 
        //but this greatly slows down the performance.
        //If you set the interval, the state of all sensors will be recorded
        //to base with given interval.
        //the interval should be large enough (>3000 ms)
        private int storeTimeInterval = 5000;

        //slows down the performance, can cause an exception of a large flow of messages per second
        private bool storeMessages = false;

        private MyNetSensorsDbContext db;

        private Gateway gateway;

        private Timer updateDbTimer;

        //store id-s of updated nodes, to write to db by timer
        private List<int> updatedNodesId = new List<int>();
        private List<Message> newMessages = new List<Message>();


        public SQLRepository(Gateway gateway)
        {
            InitializeDB();
            ConnectToController(gateway);
        }



        private void ConnectToController(Gateway gateway)
        {
            this.gateway = gateway;

            List<Message> messages = GetMessages();
            foreach (var message in messages)
                gateway.messagesLog.AddNewMessage(message);

            List<Node> nodes = GetNodes();
            foreach (var node in nodes)
                gateway.AddNode(node);

            gateway.messagesLog.OnNewMessageLogged += OnNewMessage;
            gateway.messagesLog.OnClearMessages += OnClearMessages;

            gateway.OnClearNodesList += OnClearNodesList;

            gateway.OnNewNodeEvent += OnNodeUpdated;
            gateway.OnNodeUpdatedEvent += OnNodeUpdated;
            gateway.OnNewSensorEvent += OnSensorUpdated;
            gateway.OnSensorUpdatedEvent += OnSensorUpdated;

            if (storeTimeInterval > 0)
            {
                updateDbTimer = new Timer();
                updateDbTimer.Interval = storeTimeInterval;
                updateDbTimer.Elapsed += UpdateDbTimer;
                updateDbTimer.Start();
            }
        }



        private void InitializeDB()
        {
            db = new MyNetSensorsDbContext();
        }

        public void DropMessages()
        {
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [" + "Messages" + "]");
        }

        public void DropNodes()
        {
            updatedNodesId.Clear();

            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [" + "Nodes" + "]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [" + "Sensors" + "]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [" + "SensorDatas" + "]");
        }


        private void OnClearMessages(object sender, EventArgs e)
        {
            DropMessages();
        }

        private void OnClearNodesList(object sender, EventArgs e)
        {
            DropNodes();
        }


        public List<Message> GetMessages()
        {
            List<Message> list = db.Messages.ToList();
            return list;
        }

        private void OnNewMessage(Message message)
        {
            if (!storeMessages)return;

            if (storeTimeInterval == 0)
                AddMessage(message);
            else
                newMessages.Add(message);
        }

        public void AddMessage(Message message)
        {
            db.Messages.Add(message);
            db.SaveChanges();
        }

        public List<Node> GetNodes()
        {

            List<Node> list = db.Nodes.Include(x => x.sensors.Select(d => d.sensorData)).ToList();
            /*     foreach (var node in list)
                 {
                     node.registered = node.registered.ToLocalTime();
                     node.lastSeen = node.lastSeen.ToLocalTime();
                 }
          */
            return list;
        }

        public void AddOrUpdateNode(Node node)
        {
            //     Node oldNode = db.Nodes.FirstOrDefault(x => x.nodeId == node.nodeId);

            if (db.Nodes.Any(x => x.nodeId == node.nodeId))
            {
                db.Entry(node).State = EntityState.Modified;
            }
            else
            {
                db.Nodes.Add(node);
            }
            db.SaveChanges();
        }

        public void AddOrUpdateSensor(Sensor sensor)
        {
            //   Sensor oldSensor = conn.Table<Sensor>().Where(x => x.nodeId == sensor.nodeId).FirstOrDefault(x => x.sensorId == sensor.sensorId);

            if (db.Sensors.Any(x => x.ownerNodeId == sensor.ownerNodeId && x.sensorId == sensor.sensorId))
            {
                db.Entry(sensor).State = EntityState.Modified;
            }
            else
            {
                db.Sensors.Add(sensor);
            }
            db.SaveChanges();

        }

        public void StoreAllNodes()
        {
            List<Node> nodes = gateway.GetNodes();
            foreach (var node in nodes)
            {
                if (db.Nodes.Any(x => x.nodeId == node.nodeId))
                {
                    db.Entry(node).State = EntityState.Modified;
                }
                else
                {
                    db.Nodes.Add(node);
                }
            }
            db.SaveChanges();
        }

        private void UpdateDbTimer(object sender, object e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            StoreUpdatedNodes();
            StoreNewMessages();

            sw.Stop();
            Debug.WriteLine("Store to DB: " + sw.ElapsedMilliseconds + " ms");
        }

        private void StoreNewMessages()
        {
            db.Messages.AddRange(newMessages);
            newMessages.Clear();
            db.SaveChanges();
        }


        private void OnNodeUpdated(Node node)
        {
            if (storeTimeInterval == 0) AddOrUpdateNode(node);
            else
            {
                if (!updatedNodesId.Contains(node.nodeId))
                    updatedNodesId.Add(node.nodeId);
            }
        }

        private void OnSensorUpdated(Sensor sensor)
        {
            if (storeTimeInterval == 0) AddOrUpdateSensor(sensor);
            else
            {
                if (!updatedNodesId.Contains(sensor.ownerNodeId))
                    updatedNodesId.Add(sensor.ownerNodeId);
            }
        }

        public void StoreUpdatedNodes()
        {
            if (!updatedNodesId.Any()) return;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<Node> nodes = gateway.GetNodes();
            foreach (var id in updatedNodesId)
            {
                Node node = nodes.FirstOrDefault(x => x.nodeId == id);

                if (db.Nodes.Any(x => x.nodeId == id))
                {
                    db.Entry(node).State = EntityState.Modified;
                }
                else
                {
                    db.Nodes.Add(node);
                }
            }
            updatedNodesId.Clear();
            db.SaveChanges();
        }

        public bool IsConnected()
        {
            return db.Database.Exists();
        }
    }

}
