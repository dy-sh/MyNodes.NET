using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using MyNetSensors.LogicalNodes;
using Newtonsoft.Json;

namespace MyNetSensors.LogicalNodesRepositoryDappers
{
    public class LogicalNodesRepositoryDapper:ILogicalNodesRepository
    {
        private string connectionString;

        public LogicalNodesRepositoryDapper(string connectionString)
        {
            this.connectionString = connectionString;
            CreateDb();
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
                    string req =
                        @"CREATE TABLE [dbo].[LogicalNodes](
	                    [Id] [uniqueidentifier] NOT NULL,
	                    [JsonData] [nvarchar](max)NOT NULL,       
                        ) ON [PRIMARY] ";

                    db.Query(req);
                }
                catch{}
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
                        @"CREATE TABLE [dbo].[LogicalLinks](
	                    [Id] [uniqueidentifier] NOT NULL,
	                    [InputId] [nvarchar](max)NOT NULL,       
	                    [OutputId] [nvarchar](max)NOT NULL,       
                        ) ON [PRIMARY] ";

                    db.Query(req);
                }
                catch { }
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
            string id;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                var sqlQuery = "INSERT INTO LogicalNodes (Id, JsonData) "
                               + "VALUES(@Id, @JsonData)";

                LogicalNodeSerialized serializedNode = new LogicalNodeSerialized(node);
               db.Execute(sqlQuery, serializedNode);
            }
            return node.Id;
        }

        public void UpdateNode(LogicalNode node)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE LogicalNodes SET " +
                    "JsonData = @JsonData, " +
                    "WHERE Id = @Id";

                LogicalNodeSerialized serializedNode = new LogicalNodeSerialized(node);
                db.Execute(sqlQuery, serializedNode );
            }
        }

        public LogicalNode GetNode(string id)
        {
            LogicalNode node;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                LogicalNodeSerialized serializedNode = db.Query<LogicalNodeSerialized>
                    ("SELECT * FROM LogicalNodes WHERE Id=@Id",id ).SingleOrDefault();

                node = serializedNode.GetDeserializedNode();
            }

            return node;
        }

        public List<LogicalNode> GetAllNodes()
        {
            List<LogicalNode> nodes=new List<LogicalNode>();
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                List<LogicalNodeSerialized> serializedNodes = db.Query<LogicalNodeSerialized>
                    ("SELECT * FROM LogicalNodes").ToList();

                foreach (var serializedNode in serializedNodes)
                {
                    nodes.Add(serializedNode.GetDeserializedNode());
                }
            }

            return nodes;
        }

        public void DeleteNode(string id)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM LogicalNodes WHERE Id=@Id", id );
            }
        }

        public void DropNodes()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("TRUNCATE TABLE LogicalNodes");
            }
        }

        public string AddOrUpdateLink(LogicalLink link)
        {
            throw new NotImplementedException();
        }

        public string AddLink(LogicalLink link)
        {
            throw new NotImplementedException();
        }

        public void UpdateLink(LogicalLink link)
        {
            throw new NotImplementedException();
        }

        public LogicalLink GetLink(string id)
        {
            throw new NotImplementedException();
        }

        public List<LogicalLink> GetAllLinks()
        {
            throw new NotImplementedException();
        }

        public void DeleteLink(string id)
        {
            throw new NotImplementedException();
        }

        public void DropLinks()
        {
            throw new NotImplementedException();
        }







    }
}
