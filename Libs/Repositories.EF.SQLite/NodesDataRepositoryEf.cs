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
    public class NodesDataRepositoryEf : INodesDataRepository
    {
        private int writeInterval = 5000;
        private Timer updateDbTimer = new Timer();
        private List<NodeData> cachedNewData = new List<NodeData>();
        private List<NodeData> cachedUpdatedData = new List<NodeData>();
        private Dictionary<string, int?> maxRecords = new Dictionary<string, int?>(); //nodeId,maxRecords

        public event LogEventHandler OnLogInfo;
        public event LogEventHandler OnLogError;

        private NodesDataDbContext db;

        public NodesDataRepositoryEf(NodesDataDbContext nodesDataDbContext)
        {
            this.db = nodesDataDbContext;
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
                db.Database.EnsureCreated();
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
                int count = cachedNewData.Count + cachedUpdatedData.Count;
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
                LogInfo($"Writing nodes data: {elapsed} ms ({inserts} inserts, {(int)messagesPerSec} inserts/sec)");
            }
            catch (Exception ex)
            {
                LogError("Failed to write node data. " + ex.Message);
            }

            updateDbTimer.Start();
        }


        private int WriteCached()
        {
            int inserts = 0;

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
                        int dbCount = db.NodesData.Count(x => x.NodeId == nodeId);

                        int allCount = dbCount + dataForNode.Count;
                        int max = maxRecords[nodeId].Value;
                        int more = allCount - max;
                        if (more > 0)
                        {
                            int removeFromDb = allCount - max;
                            if (removeFromDb > 0)
                            {
                                List<NodeData> removeList = db.NodesData
                                    .Where(x => x.NodeId == nodeId)
                                    .OrderBy(x => x.DateTime)
                                    .Take(more)
                                    .ToList();
                                db.NodesData.RemoveRange(removeList);
                            }

                            int removeFromCached = dataForNode.Count - max;
                            if (removeFromCached > 0)
                                dataForNode.RemoveRange(0, removeFromCached);
                        }
                    }

                    db.NodesData.AddRange(dataForNode);
                    inserts += dataForNode.Count;
                }
            }

            //-------WRITE UPDATED---------
            if (cachedUpdatedData.Count > 0)
            {
                List<NodeData> updated = cachedUpdatedData;
                cachedUpdatedData = new List<NodeData>();
                inserts += updated.Count;

                foreach (var nodeData in updated)
                {
                    NodeData old = db.NodesData.FirstOrDefault(x => x.Id == nodeData.Id);
                    old.Value = nodeData.Value;
                    old.DateTime = nodeData.DateTime;
                }
            }

            db.SaveChanges();
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
            db.NodesData.Add(nodeData);

            //remove extra data
            if (maxDbRecords != null)
            {
                int count = db.NodesData.Count(x => x.NodeId == nodeData.NodeId);
                int more = count - maxDbRecords.Value + 1;
                if (more > 0)
                {
                    List<NodeData> removeList = db.NodesData
                        .Where(x => x.NodeId == nodeData.NodeId)
                        .OrderBy(x => x.DateTime)
                        .Take(more)
                        .ToList();
                    db.NodesData.RemoveRange(removeList);
                }
            }

            db.SaveChanges();
            return nodeData.Id;
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
            NodeData old = GetNodeData(nodeData.Id);
            if (old == null)
            {
                LogError("Cant update node data. Does not exist.");
                return;
            }

            old.Value = nodeData.Value;
            old.DateTime = nodeData.DateTime;
            //db.Update(old);
            db.SaveChanges();
        }


        public List<NodeData> GetAllNodeDataForNode(string nodeId)
        {
            return db.NodesData.Where(x => x.NodeId == nodeId).ToList();
        }

        public NodeData GetNodeData(int id)
        {
            return db.NodesData.FirstOrDefault(x => x.Id == id);
        }

        public void RemoveAllNodeDataForNode(string nodeId)
        {
            cachedNewData.RemoveAll(x => x.NodeId == nodeId);
            cachedUpdatedData.RemoveAll(x => x.NodeId == nodeId);

            db.RemoveRange(db.NodesData.Where(x => x.NodeId == nodeId));
            db.SaveChanges();
        }

        public void RemoveNodeData(int id)
        {
            NodeData nodeData = GetNodeData(id);

            if (nodeData != null)
                db.Remove(nodeData);

            db.SaveChanges();
        }

        public void RemoveAllNodesData()
        {
            cachedNewData.Clear();
            cachedUpdatedData.Clear();

            db.NodesData.RemoveRange(db.NodesData);
            db.SaveChanges();
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