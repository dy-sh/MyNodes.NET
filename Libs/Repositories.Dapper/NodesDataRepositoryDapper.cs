/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Dapper;
using MyNodes.Gateways;
using MyNodes.Nodes;

namespace MyNodes.Repositories.Dapper
{
    public class NodesDataRepositoryDapper : INodesDataRepository
    {

        private string connectionString;

        private int writeInterval = 5000;
        private Timer updateDbTimer = new Timer();
        private List<NodeData> cachedNewData = new List<NodeData>();
        private List<NodeData> cachedUpdatedData = new List<NodeData>();
        private Dictionary<string, int?> maxRecords = new Dictionary<string, int?>(); //nodeId,maxRecords

        public event LogEventHandler OnLogInfo;
        public event LogEventHandler OnLogError;

        public NodesDataRepositoryDapper(string connectionString)
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

        private void CreateDb()
        {
            using (var db = new SqlConnection(connectionString + ";Database= master"))
            {

                try
                {
                    //db = new SqlConnection("Data Source=.\\sqlexpress; Database= master; Integrated Security=True;");
                    db.Open();
                    db.Execute("CREATE DATABASE [MyNodes]");
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
                        @"CREATE TABLE [dbo].[NodesData](
	                    [Id] [int] IDENTITY(1,1) NOT NULL,
	                    [NodeId] [nvarchar](max) NULL,
                        [DateTime] [datetime] NOT NULL,
	                    [Value] [nvarchar](max) NULL 
                        ) ON [PRIMARY] ");
                }
                catch
                {
                }
            }
        }

        private void UpdateDbTimerEvent(object sender, object e)
        {
            updateDbTimer.Stop();
            try
            {
                int count = cachedNewData.Count + cachedUpdatedData.Count;
                if (count == 0)
                {
                    updateDbTimer.Start();
                    return;
                }

                Stopwatch sw = new Stopwatch();
                sw.Start();


                int inserts = WriteCached().Result;

                sw.Stop();
                long elapsed = sw.ElapsedMilliseconds;
                float messagesPerSec = (float)inserts / (float)elapsed * 1000;
                LogInfo($"Writing nodes data: {elapsed} ms ({inserts} inserts, {(int)messagesPerSec} inserts/sec)");
            }
            catch (Exception ex)
            {
                LogError("Failed to write node data. "+ex.Message);
            }

            updateDbTimer.Start();
        }


        private async Task<int> WriteCached()
        {
            int inserts = 0;

            List<NodeData> dataToWrite = new List<NodeData>();

            using (var db = new SqlConnection(connectionString))
            {
                //-------WRITE NEW---------
                if (cachedNewData.Count > 0)
                {
                    List<NodeData> data = cachedNewData;
                    cachedNewData = new List<NodeData>();

                    while (data.Any())
                    {
                        string nodeId = data.First().NodeId;
                        List<NodeData> dataForNode = data.Where(x => x.NodeId == nodeId).ToList();
                        data.RemoveAll(x => x.NodeId == nodeId);

                        //remove extra data
                        if (maxRecords[nodeId] != null)
                        {
                            int dbCount =
                                db.Query<int>("SELECT COUNT(*) FROM [NodesData] WHERE NodeId=@nodeId", new {nodeId})
                                    .Single();

                            int allCount = dbCount + dataForNode.Count;
                            int max = maxRecords[nodeId].Value;
                            int more = allCount - max;
                            if (more > 0)
                            {
                                int removeFromDb = allCount - max;
                                if (removeFromDb > 0)
                                    db.Query(
                                        $"DELETE FROM [NodesData] WHERE Id IN (SELECT TOP {more} Id FROM [NodesData] WHERE NodeId=@nodeId ORDER BY DateTime ASC);",
                                        new {nodeId});

                                int removeFromCached = dataForNode.Count - max;
                                if (removeFromCached > 0)
                                    dataForNode.RemoveRange(0, removeFromCached);

                            }
                        }
                        dataToWrite.AddRange(dataForNode);
                    }

                    var sqlQuery = "INSERT INTO [NodesData] (NodeId, DateTime, Value) "
                                   + "VALUES(@NodeId, @DateTime, @Value)";

                    await db.ExecuteAsync(sqlQuery, dataToWrite);
                    inserts += dataToWrite.Count;
                }

                //-------WRITE UPDATED---------
                if (cachedUpdatedData.Count > 0)
                {
                    List<NodeData> updated = cachedUpdatedData;
                    cachedUpdatedData = new List<NodeData>();
                    inserts += updated.Count;

                    var sqlQuery =
                        "UPDATE [NodesData] SET " +
                        "Value = @Value, " +
                        "DateTime = @DateTime " +
                        "WHERE Id = @Id";

                    db.Execute(sqlQuery, updated);
                }
            }

            return inserts;
        }

