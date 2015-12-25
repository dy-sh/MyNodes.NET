/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Dapper;
using MyNetSensors.Gateway;

namespace MyNetSensors.GatewayRepository
{




    public class GatewayRepositoryDapper : IGatewayRepository
    {
        private bool showConsoleMessages = false;

        //if writeInterval==0, every message will be instantly writing to DB
        //and this will increase the reliability of the system, 
        //but this greatly slows down the performance.
        //If you set writeInterval>0, the state of all sensors 
        //will be writed to DB with this interval.
        //writeInterval should be large enough (3000 ms is ok)
        private int writeInterval = 5000;

        //slows down the performance, can cause to exception of a large flow of messages per second
        public bool storeTxRxMessages = false;

        private SerialGateway gateway;
        private Timer updateDbTimer = new Timer();

        //store id-s of updated nodes, to write to db by timer
        private List<int> updatedNodesId = new List<int>();
        //messages list, to write to db by timer
        private List<Message> newMessages = new List<Message>();

        private string connectionString;

        public GatewayRepositoryDapper(string connectionString)
        {
            updateDbTimer.Elapsed += UpdateDbTimerEvent;

            this.connectionString = connectionString;
            InitializeDB();
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


            if (writeInterval > 0)
            {
                updateDbTimer.Interval = writeInterval;
                updateDbTimer.Start();
            }
        }



        private void InitializeDB()
        {
            using (var db = new SqlConnection(connectionString + ";Database= master"))
            {

                try
                {
                    //db = new SqlConnection("Data Source=.\\sqlexpress; Database= master; Integrated Security=True;");
                    db.Open();
                    db.Execute("CREATE DATABASE [MyNetSensors]");
                }
                catch
                {
                }
            }

            using (var db = new SqlConnection(connectionString))
            {

                try
                {
                    db.Open();

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
                catch
                {
                }

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
                catch
                {
                }

                try
                {
                    db.Execute(
                        @" CREATE TABLE [dbo].[Sensors](
	                [db_Id] [int] IDENTITY(1,1) NOT NULL,
	                [nodeId] [int] NOT NULL,
	                [sensorId] [int] NOT NULL,
	                [type] [int] NULL,
	                [dataType] [int] NULL,
	                [state] [nvarchar](max) NULL,
	                [description] [nvarchar](max) NULL,
	                [storeHistoryEnabled] [bit] NULL,
	                [storeHistoryEveryChange] [bit] NULL,
	                [storeHistoryWithInterval] [int] NULL,
	                [invertData] [bit] NULL,
	                [remapEnabled] [bit] NULL,
	                [remapFromMin] [nvarchar](max) NULL,
	                [remapFromMax] [nvarchar](max) NULL,
	                [remapToMin] [nvarchar](max) NULL,
	                [remapToMax] [nvarchar](max) NULL,
	                [Node_db_Id] [int] NULL) ON [PRIMARY] ");
                }
                catch
                {
                }
            }
        }

        public void DropMessages()
        {
            newMessages.Clear();

            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                db.Query("TRUNCATE TABLE [Messages]");
            }
        }

        public void DropNodes()
        {
            updatedNodesId.Clear();

            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("TRUNCATE TABLE [Nodes]");
                db.Query("TRUNCATE TABLE [Sensors]");
            }
        }


        private void OnClearMessages()
        {
            DropMessages();
        }

        private void OnClearNodesListEvent()
        {
            DropNodes();
        }


