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
using MyNetSensors.Nodes;

namespace MyNetSensors.Repositories.EF.SQLite
{
    public class NodesStatesRepositoryEf : INodesStatesRepository
    {
        private NodesStatesHistoryDbContext historyDb;

        public NodesStatesRepositoryEf(NodesStatesHistoryDbContext nodesStatesHistoryDbContext)
        {
            this.historyDb = nodesStatesHistoryDbContext;
            CreateDb();
        }

        public void CreateDb()
        {
            try
            {
                historyDb.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public void AddState(NodeState state)
        {
            historyDb.NodesStates.Add(state);
            historyDb.SaveChanges();
        }

        public List<NodeState> GetStatesForNode(string nodeId)
        {
            return historyDb.NodesStates.Where(x => x.NodeId == nodeId).ToList();
        }

        public void RemoveStatesForNode(string nodeId)
        {
            historyDb.RemoveRange(historyDb.NodesStates.Where(x => x.NodeId == nodeId));
            historyDb.SaveChanges();
        }

        public void RemoveAllStates()
        {
            historyDb.NodesStates.RemoveRange(historyDb.NodesStates);
            historyDb.SaveChanges();
        }
    }
}