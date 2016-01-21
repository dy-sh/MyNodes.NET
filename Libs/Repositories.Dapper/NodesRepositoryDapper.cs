using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using MyNetSensors.Nodes;

namespace MyNetSensors.Repositories.Dapper
{
    public class NodesRepositoryDapper : INodesRepository
    {
        private string connectionString;

        public NodesRepositoryDapper(string connectionString)
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
                        @"CREATE TABLE [dbo].[Nodes](
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
                        @"CREATE TABLE [dbo].[Links](
	                    [Id] [nvarchar](max) NOT NULL,
	                    [InputId] [nvarchar](max)NOT NULL,       
	                    [OutputId] [nvarchar](max)NOT NULL,       
	                    [PanelId] [nvarchar](max)NOT NULL,       
                        ) ON [PRIMARY] ";

                    db.Query(req);
                }
                catch { }
            }

        }


        

        public string AddNode(Node node)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                var sqlQuery = "INSERT INTO Nodes (Id, JsonData) "
                               + "VALUES(@Id, @JsonData)";

                SerializedNode serializedNode = new SerializedNode(node);
                db.Execute(sqlQuery, serializedNode);
            }
            return node.Id;
        }

        public void UpdateNode(Node node)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE Nodes SET " +
                    "JsonData = @JsonData " +
                    "WHERE Id = @Id";

                SerializedNode serializedNode = new SerializedNode(node);
                db.Execute(sqlQuery, serializedNode);
            }
        }

        public Node GetNode(string id)
        {
            Node node;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                SerializedNode serializedNode = db.Query<SerializedNode>
                    ($"SELECT * FROM Nodes WHERE Id='{id}'").SingleOrDefault();

                node = serializedNode.GetDeserializedNode();
            }

            return node;
        }

        public List<Node> GetAllNodes()
        {
            List<Node> nodes = new List<Node>();
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                List<SerializedNode> serializedNodes = db.Query<SerializedNode>
                    ("SELECT * FROM Nodes").ToList();

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
                db.Query($"DELETE FROM Nodes WHERE Id='{id}'");
            }
        }

        public void RemoveAllNodes()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("TRUNCATE TABLE Nodes");
            }
        }






        public string AddLink(Link link)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                var sqlQuery = "INSERT INTO Links (Id, InputId, OutputId, PanelId) "
                               + "VALUES(@Id, @InputId, @OutputId, @PanelId)";

                db.Execute(sqlQuery, link);
            }
            return link.Id;
        }


        public Link GetLink(string id)
        {
            Link link;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                link = db.Query<Link>
                    ($"SELECT * FROM Links WHERE Id='{id}'").SingleOrDefault();

            }
            return link;
        }

        public List<Link> GetAllLinks()
        {
            List<Link> links;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                links = db.Query<Link>("SELECT * FROM Links").ToList();
            }

            return links;
        }

        public void RemoveLink(string id)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query($"DELETE FROM Links WHERE Id='{id}'");
            }
        }

        public void RemoveAllLinks()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("TRUNCATE TABLE Links");
            }
        }



    }
}
