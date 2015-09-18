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
    public class SqlEfRepository:INodesRepository
    {
        private bool showDebugMessages = true;
        private bool showConsoleMessages = false;

        //if store time==0, every message will be instantly recorded to DB
        //and this will increase the reliability of the system, 
        //but this greatly slows down the performance.
        //If you set the interval, the state of all sensors will be recorded
        //to base with given interval.
        //the interval should be large enough (>3000 ms)
        private int storeTimeInterval = 5000;

        //slows down the performance, can cause an exception of a large flow of messages per second
        public bool storeMessages = false;

        private MyNetSensorsDbContext db;
        private Gateway gateway;
        private Timer updateDbTimer;

        //store id-s of updated nodes, to write to db by timer
        private List<int> updatedNodesId = new List<int>();
        //messages list, to store to db by timer
        private List<Message> newMessages = new List<Message>();


        public void Connect(Gateway gateway, string connectionString)
        {
            InitializeDB(connectionString);

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



        private void InitializeDB(string connectionString)
        {
            db = new MyNetSensorsDbContext(connectionString);
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

            List<Node> list = db.Nodes.Include(x => x.sensors).OrderBy(x => x.nodeId).ToList();

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

        private void StoreAllNodes()
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
            int nodesCount = updatedNodesId.Count;
            int messagesCount = newMessages.Count;
            int messages = nodesCount + messagesCount;
            if (messages == 0) return;
            Stopwatch sw = new Stopwatch();
            sw.Start();


            StoreUpdatedNodes();
            StoreNewMessages();

            sw.Stop();
            long elapsed = sw.ElapsedMilliseconds;
            float messagesPerSec = (float)messages / (float)elapsed * 1000;
            Log(String.Format("Store to DB: {0} ms ({1} inserts, {2} inserts/sec)", elapsed, messages, (int)messagesPerSec));
        }

        private void StoreNewMessages()
        {
            //to prevent changing of collection while writing to db is not yet finished
            Message[] messages = new Message[newMessages.Count];
            newMessages.CopyTo(messages);
            newMessages.Clear();

            db.Messages.AddRange(messages);
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

        private void StoreUpdatedNodes()
        {
            if (!updatedNodesId.Any()) return;

            //to prevent changing of collection while writing to db is not yet finished
            int[] nodesTemp = new int[updatedNodesId.Count];
            updatedNodesId.CopyTo(nodesTemp);
            updatedNodesId.Clear();

            List<Node> nodes = gateway.GetNodes();
            foreach (var id in nodesTemp)
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
            db.SaveChanges();
        }

        public bool IsConnected()
        {
            return db.Database.Exists();
        }

        public void Log(string message)
        {
            if (showDebugMessages)
                Debug.WriteLine(message);
            if (showConsoleMessages)
                Console.WriteLine(message);
        }
    }

   
}
