using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using MyNetSensors.LogicalNodes;

namespace MyNetSensors.LogicalNodesRepositoryDappers
{
    public class LogicalNodesRepositoryDapper:ILogicalNodesRepository
    {
        private string connectionString;

        public LogicalNodesRepositoryDapper(string connectionString)
        {
            this.connectionString = connectionString;
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
                        @"CREATE TABLE [dbo].[SensorsLinks](
	                    [db_Id] [int] IDENTITY(1,1) NOT NULL,
	                    [fromSensorDbId] [int] NULL,       
	                    [fromNodeId] [int] NULL,       
	                    [fromSensorId] [int] NULL,       
	                    [fromDataType] [int] NULL,       
	                    [fromSensorDescription] [nvarchar](max) NULL,       
	                    [toSensorDbId] [int] NULL,       
	                    [toNodeId] [int] NULL,       
	                    [toSensorId] [int] NULL,
	                    [toDataType] [int] NULL, 
	                    [toSensorDescription] [nvarchar](max) NULL 
                        ) ON [PRIMARY] ";

                    db.Query(req);
                }
                catch
                {
                }
            }
        }


        private void CreateLinksTable()
        {
            throw new NotImplementedException();
        }



        public bool IsDbExist()
        {
            throw new NotImplementedException();
        }

        public int AddOrUpdateNode(LogicalNode node)
        {
            throw new NotImplementedException();
        }

        public int AddNode(LogicalNode node)
        {
            throw new NotImplementedException();
        }

        public void UpdateNode(LogicalNode node)
        {
            throw new NotImplementedException();
        }

        public LogicalNode GetNode(string id)
        {
            throw new NotImplementedException();
        }

        public List<LogicalNode> GetAllNodes()
        {
            throw new NotImplementedException();
        }

        public void DeleteNode(string id)
        {
            throw new NotImplementedException();
        }

        public void DropNodes()
        {
            throw new NotImplementedException();
        }

        public int AddOrUpdateLink(LogicalLink link)
        {
            throw new NotImplementedException();
        }

        public int AddLink(LogicalLink link)
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
