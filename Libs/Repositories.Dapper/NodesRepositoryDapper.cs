/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Dapper;
using MyNetSensors.Nodes;

namespace MyNetSensors.Repositories.Dapper
{
    public class NodesRepositoryDapper : INodesRepository
    {
        //if writeInterval==0, every node will be instantly writing to DB
        //and this will increase the reliability of the system, 
        //but this greatly slows down the performance.
        //If you set writeInterval>0, nodes
        //will be writed to DB with this interval.
        //writeInterval should be large enough (3000 ms is ok)
        private int writeInterval = 5000;

        private Timer updateDbTimer = new Timer();

        private List<Node> updatedNodes = new List<Node>();

        public event LogEventHandler OnLogInfo;
        public event LogEventHandler OnLogError;

        private string connectionString;

        public NodesRepositoryDapper(string connectionString)
        {
            this.connectionString = connectionString;
            CreateDb();

            updateDbTimer.Elapsed += UpdateDbTimerEvent;
            if (writeInterval > 0)
            {
                updateDbTimer.Interval = writeInterval;
                updateDbTimer.Start();
            }
        }


        private void UpdateDbTimerEvent(object sender, object e)
        {
            updateDbTimer.Stop();
            try
            {
                int count = updatedNodes.Count;
                if (count == 0)
                {
                    updateDbTimer.Start();
                    return;
                }

                Stopwatch sw = new Stopwatch();
                sw.Start();


                WriteUpdatedNodes();

                sw.Stop();
                long elapsed = sw.ElapsedMilliseconds;
                float messagesPerSec = (float)count / (float)elapsed * 1000;
                LogInfo($"Writing nodes: {elapsed} ms ({count} inserts, {(int)messagesPerSec} inserts/sec)");
            }
            catch (Exception ex)
            {

            }

            updateDbTimer.Start();
        }

        private void WriteUpdatedNodes()
        {

            Node[] nodes = new Node[updatedNodes.Count];
            updatedNodes.CopyTo(nodes);
            updatedNodes.Clear();

            List<SerializedNode> serializedNodes = nodes
                .Select(node => new SerializedNode(node)).ToList();


            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE Nodes SET " +
                    "JsonData = @JsonData " +
                    "WHERE Id = @Id";

                db.Execute(sqlQuery, serializedNodes);
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



        public void CreateDb()
        {
            CreateNodesTable();
            CreateLinksTable();
        }

        private void CreateNodesTable()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                try
                {
                    //[Id][uniqueidentifier]NOT NULL,
                    string req =
                        @"CREATE TABLE [dbo].[Nodes](
	                    [Id] [nvarchar](max) NOT NULL,
	                    [JsonData] [nvarchar](max)NOT NULL,       
                        ) ON [PRIMARY] ";

                    db.Query(req);
                }
                catch { }
            }
        }


        private void CreateLinksTable()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                try
                {
                    string req =
                        @"CREATE TABLE [dbo].[Links](
	                    [Id] [nvarchar](max) NOT NULL,
	                    [InputId] [nvarchar](max)NOT NULL,       
	                    [OutputId] [nvarchar](max)NOT NULL,       
	                    [PanelId] [nvarchar](max)NOT NULL,       
                        ) ON [PRIMARY] ";

                    db.Query(req);
                }
                catch { }
            }

        }




        public string AddNode(Node node)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                var sqlQuery = "INSERT INTO Nodes (Id, JsonData) "
                               + "VALUES(@Id, @JsonData)";

                SerializedNode serializedNode = new SerializedNode(node);
                db.Execute(sqlQuery, serializedNode);
            }
            return node.Id;
        }

        public void UpdateNode(Node node)
        {
            if (writeInterval != 0)
            {
                if (!updatedNodes.Contains(node))
                    updatedNodes.Add(node);
                return;
            }

            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE Nodes SET " +
                    "JsonData = @JsonData " +
                    "WHERE Id = @Id";

                SerializedNode serializedNode = new SerializedNode(node);
                db.Execute(sqlQuery, serializedNode);
            }
        }

        public Node GetNode(string id)
        {
            Node node;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                SerializedNode serializedNode = db.Query<SerializedNode>
                    ($"SELECT * FROM Nodes WHERE Id='{id}'").SingleOrDefault();

                node = serializedNode.GetDeserializedNode();
            }

            return node;
        }

        public List<Node> GetAllNodes()
        {
            List<Node> nodes = new List<Node>();
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                List<SerializedNode> serializedNodes = db.Query<SerializedNode>
                    ("SELECT * FROM Nodes").ToList();

                try
                {
                    foreach (var serializedNode in serializedNodes)
                    {
                        nodes.Add(serializedNode.GetDeserializedNode());
                    }
                }
                catch
                {
                    LogError("Can`t deserialize nodes. Database is corrupted or outdated.");
                }
            }

            return nodes;
        }

        public void RemoveNode(string id)
        {
            Node updNode = updatedNodes.FirstOrDefault(x => x.Id == id);
            if (updNode != null)
                updatedNodes.Remove(updNode);

            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query($"DELETE FROM Nodes WHERE Id='{id}'");
            }
        }

        public void RemoveAllNodes()
        {
            updatedNodes.Clear();
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("TRUNCATE TABLE Nodes");
            }
        }






        public string AddLink(Link link)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                var sqlQuery = "INSERT INTO Links (Id, InputId, OutputId, PanelId) "
                               + "VALUES(@Id, @InputId, @OutputId, @PanelId)";

                db.Execute(sqlQuery, link);
            }
            return link.Id;
        }


        public Link GetLink(string id)
        {
            Link link;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                link = db.Query<Link>
                    ($"SELECT * FROM Links WHERE Id='{id}'").SingleOrDefault();

            }
            return link;
        }

        public List<Link> GetAllLinks()
        {
            List<Link> links;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                links = db.Query<Link>("SELECT * FROM Links").ToList();
            }

            return links;
        }

        public void RemoveLink(string id)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query($"DELETE FROM Links WHERE Id='{id}'");
            }
        }

        public void RemoveAllLinks()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("TRUNCATE TABLE Links");
            }
        }



    }
}
