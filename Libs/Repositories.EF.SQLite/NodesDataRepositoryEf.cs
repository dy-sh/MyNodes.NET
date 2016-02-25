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
        private List<NodeData> cachedData = new List<NodeData>();
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
                int count = cachedData.Count;
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

            }

            updateDbTimer.Start();
        }


        private int WriteCached()
        {
            int inserts = 0;

            List<NodeData> data = cachedData;
            cachedData = new List<NodeData>();

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

            db.SaveChanges();
            return inserts;
        }

        public void AddNodeData(NodeData data, int? maxDbRecords)
        {
            if (writeInterval != 0)
            {
                maxRecords[data.NodeId] = maxDbRecords;
                cachedData.Add(data);
                return;
            }

            db.NodesData.Add(data);

            //remove extra data
            if (maxDbRecords != null)
            {
                int count = db.NodesData.Count(x => x.NodeId == data.NodeId);
                int more = count - maxDbRecords.Value + 1;
                if (more > 0)
                {
                    List<NodeData> removeList = db.NodesData
                        .Where(x => x.NodeId == data.NodeId)
                        .OrderBy(x => x.DateTime)
                        .Take(more)
                        .ToList();
                    db.NodesData.RemoveRange(removeList);
                }
            }

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
            cachedData.RemoveAll(x => x.NodeId == nodeId);

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
            cachedData.Clear();

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