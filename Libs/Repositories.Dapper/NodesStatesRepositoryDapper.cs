/*  MyNetSensors 
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
using MyNetSensors.Gateways;
using MyNetSensors.Nodes;

namespace MyNetSensors.Repositories.Dapper
{
    public class NodesStatesRepositoryDapper : INodesStatesRepository
    {

        private string connectionString;

        private int writeInterval = 5000;
        private Timer updateDbTimer = new Timer();
        private List<NodeState> cachedStates = new List<NodeState>();
        private Dictionary<string, int?> maxStates = new Dictionary<string, int?>(); //nodeId,maxStates

        public event LogEventHandler OnLogInfo;
        public event LogEventHandler OnLogError;

        public NodesStatesRepositoryDapper(string connectionString)
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
                        @"CREATE TABLE [dbo].[NodesStates](
	                    [Id] [int] IDENTITY(1,1) NOT NULL,
	                    [NodeId] [nvarchar](max) NULL,
	                    [State] [nvarchar](max) NULL,
	                    [DateTime] [datetime] NOT NULL ) ON [PRIMARY] ");
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void UpdateDbTimerEvent(object sender, object e)
        {
            updateDbTimer.Stop();
            try
            {
                int count = cachedStates.Count;
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
                LogInfo($"Writing nodes states: {elapsed} ms ({inserts} inserts, {(int)messagesPerSec} inserts/sec)");
            }
            catch (Exception ex)
            {

            }

            updateDbTimer.Start();
        }


        private async Task<int> WriteCached()
        {
            int inserts = 0;

            List<NodeState> states = cachedStates;
            cachedStates = new List<NodeState>();

            List<NodeState> statesToWrite = new List<NodeState>();

            using (var db = new SqlConnection(connectionString))
            {
                while (states.Any())
                {
                    string nodeId = states.First().NodeId;
                    List<NodeState> statesForNode = states.Where(x => x.NodeId == nodeId).ToList();
                    states.RemoveAll(x => x.NodeId == nodeId);

                    //remove extra data
                    if (maxStates[nodeId] != null)
                    {
                        int dbCount =
                            db.Query<int>("SELECT COUNT(*) FROM [NodesStates] WHERE NodeId=@nodeId", new {nodeId})
                                .Single();

                        int allCount = dbCount + statesForNode.Count;
                        int max = maxStates[nodeId].Value;
                        int more = allCount - max;
                        if (more > 0)
                        {
                            int removeFromDb = allCount - max;
                            if (removeFromDb > 0)
                                db.Query(
                                    $"DELETE FROM [NodesStates] WHERE Id IN (SELECT TOP {more} Id FROM [NodesStates] WHERE NodeId=@nodeId ORDER BY DateTime ASC);",
                                    new {nodeId});

                            int removeFromCached = statesForNode.Count - max;
                            if (removeFromCached > 0)
                                statesForNode.RemoveRange(0, removeFromCached);

                        }
                    }
                    statesToWrite.AddRange(statesForNode);
                }

                var sqlQuery = "INSERT INTO [NodesStates] (NodeId, State, DateTime) "
                    + "VALUES(@NodeId, @State, @DateTime)";

                await db.ExecuteAsync(sqlQuery, statesToWrite);
                inserts += statesToWrite.Count;
            }

            return inserts;
        }

        public void AddState(NodeState state, int? maxStatesCount)
        {
            if (writeInterval != 0)
            {
                maxStates[state.NodeId] = maxStatesCount;
                cachedStates.Add(state);
                return;
            }

            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery = "INSERT INTO [NodesStates] (NodeId, State, DateTime) "
                               +
                               "VALUES(@NodeId, @State, @DateTime); "
                               + "SELECT CAST(SCOPE_IDENTITY() as int)";
                db.Query(sqlQuery, state);

                //remove extra data
                if (maxStatesCount != null)
                {
                    int count =
                        db.Query<int>("SELECT COUNT(*) FROM [NodesStates] WHERE NodeId=@nodeId", new {state.NodeId})
                            .Single();

                    int more = count - maxStatesCount.Value + 1;

                    if (more > 0)
                        db.Query(
                            $"DELETE FROM [NodesStates] WHERE Id IN (SELECT TOP {more} Id FROM [NodesStates] WHERE NodeId=@nodeId ORDER BY DateTime ASC);",
                            new {state.NodeId});
                }
            }
        }



        public List<NodeState> GetStatesForNode(string nodeId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                try
                {
                    string req = $"SELECT * FROM [NodesStates] WHERE NodeId=@nodeId";
                    return db.Query<NodeState>(req, new { nodeId }).ToList();
                }
                catch { }

                return null;
            }
        }

        public void RemoveStatesForNode(string nodeId)
        {
            cachedStates.RemoveAll(x => x.NodeId == nodeId);

            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                db.Query($"DELETE FROM [NodesStates] WHERE NodeId=@nodeId", new { nodeId });
            }
        }

        public void RemoveAllStates()
        {
            cachedStates.Clear();

            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                db.Query("TRUNCATE TABLE [NodesStates]");
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