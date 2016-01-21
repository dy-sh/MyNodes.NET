/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Timers;
using Dapper;
using MyNetSensors.Gateways;
using MyNetSensors.Nodes;

namespace MyNetSensors.Repositories.Dapper
{
    public class NodesStatesRepositoryDapper : INodesStatesRepository
    {
        private string connectionString;


        public NodesStatesRepositoryDapper(string connectionString)
        {
            this.connectionString = connectionString;
            CreateDb();
        }

        private void CreateDb()
        {
            using (var db = new SqlConnection(connectionString + ";Database= master"))
            {

                try
                {
                    //db = new SqlConnection("Data Source=.\\sqlexpress; Database= master; Integrated Security=True;");
                    db.Open();
                    db.Execute("CREATE DATABASE [MyNetSensors]");
                }
                catch
                {
                }
            }

            using (var db = new SqlConnection(connectionString))
            {

                try
                {
                    db.Open();

                    db.Execute(
                        @"CREATE TABLE [dbo].[NodesStates](
	            [Id] [int] IDENTITY(1,1) NOT NULL,
	            [NodeId] [nvarchar](max) NULL,
	            [State] [nvarchar](max) NULL,
	            [DateTime] [datetime] NOT NULL ) ON [PRIMARY] ");
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void AddState(NodeState state)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery = "INSERT INTO [NodesStates] (NodeId, State, DateTime) "
                               +
                               "VALUES(@NodeId, @State, @DateTime); "
                               + "SELECT CAST(SCOPE_IDENTITY() as int)";
                db.Query(sqlQuery, state);
            }
        }

        public List<NodeState> GetStatesForNode(string nodeId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                try
                {
                    string req = $"SELECT * FROM [NodesStates] WHERE NodeId=@nodeId";
                    return db.Query<NodeState>(req, new { nodeId }).ToList();
                }
                catch{}

                return null;
            }
        }

        public void RemoveStatesForNode(string nodeId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                db.Query($"DELETE FROM [NodesStates] WHERE NodeId=@nodeId", new { nodeId });
            }
        }

        public void RemoveAllStates()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                db.Query("TRUNCATE TABLE [NodesStates]");
            }
        }
    }
}