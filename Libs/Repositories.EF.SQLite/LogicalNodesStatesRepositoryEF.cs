/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Timers;
using Microsoft.Data.Entity;
using MyNetSensors.Gateways;
using MyNetSensors.LogicalNodes;

namespace MyNetSensors.Repositories.EF.SQLite
{
    public class LogicalNodesStatesRepositoryEF : ILogicalNodesStatesRepository
    {
        private LogicalNodesStatesDbContext db;

        public LogicalNodesStatesRepositoryEF(LogicalNodesStatesDbContext logicalNodesStatesDbContext)
        {
            this.db = logicalNodesStatesDbContext;
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

        public void AddState(NodeState state)
        {
            db.NodesStates.Add(state);
            db.SaveChanges();
        }

        public List<NodeState> GetStatesForNode(string nodeId)
        {
            return db.NodesStates.Where(x => x.NodeId == nodeId).ToList();
        }

        public void RemoveStatesForNode(string nodeId)
        {
            db.RemoveRange(db.NodesStates.Where(x => x.NodeId == nodeId));
        }

        public void RemoveAllStates()
        {
            db.NodesStates.RemoveRange(db.NodesStates);
            db.SaveChanges();
        }
    }
}