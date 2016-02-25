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
using Microsoft.Data.Entity;
using MyNetSensors.Gateways;
using MyNetSensors.Nodes;

namespace MyNetSensors.Repositories.EF.SQLite
{
    public class NodesStatesRepositoryEf : INodesStatesRepository
    {
        private int writeInterval = 5000;
        private Timer updateDbTimer = new Timer();
        private List<NodeState> cachedStates = new List<NodeState>();
        private Dictionary<string, int> maxStates = new Dictionary<string, int>(); //nodeId,maxStates

        public event LogEventHandler OnLogInfo;
        public event LogEventHandler OnLogError;

        private NodesStatesHistoryDbContext historyDb;

        public NodesStatesRepositoryEf(NodesStatesHistoryDbContext nodesStatesHistoryDbContext)
        {
            this.historyDb = nodesStatesHistoryDbContext;
            CreateDb();

            updateDbTimer.Elapsed += UpdateDbTimerEvent;
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
                historyDb.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
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


                int inserts = WriteCached();

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


        private int WriteCached()
        {
            int inserts=0;

            List<NodeState> states = cachedStates;
            cachedStates = new List<NodeState>();

            while (states.Any())
            {
                string nodeId = states.First().NodeId;
                List<NodeState> statesForNode = states.Where(x => x.NodeId == nodeId).ToList();
                states.RemoveAll(x => x.NodeId == nodeId);

                int dbCount = historyDb.NodesStates.Count(x => x.NodeId == nodeId);

                int allCount = dbCount + statesForNode.Count;
                int max = maxStates[nodeId];
                int more = allCount - max;
                if (more > 0)
                {
                    int removeFromDb = allCount - max;
                    if (removeFromDb > 0)
                    {
                        List<NodeState> removeList = historyDb.NodesStates
                            .Where(x => x.NodeId == nodeId)
                            .OrderBy(x=>x.DateTime)
                            .Take(more)
                            .ToList();
                        historyDb.NodesStates.RemoveRange(removeList);
                    }
      
                    int removeFromCached = statesForNode.Count - max;
                    if (removeFromCached > 0)
                        statesForNode.RemoveRange(0, removeFromCached);
                }

                historyDb.NodesStates.AddRange(statesForNode);
                inserts += statesForNode.Count;

                historyDb.SaveChanges();
            }
            return inserts;
        }

        public void AddState(NodeState state, int maxStatesCount)
        {
            if (writeInterval != 0)
            {
                maxStates[state.NodeId] = maxStatesCount;
                cachedStates.Add(state);
                return;
            }

            historyDb.NodesStates.Add(state);

            int count = historyDb.NodesStates.Count(x => x.NodeId == state.NodeId);

            int more = count - maxStatesCount + 1;

            if (more > 0)
            {
                List<NodeState> removeList = historyDb.NodesStates
                            .Where(x => x.NodeId == state.NodeId)
                            .OrderBy(x=>x.DateTime)
                            .Take(more)
                            .ToList();
                historyDb.NodesStates.RemoveRange(removeList);
            }

            historyDb.SaveChanges();
        }

        public List<NodeState> GetStatesForNode(string nodeId)
        {
            return historyDb.NodesStates.Where(x => x.NodeId == nodeId).ToList();
        }

        public void RemoveStatesForNode(string nodeId)
        {
            cachedStates.RemoveAll(x => x.NodeId == nodeId);

            historyDb.RemoveRange(historyDb.NodesStates.Where(x => x.NodeId == nodeId));
            historyDb.SaveChanges();
        }

        public void RemoveAllStates()
        {
            cachedStates.Clear();

            historyDb.NodesStates.RemoveRange(historyDb.NodesStates);
            historyDb.SaveChanges();
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