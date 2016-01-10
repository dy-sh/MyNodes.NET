using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Data.Entity;
using MyNetSensors.LogicalNodes;

namespace MyNetSensors.Repositories.EF.SQLite
{
    public class LogicalNodesRepositoryEF : ILogicalNodesRepository
    {
        private LogicalNodesDbContext db;

        public LogicalNodesRepositoryEF(LogicalNodesDbContext nodesDbContext)
        {
            this.db = nodesDbContext;
            CreateDb();
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



        public bool IsDbExist()
        {
            //todo check if db exist
            return true;
        }

        public string AddOrUpdateNode(LogicalNode node)
        {
            string id = node.Id;

            LogicalNode oldLink = GetNode(node.Id);

            if (oldLink == null)
                id = AddNode(node);
            else
                UpdateNode(node);

            return id;
        }

        public string AddNode(LogicalNode node)
        {

            LogicalNodeSerialized serializedNode = new LogicalNodeSerialized(node);
            db.LogicalNodesSerialized.Add(serializedNode);
            db.SaveChanges();

            return serializedNode.Id;
        }

        public void UpdateNode(LogicalNode node)
        {
            LogicalNodeSerialized oldNode = db.LogicalNodesSerialized.FirstOrDefault(x => x.Id == node.Id);
            if (oldNode == null)
                return;

            LogicalNodeSerialized newNode = new LogicalNodeSerialized(node);
            oldNode.JsonData = newNode.JsonData;

            db.LogicalNodesSerialized.Update(oldNode);
            db.SaveChanges();
        }

        public LogicalNode GetNode(string id)
        {
            LogicalNodeSerialized serializedNode = db.LogicalNodesSerialized.FirstOrDefault(x=>x.Id==id);
            if (serializedNode != null)
            {
                LogicalNode node = serializedNode.GetDeserializedNode();
                return node;
            }
            return null;
        }

        public List<LogicalNode> GetAllNodes()
        {
            List<LogicalNode> nodes = new List<LogicalNode>();
            List<LogicalNodeSerialized> serializedNodes = db.LogicalNodesSerialized.ToList();
            foreach (var serializedNode in serializedNodes)
            {
                nodes.Add(serializedNode.GetDeserializedNode());
            }

            return nodes;
        }

        public void DeleteNode(string id)
        {
            LogicalNodeSerialized node = db.LogicalNodesSerialized.FirstOrDefault(x => x.Id == id);
            if (node==null)
                return;
            db.LogicalNodesSerialized.Remove(node);
            db.SaveChanges();
        }

        public void DropNodes()
        {
            db.LogicalNodesSerialized.RemoveRange(db.LogicalNodesSerialized);
            db.SaveChanges();
        }





        public string AddOrUpdateLink(LogicalLink link)
        {
            string id = link.Id;

            LogicalLink oldLink = GetLink(link.Id);

            if (oldLink == null)
                id = AddLink(link);
            else
                UpdateLink(link);

            return id;
        }

        public string AddLink(LogicalLink link)
        {
            db.LogicalLinks.Add(link);
            db.SaveChanges();

            return link.Id;
        }

        public void UpdateLink(LogicalLink link)
        {
            db.LogicalLinks.Update(link);
            db.SaveChanges();
        }

        public LogicalLink GetLink(string id)
        {
            return db.LogicalLinks.FirstOrDefault(x => x.Id == id);
        }

        public List<LogicalLink> GetAllLinks()
        {
            return db.LogicalLinks.ToList();
        }

        public void DeleteLink(string id)
        {
            LogicalLink link = db.LogicalLinks.FirstOrDefault(x => x.Id == id);
            if (link == null)
                return;
            db.LogicalLinks.Remove(link);
            db.SaveChanges();
        }

        public void DropLinks()
        {
            db.LogicalLinks.RemoveRange(db.LogicalLinks);
            db.SaveChanges();
        }



    }
}
