/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/


/*


        !!!!!!!!!!!!!!!!!

        This class has been replaced with GatewayRepositoryDapper. 
        In this form it is no longer functional, but I left it to finish later.

        !!!!!!!!!!!!!!!!!

*/





using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using MyNetSensors.Gateway;

namespace MyNetSensors.GatewayRepository
{
    public class GatewayRepositoryEf:IGatewayRepository
    {
        private bool showDebugMessages = true;
        private bool showConsoleMessages = false;

        //if writeInterval==0, every message will be instantly writing to DB
        //and this will increase the reliability of the system, 
        //but this greatly slows down the performance.
        //If you set writeInterval>0, the state of all sensors 
        //will be writed to DB with this interval.
        //writeInterval should be large enough (3000 ms is ok)
        private int writeInterval = 5000;

        //slows down the performance, can cause an exception of a large flow of messages per second
        public bool storeTxRxMessages = false;

        private GatewayRepositoryEfDbContext db;
        private SerialGateway gateway;
        private Timer updateDbTimer=new Timer();

        //store id-s of updated nodes, to write to db by timer
        private List<int> updatedNodesId = new List<int>();
        //messages list, to write to db by timer
        private List<Message> newMessages = new List<Message>();

        public GatewayRepositoryEf(string connectionString)
        {
            InitializeDB(connectionString);
        }

        public void ConnectToGateway(SerialGateway gateway)
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

            gateway.OnClearNodesListEvent += OnClearNodesListEvent;

            gateway.OnNewNodeEvent += OnNodeUpdated;
            gateway.OnNodeUpdatedEvent += OnNodeUpdated;
            gateway.OnNewSensorEvent += OnSensorUpdated;
            gateway.OnSensorUpdatedEvent += OnSensorUpdated;

            updateDbTimer.Elapsed += UpdateDbTimer;

            if (writeInterval > 0)
            {
                updateDbTimer.Interval = writeInterval;
                updateDbTimer.Start();
            }
        }



        private void InitializeDB(string connectionString)
        {
            db = new GatewayRepositoryEfDbContext(connectionString);
        }

        public void DropMessages()
        {
            newMessages.Clear();

            db.Database.ExecuteSqlCommand("TRUNCATE TABLE Messages");
        }

        public Sensor GetSensor(int ownerNodeId, int sensorId)
        {
            throw new NotImplementedException();
        }

        public void DropNodes()
        {
            updatedNodesId.Clear();

            db.Database.ExecuteSqlCommand("TRUNCATE TABLE Nodes");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE Sensors");
        }


        private void OnClearMessages(object sender, EventArgs e)
        {
            DropMessages();
        }

        private void OnClearNodesListEvent(object sender, EventArgs e)
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
            if (!storeTxRxMessages)return;

            if (writeInterval == 0)
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

        public Node GetNodeByDbId(int db_Id)
        {
            throw new NotImplementedException();
        }

        public Node GetNodeByNodeId(int nodeId)
        {
            throw new NotImplementedException();
        }

        public Sensor GetSensor(int db_Id)
        {
            throw new NotImplementedException();
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

        private void WriteAllNodes()
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
            updateDbTimer.Stop();
            int nodesCount = updatedNodesId.Count;
            int messagesCount = newMessages.Count;
            int messages = nodesCount + messagesCount;
            if (messages == 0) return;
            Stopwatch sw = new Stopwatch();
            sw.Start();


            WriteUpdatedNodes();
            WriteNewMessages();

            sw.Stop();
            long elapsed = sw.ElapsedMilliseconds;
            float messagesPerSec = (float)messages / (float)elapsed * 1000;
            Log(String.Format("Writing to DB: {0} ms ({1} inserts, {2} inserts/sec)", elapsed, messages, (int)messagesPerSec));
            updateDbTimer.Start();

        }

        private void WriteNewMessages()
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
            if (writeInterval == 0) AddOrUpdateNode(node);
            else
            {
                if (!updatedNodesId.Contains(node.nodeId))
                    updatedNodesId.Add(node.nodeId);
            }
        }

        private void OnSensorUpdated(Sensor sensor)
        {
            if (writeInterval == 0) AddOrUpdateSensor(sensor);
            else
            {
                if (!updatedNodesId.Contains(sensor.ownerNodeId))
                    updatedNodesId.Add(sensor.ownerNodeId);
            }
        }

        private void WriteUpdatedNodes()
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

        public bool IsDbExist()
        {
            return db.Database.Exists();
        }

        public void ShowDebugInConsole(bool enable)
        {
            showConsoleMessages = enable;
        }

        public void UpdateNodeSettings(Node node)
        {
            throw new NotImplementedException();
        }

        public void UpdateSensorSettings(Sensor sensor)
        {
            throw new NotImplementedException();
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

        public void SetStoreTxRxMessages(bool enable)
        {
            storeTxRxMessages = enable;
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