        public void AddNodeData(NodeData nodeData, int? maxDbRecords)
        {
            if (writeInterval != 0)
            {
                maxRecords[nodeData.NodeId] = maxDbRecords;
                cachedNewData.Add(nodeData);
                return;
            }

            AddNodeDataImmediately(nodeData, maxDbRecords);
        }

        public int AddNodeDataImmediately(NodeData nodeData, int? maxDbRecords = null)
        {
            int id = -1;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery = "INSERT INTO [NodesData] (NodeId, DateTime, Value) "
                               +
                               "VALUES(@NodeId, @DateTime, @Value); "
                               + "SELECT CAST(SCOPE_IDENTITY() as int)";
                id = db.Query<int>(sqlQuery, nodeData).Single();

                //remove extra data
                if (maxDbRecords != null)
                {
                    int count =
                        db.Query<int>("SELECT COUNT(*) FROM [NodesData] WHERE NodeId=@nodeId", new { nodeData.NodeId })
                            .Single();

                    int more = count - maxDbRecords.Value + 1;

                    if (more > 0)
                        db.Query(
                            $"DELETE FROM [NodesData] WHERE Id IN (SELECT TOP {more} Id FROM [NodesData] WHERE NodeId=@nodeId ORDER BY DateTime ASC);",
                            new { nodeData.NodeId });
                }
            }
            return id;
        }

        public void UpdateNodeData(NodeData nodeData)
        {
            if (writeInterval != 0)
            {
                cachedUpdatedData.RemoveAll(x => x.Id == nodeData.Id);
                cachedUpdatedData.Add(nodeData);
                return;
            }

            UpdateNodeDataImmediately(nodeData);
        }

        public void UpdateNodeDataImmediately(NodeData nodeData)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE [NodesData] SET " +
                    "Value = @Value, " +
                    "DateTime = @DateTime " +
                    "WHERE Id = @Id";

                db.Execute(sqlQuery, nodeData);
            }
        }


        public List<NodeData> GetAllNodeDataForNode(string nodeId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                try
                {
                    string req = $"SELECT * FROM [NodesData] WHERE NodeId=@nodeId";
                    return db.Query<NodeData>(req, new { nodeId }).ToList();
                }
                catch { }

                return null;
            }
        }

        public NodeData GetNodeData(int id)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                try
                {
                    return db.Query<NodeData>("SELECT * FROM [NodesData] WHERE Id=@id", new { id })
                        .FirstOrDefault();
                }
                catch { }

                return null;
            }
        }

        public void RemoveAllNodeDataForNode(string nodeId)
        {
            cachedNewData.RemoveAll(x => x.NodeId == nodeId);
            cachedUpdatedData.RemoveAll(x => x.NodeId == nodeId);

            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                db.Query($"DELETE FROM [NodesData] WHERE NodeId=@nodeId", new { nodeId });
            }
        }

        public void RemoveNodeData(int id)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                db.Query($"DELETE FROM [NodesData] WHERE Id=@id", new { id });
            }
        }

        public void RemoveAllNodesData()
        {
            cachedNewData.Clear();
            cachedUpdatedData.Clear();

            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                db.Query("TRUNCATE TABLE [NodesData]");
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
    }
}