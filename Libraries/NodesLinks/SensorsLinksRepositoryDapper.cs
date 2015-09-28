/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace MyNetSensors.NodesLinks
{
    public class SensorsLinksRepositoryDapper: ISensorsLinksRepository
    {
        private string connectionString;

        public SensorsLinksRepositoryDapper(string connectionString)
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
	                    [fromSensorDbId] [int] NULL,       
	                    [fromNodeId] [int] NULL,       
	                    [fromSensorId] [int] NULL,       
	                    [fromDataType] [int] NULL,       
	                    [toSensorDbId] [int] NULL,       
	                    [toNodeId] [int] NULL,       
	                    [toSensorId] [int] NULL,
	                    [toDataType] [int] NULL 
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

        public int AddOrUpdateLink(SensorLink link)
        {
            int db_Id = link.db_Id;

            SensorLink oldLink = GetLink(link.db_Id);

            if (oldLink == null)
                db_Id = AddLink(link);
            else
                UpdateLink(link);

            return db_Id;
        }

        public int AddLink(SensorLink link)
        {
            int db_Id;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                var sqlQuery = "INSERT INTO NodesLinks (fromSensorDbId,fromNodeId,fromSensorId,fromDataType, toSensorDbId, toNodeId, toSensorId,toDataType) "
                                          + "VALUES(@fromSensorDbId,@fromNodeId,@fromSensorId,@fromDataType, @toSensorDbId, @toNodeId, @toSensorId,@toDataType); "
                            + "SELECT CAST(SCOPE_IDENTITY() as int)";

                db_Id = db.Query<int>(sqlQuery, link).Single();
            }
            return db_Id;
        }

        public void UpdateLink(SensorLink link)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE NodesLinks SET " +
                    "fromSensorDbId = @fromSensorDbId, " +
                    "fromNodeId = @fromNodeId, " +
                    "fromSensorId = @fromSensorId, " +
                    "fromDataType = @fromDataType, " +
                    "toSensorDbId = @toSensorDbId, " +
                    "toNodeId = @toNodeId, " +
                    "toSensorId = @toSensorId " +
                    "toDataType = @toDataType " +
                    "WHERE db_Id = @db_Id";
                db.Execute(sqlQuery, link);
            }
        }

        public SensorLink GetLink(int db_Id)
        {
            SensorLink link;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                link = db.Query<SensorLink>("SELECT * FROM NodesLinks WHERE db_Id=@db_Id",
                    new { db_Id }).SingleOrDefault();
            }

            return link;
        }

        public List<SensorLink> GetLinksFrom(int nodeId, int sensorId)
        {
            List<SensorLink> links;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                links = db.Query<SensorLink>("SELECT * FROM NodesLinks WHERE fromNodeId=@nodeId AND fromSensorId=@sensorId",
                    new { nodeId, sensorId }).ToList();
            }

            return links;
        }

        public List<SensorLink> GetLinksTo(int nodeId, int sensorId)
        {
            List<SensorLink> links;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                links = db.Query<SensorLink>("SELECT * FROM NodesLinks WHERE toNodeId=@nodeId AND toSensorId=@sensorId",
                    new { nodeId, sensorId }).ToList();
            }

            return links;
        }

        public List<SensorLink> GetLinksFrom(int sensorDbId)
        {
            List<SensorLink> links;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                links = db.Query<SensorLink>("SELECT * FROM NodesLinks WHERE fromSensorDbId=@sensorDbId",
                    new { sensorDbId }).ToList();
            }

            return links;
        }

        public List<SensorLink> GetLinksTo(int sensorDbId)
        {
            List<SensorLink> links;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                links = db.Query<SensorLink>("SELECT * FROM NodesLinks WHERE toSensorDbId=@sensorDbId",
                    new { sensorDbId }).ToList();
            }

            return links;
        }


        public List<SensorLink> GetAllLinks()
        {
            List<SensorLink> links;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                links = db.Query<SensorLink>("SELECT * FROM NodesLinks").ToList();
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

        public void DeleteLinksFrom(int nodeId, int sensorId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM NodesLinks WHERE fromNodeId=@nodeId AND fromSensorId=@sensorId",
                    new { nodeId, sensorId });
            }

        }

        public void DeleteLinksTo(int nodeId, int sensorId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM NodesLinks WHERE toNodeId=@nodeId AND toSensorId=@sensorId",
                    new { nodeId, sensorId });
            }

        }

        public void DeleteLinksFrom(int sensorDbId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM NodesLinks WHERE fromSensorDbId=@sensorDbId",
                    new { sensorDbId });
            }

        }

        public void DeleteLinksTo(int sensorDbId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM NodesLinks WHERE toSensorDbId=@sensorDbId",
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
