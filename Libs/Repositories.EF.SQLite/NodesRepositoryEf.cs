/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Microsoft.EntityFrameworkCore;
using MyNodes.Nodes;

namespace MyNodes.Repositories.EF.SQLite
{
    public class NodesRepositoryEf : INodesRepository
    {
        //if writeInterval==0, every node will be instantly writing to DB
        //and this will increase the reliability of the system, 
        //but this greatly slows down the performance.
        //If you set writeInterval>0, nodes
        //will be writed to DB with this interval.
        //writeInterval should be large enough (3000 ms is ok)
        private int writeInterval = 5000;

        private Timer updateDbTimer = new Timer();

        private NodesDbContext db;

        private List<Node> updatedNodes = new List<Node>();

        public event LogEventHandler OnLogInfo;
        public event LogEventHandler OnLogError;

        public NodesRepositoryEf(NodesDbContext nodesDbContext)
        {
            updateDbTimer.Elapsed += UpdateDbTimerEvent;

            this.db = nodesDbContext;
            CreateDb();

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
            catch
            {

            }

            updateDbTimer.Start();
        }

        private void WriteUpdatedNodes()
        {

            Node[] nodes = new Node[updatedNodes.Count];
            updatedNodes.CopyTo(nodes);
            updatedNodes.Clear();

            List<SerializedNode> serializedNodes = new List<SerializedNode>();
            foreach (var node in nodes)
            {
                SerializedNode oldNode = db.SerializedNodes.FirstOrDefault(x => x.Id == node.Id);
                if (oldNode == null)
                    continue;

                SerializedNode newNode = new SerializedNode(node);
                oldNode.JsonData = newNode.JsonData;
                serializedNodes.Add(oldNode);
            }


            db.SerializedNodes.UpdateRange(serializedNodes);
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


        public void CreateDb()
        {
            try
            {
                db.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }



        public void AddNode(Node node)
        {

            SerializedNode serializedNode = new SerializedNode(node);
            db.SerializedNodes.Add(serializedNode);

            db.SaveChanges();
        }

        public void AddNodes(List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                SerializedNode serializedNode = new SerializedNode(node);
                db.SerializedNodes.Add(serializedNode);
            }

            db.SaveChanges();
        }

        public void UpdateNode(Node node)
        {
            if (writeInterval != 0)
            {
                if (!updatedNodes.Contains(node))
                    updatedNodes.Add(node);
                return;
            }


            SerializedNode oldNode = db.SerializedNodes.FirstOrDefault(x => x.Id == node.Id);
            if (oldNode == null)
                return;

            SerializedNode newNode = new SerializedNode(node);
            oldNode.JsonData = newNode.JsonData;

            db.SerializedNodes.Update(oldNode);
            db.SaveChanges();
        }

        public Node GetNode(string id)
        {
            SerializedNode serializedNode = db.SerializedNodes.FirstOrDefault(x => x.Id == id);
            if (serializedNode != null)
            {
                Node node = serializedNode.GetDeserializedNode();
                return node;
            }
            return null;
        }

        public List<Node> GetAllNodes()
        {
            List<Node> nodes = new List<Node>();
            List<SerializedNode> serializedNodes = db.SerializedNodes.ToList();

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

            return nodes;
        }

        public void RemoveNode(string id)
        {
            Node updNode = updatedNodes.FirstOrDefault(x => x.Id == id);
            if (updNode != null)
                updatedNodes.Remove(updNode);

            SerializedNode node = db.SerializedNodes.FirstOrDefault(x => x.Id == id);
            if (node == null)
                return;
            db.SerializedNodes.Remove(node);
            db.SaveChanges();
        }

        public void RemoveNodes(List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                Node updNode = updatedNodes.FirstOrDefault(x => x.Id == node.Id);
                if (updNode != null)
                    updatedNodes.Remove(updNode);

                SerializedNode snode = db.SerializedNodes.FirstOrDefault(x => x.Id == node.Id);
                if (snode != null)
                    db.SerializedNodes.Remove(snode);
            }
            db.SaveChanges();
        }

        public void RemoveAllNodes()
        {
            updatedNodes.Clear();
            db.SerializedNodes.RemoveRange(db.SerializedNodes);
            db.SaveChanges();
        }





        public void AddLink(Link link)
        {
            //if (!db.Links.Any(x => x.Id == link.Id))
            db.Links.Add(link);

            db.SaveChanges();
        }

        public void AddLinks(List<Link> links)
        {
            foreach (var link in links)
            {
                db.Links.Add(link);
            }

            db.SaveChanges();
        }

        public Link GetLink(string id)
        {
            return db.Links.FirstOrDefault(x => x.Id == id);
        }

        public List<Link> GetAllLinks()
        {
            return db.Links.ToList();
        }

        public void RemoveLink(string id)
        {
            Link link = db.Links.FirstOrDefault(x => x.Id == id);
            if (link == null)
                return;
            db.Links.Remove(link);
            db.SaveChanges();
        }

        public void RemoveLinks(List<Link> links)
        {
            foreach (var link in links)
            {
                if (db.Links.Any(x => x.Id == link.Id))
                    db.Links.Remove(link);
            }
            //db.Links.RemoveRange(links);
            db.SaveChanges();
        }

        public void RemoveAllLinks()
        {
            db.Links.RemoveRange(db.Links);
            db.SaveChanges();
        }



    }
}
