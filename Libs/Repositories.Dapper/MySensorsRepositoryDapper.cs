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
using Dapper;
using MyNetSensors.Gateways;
using MyNetSensors.Gateways.MySensors.Serial;

namespace MyNetSensors.Repositories.Dapper
{




    public class MySensorsRepositoryDapper : IMySensorsRepository
    {
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

        private string connectionString;

        public event LogEventHandler OnLogInfo;
        public event LogEventHandler OnLogError;

        public MySensorsRepositoryDapper(string connectionString)
        {
            updateDbTimer.Elapsed += UpdateDbTimerEvent;

            this.connectionString = connectionString;
            CreateDb();

            if (writeInterval > 0)
            {
                updateDbTimer.Interval = writeInterval;
                updateDbTimer.Start();
            }
        }




        private void CreateDb()
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
                    db.Execute(
                        @" CREATE TABLE [dbo].[MySensorsNodes](
	                [Id] [int] NOT NULL,
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
                        @" CREATE TABLE [dbo].[MySensorsSensors](
	                [Id] [int] IDENTITY(1,1) NOT NULL,
	                [nodeId] [int] NOT NULL,
	                [sensorId] [int] NOT NULL,
	                [type] [int] NULL,
	                [dataType] [int] NULL,
	                [state] [nvarchar](max) NULL,
	                [description] [nvarchar](max) NULL,	              
	                ) ON [PRIMARY] ");
                }
                catch
                {
                }
            }
        }

    

        public void RemoveAllNodesAndSensors()
        {
            updatedNodes.Clear();
            updatedSensors.Clear();

            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("TRUNCATE TABLE [MySensorsNodes]");
                db.Query("TRUNCATE TABLE [MySensorsSensors]");
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
                ParentKey = (node) => node.Id
            };

            List<Node> list;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                string joinQuery = "SELECT * FROM MySensorsNodes n JOIN MySensorsSensors s ON n.Id = s.nodeId ORDER BY n.Id";

                list = db.Query<Node, Sensor, Node>(joinQuery, mapper.Map, splitOn: "Id")
                     .Where(y => y != null).ToList();
            }
            return list;
        }



        

        public int AddNode(Node node)
        {

            using (var db = new SqlConnection(connectionString))
            {
                var sqlQuery = "INSERT INTO MySensorsNodes (Id, registered, lastSeen, isRepeatingNode, name ,version, batteryLevel) "
                               +
                               "VALUES(@Id, @registered, @lastSeen, @isRepeatingNode, @name, @version, @batteryLevel)";
                               
                db.Query(sqlQuery, node);
               // gateway.SetNodeDbId(node.nodeId, id);
            }

            return node.Id;
        }

        public void UpdateNode(Node node)
        {
            if (writeInterval == 0)
            {
                using (var db = new SqlConnection(connectionString))
                {
                    var sqlQuery =
                        "UPDATE MySensorsNodes SET " +
                        "registered = @registered, " +
                        "lastSeen = @lastSeen, " +
                        "isRepeatingNode = @isRepeatingNode, " +
                        "name = @name, " +
                        "version = @version, " +
                        "batteryLevel = @batteryLevel " +
                        "WHERE Id = @Id";
                    db.Execute(sqlQuery, node);
                }
            }
            else
            {
                if (!updatedNodes.Contains(node))
                    updatedNodes.Add(node);
            }
        }


        public int AddSensor(Sensor sensor)
        {
            int id;
            using (var db = new SqlConnection(connectionString))
            {

                var sqlQuery = "INSERT INTO MySensorsSensors (nodeId, sensorId, type, dataType,state, description) "
                               +
                               "VALUES(@nodeId, @sensorId, @type, @dataType ,@state, @description); "
                               + "SELECT CAST(SCOPE_IDENTITY() as int)";
                id = db.Query<int>(sqlQuery, sensor).Single();

            }
            return id;
        }


        public void UpdateSensor(Sensor sensor)
        {
            if (writeInterval == 0)
            {
                using (var db = new SqlConnection(connectionString))
                {
                    db.Open();

                    var sqlQuery =
                        "UPDATE MySensorsSensors SET " +
                        "nodeId = @nodeId, " +
                        "sensorId  = @sensorId, " +
                        "type = @type, " +
                        "dataType = @dataType, " +
                        "state = @state, " +
                        "description = @description " +
                        "WHERE nodeId = @nodeId AND sensorId = @sensorId";
                    db.Execute(sqlQuery, sensor);
                }
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

                using (var db = new SqlConnection(connectionString))
                {
                    var sqlQuery =
                        "UPDATE MySensorsNodes SET " +
                        "registered = @registered, " +
                        "lastSeen = @lastSeen, " +
                        "isRepeatingNode = @isRepeatingNode, " +
                        "name = @name, " +
                        "version = @version, " +
                        "batteryLevel = @batteryLevel " +
                        "WHERE Id = @Id";
                    db.Execute(sqlQuery, nodesTemp);
                }
            }

            if (updatedSensors.Any())
            {

                //to prevent changing of collection while writing to db is not yet finished
                Sensor[] sensorsTemp = new Sensor[updatedSensors.Count];
                updatedSensors.CopyTo(sensorsTemp);
                updatedSensors.Clear();

                using (var db = new SqlConnection(connectionString))
                {
                    var sqlQuery =
                        "UPDATE MySensorsSensors SET " +
                        "nodeId = @nodeId, " +
                        "sensorId  = @sensorId, " +
                        "type = @type, " +
                        "dataType = @dataType, " +
                        "state = @state, " +
                        "description = @description " +
                        "WHERE nodeId = @nodeId AND sensorId = @sensorId";
                    db.Execute(sqlQuery, sensorsTemp);
                }
            }
        }






        public Node GetNode(int id)
        {
            var mapper = new OneToManyDapperMapper<Node, Sensor, int>()
            {
                AddChildAction = (node, sensor) =>
                {
                    if (node.sensors == null)
                        node.sensors = new List<Sensor>();

                    node.sensors.Add(sensor);
                },
                ParentKey = (node) => node.Id
            };

            string joinQuery = $"SELECT * FROM MySensorsNodes n JOIN MySensorsSensors s ON n.Id = s.nodeId WHERE n.Id = {id}";

            Node result;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                result = db.Query<Node, Sensor, Node>(joinQuery, mapper.Map, splitOn: "Id").FirstOrDefault();
            }

            return result;
        }

        public Sensor GetSensor(int id)
        {
            Sensor sensor;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                sensor = db.Query<Sensor>("SELECT * FROM MySensorsSensors WHERE Id = @Id", new { Id = id }).FirstOrDefault();
            }
            return sensor;
        }

        public Sensor GetSensor(int nodeId, int sensorId)
        {
            Sensor sensor;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                sensor = db.Query<Sensor>("SELECT * FROM MySensorsSensors WHERE nodeId = @nodeId AND sensorId = @sensorId",
                        new { nodeId, sensorId }).FirstOrDefault();
            }
            return sensor;
        }

        

        public void RemoveNode(int id)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "Delete FROM MySensorsNodes " +
                    "WHERE Id = @Id";
                db.Execute(sqlQuery, new { Id = id });

                sqlQuery =
                    "Delete FROM MySensorsSensors " +
                    "WHERE Id = @Id";
                db.Execute(sqlQuery, new { Id = id });
            }
        }
        







        private void UpdateDbTimerEvent(object sender, object e)
        {
            updateDbTimer.Stop();
            try
            {
                int messages = updatedNodes.Count;
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
                float messagesPerSec = (float) messages/(float) elapsed*1000;
                LogInfo($"Writing to DB: {elapsed} ms ({messages} inserts, {(int) messagesPerSec} inserts/sec)");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

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
