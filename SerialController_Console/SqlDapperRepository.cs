/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Dapper;
using MyNetSensors.SerialGateway;

namespace MyNetSensors.SerialController_Console
{
    class SqlDapperRepository : INodesRepository
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
        public bool storeTxRxMessages = false;

        private Gateway gateway;
        private Timer updateDbTimer=new Timer();

        //store id-s of updated nodes, to write to db by timer
        private List<int> updatedNodesId = new List<int>();
        //messages list, to store to db by timer
        private List<Message> newMessages = new List<Message>();

        private IDbConnection db;


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

            updateDbTimer.Elapsed += UpdateDbTimer;

            if (storeTimeInterval > 0)
            {
                updateDbTimer.Interval = storeTimeInterval;
                updateDbTimer.Start();
            }
        }



        private void InitializeDB(string connectionString)
        {
            // db= new SqlConnection("Data Source=.\\sqlexpress; Database= MyNetSensors; Integrated Security=True;");

            try
            {
                //db = new SqlConnection("Data Source=.\\sqlexpress; Database= master; Integrated Security=True;");
                db = new SqlConnection(connectionString + ";Database= master");
                db.Open();
                db.Execute("CREATE DATABASE [MyNetSensors]");
                db.Close();
                db.Dispose();
            }
            catch { }

            db = new SqlConnection(connectionString);

            try
            {
                db.Execute(
            @"CREATE TABLE [dbo].[Messages](
	            [db_Id] [int] IDENTITY(1,1) NOT NULL,
	            [nodeId] [int] NOT NULL,
	            [sensorId] [int] NOT NULL,
	            [messageType] [int] NOT NULL,
	            [ack] [bit] NOT NULL,
	            [subType] [int] NOT NULL,
	            [payload] [nvarchar](max) NULL,
	            [isValid] [bit] NOT NULL,
	            [incoming] [bit] NOT NULL,
	            [dateTime] [datetime] NOT NULL ) ON [PRIMARY] ");
            }
            catch { }

            try
            {
                db.Execute(
            @" CREATE TABLE [dbo].[Nodes](
	                [db_Id] [int] IDENTITY(1,1) NOT NULL,
	                [nodeId] [int] NOT NULL,
	                [registered] [datetime] NOT NULL,
	                [lastSeen] [datetime] NOT NULL,
	                [isRepeatingNode] [bit] NULL,
	                [name] [nvarchar](max) NULL,
	                [version] [nvarchar](max) NULL,
	                [batteryLevel] [int] NULL ) ON [PRIMARY] ");
            }
            catch { }

            try
            {
                db.Execute(
           @" CREATE TABLE [dbo].[Sensors](
	                [db_Id] [int] IDENTITY(1,1) NOT NULL,
	                [ownerNodeId] [int] NOT NULL,
	                [sensorId] [int] NOT NULL,
	                [sensorType] [int] NULL,
	                [description] [nvarchar](max) NULL,
	                [sensorDataJson] [nvarchar](max) NULL,
	                [Node_db_Id] [int] NULL) ON [PRIMARY] ");
            }
            catch { }

        }

        public void DropMessages()
        {
            newMessages.Clear();

            db.Query("TRUNCATE TABLE [Messages]");
        }

        public void DropNodes()
        {
            updatedNodesId.Clear();

            db.Query("TRUNCATE TABLE [Nodes]");
            db.Query("TRUNCATE TABLE [Sensors]");
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
            List<Message> messages = db.Query<Message>("SELECT * FROM Messages").ToList();
            return messages;
        }

        private void OnNewMessage(Message message)
        {
            if (!storeTxRxMessages) return;

            if (storeTimeInterval == 0)
                AddMessage(message);
            else
                newMessages.Add(message);
        }

        public void AddMessage(Message message)
        {
            var sqlQuery = "INSERT INTO Messages (nodeId, sensorId, messageType, ack, subType ,payload, isValid, incoming, dateTime) "
                + "VALUES(@nodeId, @sensorId, @messageType, @ack, @subType, @payload, @isValid, @incoming, @dateTime); "
                + "SELECT CAST(SCOPE_IDENTITY() as int)";
            db.Query(sqlQuery, message);
        }

        public List<Node> GetNodes()
        {
            var mapper = new EnittyOneToManyMapper<Node, Sensor, int>()
            {
                AddChildAction = (node, sensor) =>
                {
                    if (node.sensors == null)
                        node.sensors = new List<Sensor>();

                    node.sensors.Add(sensor);
                },
                ParentKey = (node) => node.db_Id
            };

            string joinQuery = "SELECT * FROM Nodes n JOIN Sensors s ON n.db_Id = s.Node_db_Id ORDER BY n.nodeId";

            var list = db.Query<Node, Sensor, Node>(joinQuery, mapper.Map, splitOn: "db_Id")
                    .Where(y => y != null).ToList();

            return list;
        }





        public void AddOrUpdateNode(Node node)
        {
            Node oldNode = db.Query<Node>("SELECT * FROM Nodes WHERE nodeId = @nodeId", new { node.nodeId }).SingleOrDefault();

            if (oldNode == null)
            {
                var sqlQuery = "INSERT INTO Nodes (nodeId, registered, lastSeen, isRepeatingNode, name ,version, batteryLevel) "
                    + "VALUES(@nodeId, @registered, @lastSeen, @isRepeatingNode, @name, @version, @batteryLevel); "
                    + "SELECT CAST(SCOPE_IDENTITY() as int)";
                db.Execute(sqlQuery, node);
            }
            else
            {
                var sqlQuery =
                    "UPDATE Nodes " +
                    "SET nodeId = @nodeId, " +
                    "registered  = @registered, " +
                    "lastSeen = @lastSeen, " +
                    "isRepeatingNode = @isRepeatingNode, " +
                    "name = @name, " +
                    "version = @version, " +
                    "batteryLevel = @batteryLevel " +
                    "WHERE nodeId = @nodeId";
                db.Execute(sqlQuery, node);
            }

            foreach (var sensor in node.sensors)
            {
                AddOrUpdateSensor(sensor);
            }
        }

        public void AddOrUpdateSensor(Sensor sensor)
        {
            Sensor oldSensor = db.Query<Sensor>("SELECT * FROM Sensors WHERE ownerNodeId = @ownerNodeId AND sensorId = @sensorId", new { ownerNodeId = sensor.ownerNodeId, sensorId = sensor.sensorId }).SingleOrDefault();
            int node_db_id = db.Query<Sensor>("SELECT * FROM Nodes WHERE nodeId = @nodeId", new { nodeId = sensor.ownerNodeId }).SingleOrDefault().db_Id;

            if (oldSensor == null)
            {
                var sqlQuery = "INSERT INTO Sensors (ownerNodeId, sensorId, sensorType, description, sensorDataJson ,Node_db_Id) "
                    + "VALUES(@ownerNodeId, @sensorId, @sensorType, @description, @sensorDataJson, @Node_db_Id); "
                    + "SELECT CAST(SCOPE_IDENTITY() as int)";
                db.Execute(sqlQuery, new
                {
                    ownerNodeId = sensor.ownerNodeId,
                    sensorId = sensor.sensorId,
                    sensorType = sensor.sensorType,
                    description = sensor.description,
                    sensorDataJson = sensor.sensorDataJson,
                    Node_db_Id = node_db_id
                });
            }
            else
            {
                var sqlQuery =
                    "UPDATE Sensors " +
                    "SET ownerNodeId = @ownerNodeId, " +
                    "sensorId  = @sensorId, " +
                    "sensorType = @sensorType, " +
                    "description = @description, " +
                    "sensorDataJson = @sensorDataJson, " +
                    "Node_db_Id = @Node_db_Id " +
                    "WHERE ownerNodeId = @ownerNodeId AND sensorId = @sensorId";
                db.Execute(sqlQuery, new
                {
                    ownerNodeId = sensor.ownerNodeId,
                    sensorId = sensor.sensorId,
                    sensorType = sensor.sensorType,
                    description = sensor.description,
                    sensorDataJson = sensor.sensorDataJson,
                    Node_db_Id = node_db_id
                });
            }
        }

        private void StoreAllNodes()
        {
            List<Node> nodes = gateway.GetNodes();
            foreach (var node in nodes)
            {
                AddOrUpdateNode(node);
            }
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

            var sqlQuery = "INSERT INTO Messages (nodeId, sensorId, messageType, ack, subType ,payload, isValid, incoming, dateTime) "
    + "VALUES(@nodeId, @sensorId, @messageType, @ack, @subType, @payload, @isValid, @incoming, @dateTime); "
    + "SELECT CAST(SCOPE_IDENTITY() as int)";
            db.Execute(sqlQuery, messages);

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
                AddOrUpdateNode(node);
            }
        }

        public bool IsConnected()
        {
            //return db.Database.Exists();
            return true;
        }

        public void ShowDebugInConsole(bool enable)
        {
            showConsoleMessages = enable;
        }

        public void SetStoreInterval(int ms)
        {
            storeTimeInterval = ms;
            updateDbTimer.Stop();
            if (storeTimeInterval > 0)
            {
                updateDbTimer.Interval = storeTimeInterval;
                updateDbTimer.Start();
            }
        }

        public void StoreTxRxMessages(bool enable)
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




    public class EnittyOneToManyMapper<TP, TC, TPk>
    {
        private readonly IDictionary<TPk, TP> _lookup = new Dictionary<TPk, TP>();

        public Action<TP, TC> AddChildAction { get; set; }

        public Func<TP, TPk> ParentKey { get; set; }


        public virtual TP Map(TP parent, TC child)
        {
            TP entity;
            var found = true;
            var primaryKey = ParentKey(parent);

            if (!_lookup.TryGetValue(primaryKey, out entity))
            {
                _lookup.Add(primaryKey, parent);
                entity = parent;
                found = false;
            }

            AddChildAction(entity, child);

            return !found ? entity : default(TP);

        }
    }
}
