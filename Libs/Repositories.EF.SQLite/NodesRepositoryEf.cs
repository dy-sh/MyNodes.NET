using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Data.Entity;
using MyNetSensors.Nodes;

namespace MyNetSensors.Repositories.EF.SQLite
{
    public class NodesRepositoryEf : INodesRepository
    {
        private NodesDbContext db;

        public NodesRepositoryEf(NodesDbContext nodesDbContext)
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

        

        public string AddNode(Node node)
        {

            SerializedNode serializedNode = new SerializedNode(node);
            db.SerializedNodes.Add(serializedNode);
            db.SaveChanges();

            return serializedNode.Id;
        }

        public void UpdateNode(Node node)
        {
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
            SerializedNode serializedNode = db.SerializedNodes.FirstOrDefault(x=>x.Id==id);
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
            foreach (var serializedNode in serializedNodes)
            {
                nodes.Add(serializedNode.GetDeserializedNode());
            }

            return nodes;
        }

        public void RemoveNode(string id)
        {
            SerializedNode node = db.SerializedNodes.FirstOrDefault(x => x.Id == id);
            if (node==null)
                return;
            db.SerializedNodes.Remove(node);
            db.SaveChanges();
        }

        public void RemoveAllNodes()
        {
            db.SerializedNodes.RemoveRange(db.SerializedNodes);
            db.SaveChanges();
        }




        
        public string AddLink(Link link)
        {
            db.Links.Add(link);
            db.SaveChanges();

            return link.Id;
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

        public void RemoveAllLinks()
        {
            db.Links.RemoveRange(db.Links);
            db.SaveChanges();
        }



    }
}
