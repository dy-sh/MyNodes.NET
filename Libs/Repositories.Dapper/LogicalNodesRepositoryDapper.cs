using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using MyNetSensors.LogicalNodes;

namespace MyNetSensors.Repositories.Dapper
{
    public class LogicalNodesRepositoryDapper : ILogicalNodesRepository
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
                    //[Id][uniqueidentifier]NOT NULL,
                    string req =
                        @"CREATE TABLE [dbo].[LogicalNodes](
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
                        @"CREATE TABLE [dbo].[LogicalLinks](
	                    [Id] [nvarchar](max) NOT NULL,
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
                    "JsonData = @JsonData " +
                    "WHERE Id = @Id";

                LogicalNodeSerialized serializedNode = new LogicalNodeSerialized(node);
                db.Execute(sqlQuery, serializedNode);
            }
        }

        public LogicalNode GetNode(string id)
        {
            LogicalNode node;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                LogicalNodeSerialized serializedNode = db.Query<LogicalNodeSerialized>
                    ($"SELECT * FROM LogicalNodes WHERE Id='{id}'").SingleOrDefault();

                node = serializedNode.GetDeserializedNode();
            }

            return node;
        }

        public List<LogicalNode> GetAllNodes()
        {
            List<LogicalNode> nodes = new List<LogicalNode>();
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

        public void RemoveNode(string id)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query($"DELETE FROM LogicalNodes WHERE Id='{id}'");
            }
        }

        public void RemoveAllNodes()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("TRUNCATE TABLE LogicalNodes");
            }
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
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                var sqlQuery = "INSERT INTO LogicalLinks (Id, InputId, OutputId) "
                               + "VALUES(@Id, @InputId, @OutputId)";

                db.Execute(sqlQuery, link);
            }
            return link.Id;
        }

        public void UpdateLink(LogicalLink link)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE LogicalLinks SET " +
                    "InputId = @InputId, OutputId = @OutputId " +
                    "WHERE Id = @Id";

                db.Execute(sqlQuery, link);
            }
        }

        public LogicalLink GetLink(string id)
        {
            LogicalLink link;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                link = db.Query<LogicalLink>
                    ($"SELECT * FROM LogicalLinks WHERE Id='{id}'").SingleOrDefault();

            }
            return link;
        }

        public List<LogicalLink> GetAllLinks()
        {
            List<LogicalLink> links;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                links = db.Query<LogicalLink>("SELECT * FROM LogicalLinks").ToList();
            }

            return links;
        }

        public void RemoveLink(string id)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query($"DELETE FROM LogicalLinks WHERE Id='{id}'");
            }
        }

        public void RemoveAllLinks()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("TRUNCATE TABLE LogicalLinks");
            }
        }



    }
}