        public List<Message> GetMessages()
        {
            List<Message> messages;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                messages = db.Query<Message>("SELECT * FROM Messages").ToList();
            }
            return messages;
        }

        private void OnNewMessage(Message message)
        {
            if (!storeTxRxMessages) return;

            if (writeInterval == 0)
                AddMessage(message);
            else
                newMessages.Add(message);
        }

        public void AddMessage(Message message)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery = "INSERT INTO Messages (nodeId, sensorId, messageType, ack, subType ,payload, isValid, incoming, dateTime) "
                               +
                               "VALUES(@nodeId, @sensorId, @messageType, @ack, @subType, @payload, @isValid, @incoming, @dateTime); "
                               + "SELECT CAST(SCOPE_IDENTITY() as int)";
                db.Query(sqlQuery, message);
            }
        }

        public List<Node> GetNodes()
        {
            var mapper = new OneToManyDapperMapper<Node, Sensor, int>()
            {
                AddChildAction = (node, sensor) =>
                {
                    if (node.sensors == null)
                        node.sensors = new List<Sensor>();

                    node.sensors.Add(sensor);
                },
                ParentKey = (node) => node.db_Id
            };

            List<Node> list;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                string joinQuery = "SELECT * FROM Nodes n JOIN Sensors s ON n.db_Id = s.Node_db_Id ORDER BY n.nodeId";

                list = db.Query<Node, Sensor, Node>(joinQuery, mapper.Map, splitOn: "db_Id")
                     .Where(y => y != null).ToList();
            }
            return list;
        }





        public void AddOrUpdateNode(Node node)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                Node oldNode =
                    db.Query<Node>("SELECT * FROM Nodes WHERE nodeId = @nodeId", new { node.nodeId }).SingleOrDefault();

                if (oldNode == null)
                {
                    var sqlQuery = "INSERT INTO Nodes (nodeId, registered, lastSeen, isRepeatingNode, name ,version, batteryLevel) "
                                   +
                                   "VALUES(@nodeId, @registered, @lastSeen, @isRepeatingNode, @name, @version, @batteryLevel); "
                                   + "SELECT CAST(SCOPE_IDENTITY() as int)";
                    int dbId = db.Query<int>(sqlQuery, node).Single();
                    gateway.SetNodeDbId(node.nodeId, dbId);
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
            }

            foreach (var sensor in node.sensors)
            {
                AddOrUpdateSensor(sensor);
            }
        }

        public void AddOrUpdateSensor(Sensor sensor)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                Sensor oldSensor =
                    db.Query<Sensor>("SELECT * FROM Sensors WHERE nodeId = @nodeId AND sensorId = @sensorId",
                        new { nodeId = sensor.nodeId, sensorId = sensor.sensorId }).SingleOrDefault();
                int node_db_id =
                    db.Query<Sensor>("SELECT * FROM Nodes WHERE nodeId = @nodeId", new { nodeId = sensor.nodeId })
                        .SingleOrDefault()
                        .db_Id;

                if (oldSensor == null)
                {
                    var sqlQuery = "INSERT INTO Sensors (nodeId, sensorId, type, dataType,state, description, storeHistoryEnabled, storeHistoryEveryChange, storeHistoryWithInterval, invertData, remapEnabled, remapFromMin, remapFromMax, remapToMin, remapToMax, Node_db_Id) "
                                   +
                                   "VALUES(@nodeId, @sensorId, @type, @dataType ,@state, @description,  @storeHistoryEnabled, @storeHistoryEveryChange, @storeHistoryWithInterval, @invertData, @remapEnabled, @remapFromMin, @remapFromMax, @remapToMin, @remapToMax, @Node_db_Id); "
                                   + "SELECT CAST(SCOPE_IDENTITY() as int)";
                    int dbId = db.Query<int>(sqlQuery, new
                    {
                        nodeId = sensor.nodeId,
                        sensorId = sensor.sensorId,
                         type= sensor.type,
                        dataType= sensor.dataType,
                        state= sensor.state,
                        description = sensor.description,
                        storeHistoryEnabled = sensor.storeHistoryEnabled,
                        storeHistoryEveryChange = sensor.storeHistoryEveryChange,
                        storeHistoryWithInterval = sensor.storeHistoryWithInterval,
                        invertData = sensor.invertData,
                        remapEnabled = sensor.remapEnabled,
                        remapFromMin = sensor.remapFromMin,
                        remapFromMax = sensor.remapFromMax,
                        remapToMin = sensor.remapToMin,
                        remapToMax = sensor.remapToMax,
                        Node_db_Id = node_db_id
                    }).Single();

                    gateway.SetSensorDbId(sensor.nodeId, sensor.sensorId, dbId);
                }
                else
                {
                    var sqlQuery =
                        "UPDATE Sensors SET " +
                        "nodeId = @nodeId, " +
                        "sensorId  = @sensorId, " +
                        "type = @type, " +
                        "dataType = @dataType, " +
                        "state = @state, " +
                        "description = @description, " +
                        "storeHistoryEnabled = @storeHistoryEnabled, " +
                        "storeHistoryWithInterval = @storeHistoryWithInterval, " +
                        "storeHistoryEveryChange = @storeHistoryEveryChange, " +
                        "invertData = @invertData, " +
                        "remapEnabled = @remapEnabled, " +
                        "remapFromMin = @remapFromMin, " +
                        "remapFromMax = @remapFromMax, " +
                        "remapToMin = @remapToMin, " +
                        "remapToMax = @remapToMax, " +
                        "Node_db_Id = @Node_db_Id " +
                        "WHERE nodeId = @nodeId AND sensorId = @sensorId";
                    db.Execute(sqlQuery, new
                    {
                        nodeId = sensor.nodeId,
                        sensorId = sensor.sensorId,
                        type = sensor.type,
                        dataType = sensor.dataType,
                        state = sensor.state,
                        description = sensor.description,
                        storeHistoryEnabled = sensor.storeHistoryEnabled,
                        storeHistoryWithInterval = sensor.storeHistoryWithInterval,
                        storeHistoryEveryChange = sensor.storeHistoryEveryChange,
                        invertData = sensor.invertData,
                        remapEnabled = sensor.remapEnabled,
                        remapFromMin = sensor.remapFromMin,
                        remapFromMax = sensor.remapFromMax,
                        remapToMin = sensor.remapToMin,
                        remapToMax = sensor.remapToMax,
                        Node_db_Id = node_db_id
                    });
                }
            }
        }





        private void WriteAllNodes()
        {
            List<Node> nodes = gateway.GetNodes();
            foreach (var node in nodes)
            {
                AddOrUpdateNode(node);
            }
        }


        private void WriteNewMessages()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                //to prevent changing of collection while writing to db is not yet finished
                Message[] messages = new Message[newMessages.Count];
                newMessages.CopyTo(messages);
                newMessages.Clear();

                var sqlQuery = "INSERT INTO Messages (nodeId, sensorId, messageType, ack, subType ,payload, isValid, incoming, dateTime) "
                               +
                               "VALUES(@nodeId, @sensorId, @messageType, @ack, @subType, @payload, @isValid, @incoming, @dateTime); "
                               + "SELECT CAST(SCOPE_IDENTITY() as int)";
                db.Execute(sqlQuery, messages);
            }
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
                if (!updatedNodesId.Contains(sensor.nodeId))
                    updatedNodesId.Add(sensor.nodeId);
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
                AddOrUpdateNode(node);
            }
        }

        public bool IsDbExist()
        {
            //todo check if db exist
            return true;
        }

        public void ShowDebugInConsole(bool enable)
        {
            showConsoleMessages = enable;
        }



        public void SetStoreTxRxMessages(bool enable)
        {
            storeTxRxMessages = enable;
        }





        public Node GetNodeByNodeId(int nodeId)
        {
            var mapper = new OneToManyDapperMapper<Node, Sensor, int>()
            {
                AddChildAction = (node, sensor) =>
                {
                    if (node.sensors == null)
                        node.sensors = new List<Sensor>();

                    node.sensors.Add(sensor);
                },
                ParentKey = (node) => node.db_Id
            };


            Node result;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                string joinQuery = $"SELECT * FROM Nodes n JOIN Sensors s ON n.db_Id = s.Node_db_Id WHERE n.nodeId = {nodeId}";
                result = db.Query<Node, Sensor, Node>(joinQuery, mapper.Map, splitOn: "db_Id").FirstOrDefault();
                if (result == null)
                {
                    joinQuery = $"SELECT * FROM Nodes WHERE nodeId = {nodeId}";
                    result = db.Query<Node>(joinQuery).FirstOrDefault();
                }
            }

            return result;
        }


        public Node GetNodeByDbId(int db_Id)
        {
            var mapper = new OneToManyDapperMapper<Node, Sensor, int>()
            {
                AddChildAction = (node, sensor) =>
                {
                    if (node.sensors == null)
                        node.sensors = new List<Sensor>();

                    node.sensors.Add(sensor);
                },
                ParentKey = (node) => node.db_Id
            };

            string joinQuery = $"SELECT * FROM Nodes n JOIN Sensors s ON n.db_Id = s.Node_db_Id WHERE n.db_Id = {db_Id}";

            Node result;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                result = db.Query<Node, Sensor, Node>(joinQuery, mapper.Map, splitOn: "db_Id").FirstOrDefault();
            }

            return result;
        }

        public Sensor GetSensor(int db_Id)
        {
            Sensor sensor;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                sensor = db.Query<Sensor>("SELECT * FROM Sensors WHERE db_Id = @db_Id", new { db_Id }).FirstOrDefault();
            }
            return sensor;
        }

        public Sensor GetSensor(int nodeId, int sensorId)
        {
            Sensor sensor;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                sensor = db.Query<Sensor>("SELECT * FROM Sensors WHERE nodeId = @nodeId AND sensorId = @sensorId",
                        new { nodeId, sensorId }).FirstOrDefault();
            }
            return sensor;
        }


        public void UpdateNodeSettings(Node node)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE Nodes SET " +
                    "name = @name " +
                    "WHERE nodeId = @nodeId";
                db.Execute(sqlQuery, node);
            }

            foreach (var sensor in node.sensors)
            {
                UpdateSensorSettings(sensor);
            }
        }

        public void UpdateSensorSettings(Sensor sensor)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE Sensors SET " +
                    "description = @description, " +
                    "storeHistoryEnabled = @storeHistoryEnabled, " +
                    "storeHistoryEveryChange = @storeHistoryEveryChange, " +
                    "storeHistoryWithInterval = @storeHistoryWithInterval, " +
                    "invertData = @invertData, " +
                    "remapEnabled = @remapEnabled, " +
                    "remapFromMin = @remapFromMin, " +
                    "remapFromMax = @remapFromMax, " +
                    "remapToMin = @remapToMin, " +
                    "remapToMax = @remapToMax " +
                    "WHERE nodeId = @nodeId AND sensorId = @sensorId";
                db.Execute(sqlQuery, new
                {
                    description = sensor.description,
                    storeHistoryEnabled = sensor.storeHistoryEnabled,
                    storeHistoryEveryChange = sensor.storeHistoryEveryChange,
                    storeHistoryWithInterval = sensor.storeHistoryWithInterval,
                    invertData = sensor.invertData,
                    remapEnabled = sensor.remapEnabled,
                    remapFromMin = sensor.remapFromMin,
                    remapFromMax = sensor.remapFromMax,
                    remapToMin = sensor.remapToMin,
                    remapToMax = sensor.remapToMax,
                    sensorId = sensor.sensorId,
                    nodeId = sensor.nodeId
                });
            }
        }

        public void DeleteNodeByDbId(int db_Id)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "Delete FROM Nodes " +
                    "WHERE db_Id = @db_Id";
                db.Execute(sqlQuery,new { db_Id });

                sqlQuery =
                    "Delete FROM Sensors " +
                    "WHERE Node_db_Id = @Node_db_Id";
                db.Execute(sqlQuery, new { Node_db_Id= db_Id });
            }
        }

        public void DeleteNodeByNodeId(int nodeId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "Delete FROM Nodes " +
                    "WHERE nodeId = @nodeId";
                db.Execute(sqlQuery, new { nodeId });


                sqlQuery =
                    "Delete FROM Sensors " +
                    "WHERE nodeId = @nodeId";
                db.Execute(sqlQuery, new { nodeId = nodeId });
            }
        }







        private void UpdateDbTimerEvent(object sender, object e)
        {
            updateDbTimer.Stop();
            try
            {
                int nodesCount = updatedNodesId.Count;
                int messagesCount = newMessages.Count;
                int messages = nodesCount + messagesCount;
                if (messages == 0)
                {
                    updateDbTimer.Start();
                    return;
                };
                Stopwatch sw = new Stopwatch();
                sw.Start();


                WriteUpdatedNodes();
                WriteNewMessages();

                sw.Stop();
                long elapsed = sw.ElapsedMilliseconds;
                float messagesPerSec = (float)messages / (float)elapsed * 1000;
                Log($"Writing to DB: {elapsed} ms ({messages} inserts, {(int)messagesPerSec} inserts/sec)");
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
                if (gateway != null)
                    updateDbTimer.Start();
            }
        }

        public void Log(string message)
        {
            if (showConsoleMessages)
                Console.WriteLine(message);
        }

    }



}
