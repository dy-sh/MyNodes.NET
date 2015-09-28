/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace NodesLinks
{
    public class NodesLinksRepositoryDapper: INodesLinksRepository
    {
        private string connectionString;

        public NodesLinksRepositoryDapper(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public void CreateDb()
        {
            CreateNodesLinksTable();
        }

        private void CreateNodesLinksTable()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                try
                {
                    string req = String.Format(
                        @"CREATE TABLE [dbo].[NodesLinks](
	                    [db_Id] [int] IDENTITY(1,1) NOT NULL,
	                    [inSensorDbId] [int] NULL,       
	                    [inNodeId] [int] NULL,       
	                    [inSensorId] [int] NULL,       
	                    [outSensorDbId] [int] NULL,       
	                    [outNodeId] [int] NULL,       
	                    [outSensorId] [int] NULL 
                        ) ON [PRIMARY] ");

                    db.Query(req);
                }
                catch
                {
                }
            }
        }

        public bool IsDbExist()
        {
            //todo check if db exist
            return true;
        }

        public int AddOrUpdateLink(NodeLink link)
        {
            int db_Id = link.db_Id;

            NodeLink oldLink = GetLink(link.db_Id);

            if (oldLink == null)
                db_Id = AddLink(link);
            else
                UpdateLink(link);

            return db_Id;
        }

        public int AddLink(NodeLink link)
        {
            int db_Id;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                var sqlQuery = "INSERT INTO NodesLinks (inSensorDbId,inNodeId,inSensorId, outSensorDbId, outNodeId, outSensorId) "
                                          + "VALUES(@inSensorDbId,@inNodeId,@inSensorId, @outSensorDbId, @outNodeId, @outSensorId); "
                            + "SELECT CAST(SCOPE_IDENTITY() as int)";

                db_Id = db.Query<int>(sqlQuery, link).Single();
            }
            return db_Id;
        }

        public void UpdateLink(NodeLink link)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE NodesLinks SET " +
                    "inSensorDbId = @inSensorDbId, " +
                    "inNodeId = @inNodeId, " +
                    "inSensorId = @inSensorId, " +
                    "outSensorDbId = @outSensorDbId, " +
                    "outNodeId = @outNodeId, " +
                    "outSensorId = @outSensorId " +
                    "WHERE db_Id = @db_Id";
                db.Execute(sqlQuery, link);
            }
        }

        public NodeLink GetLink(int db_Id)
        {
            NodeLink link;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                link = db.Query<NodeLink>("SELECT * FROM NodesLinks WHERE db_Id=@db_Id",
                    new { db_Id }).SingleOrDefault();
            }

            return link;
        }

        public List<NodeLink> GetIncomingLinks(int nodeId, int sensorId)
        {
            List<NodeLink> links;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                links = db.Query<NodeLink>("SELECT * FROM NodesLinks WHERE inNodeId=@nodeId AND inSensorId=@sensorId",
                    new { nodeId, sensorId }).ToList();
            }

            return links;
        }

        public List<NodeLink> GetOutgoingLinks(int nodeId, int sensorId)
        {
            List<NodeLink> links;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                links = db.Query<NodeLink>("SELECT * FROM NodesLinks WHERE outNodeId=@nodeId AND outSensorId=@sensorId",
                    new { nodeId, sensorId }).ToList();
            }

            return links;
        }

        public List<NodeLink> GetIncomingLinks(int sensorDbId)
        {
            List<NodeLink> links;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                links = db.Query<NodeLink>("SELECT * FROM NodesLinks WHERE inSensorDbId=@sensorDbId",
                    new { sensorDbId }).ToList();
            }

            return links;
        }

        public List<NodeLink> GetOutgoingLinks(int sensorDbId)
        {
            List<NodeLink> links;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                links = db.Query<NodeLink>("SELECT * FROM NodesLinks WHERE outSensorDbId=@sensorDbId",
                    new { sensorDbId }).ToList();
            }

            return links;
        }


        public List<NodeLink> GetAllLinks()
        {
            List<NodeLink> links;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                links = db.Query<NodeLink>("SELECT * FROM NodesLinks").ToList();
            }

            return links;
        }

        public void DeleteLink(int db_Id)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM NodesLinks WHERE db_Id=@db_Id",
                    new { db_Id });
            }
        }

        public void DeleteIncomingLinks(int nodeId, int sensorId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM NodesLinks WHERE inNodeId=@nodeId AND inSensorId=@sensorId",
                    new { nodeId, sensorId });
            }

        }

        public void DeleteOutgoingLinks(int nodeId, int sensorId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM NodesLinks WHERE outNodeId=@nodeId AND outSensorId=@sensorId",
                    new { nodeId, sensorId });
            }

        }

        public void DeleteIncomingLinks(int sensorDbId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM NodesLinks WHERE inSensorDbId=@sensorDbId",
                    new { sensorDbId });
            }

        }

        public void DeleteOutgoingLinks(int sensorDbId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM NodesLinks WHERE outSensorDbId=@sensorDbId",
                    new { sensorDbId });
            }

        }



        public void DropAllLinks()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("TRUNCATE TABLE [NodesLinks]");
            }
        }
    }
}
