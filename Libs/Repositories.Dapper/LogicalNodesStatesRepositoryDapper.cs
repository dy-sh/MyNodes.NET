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
using MyNetSensors.LogicalNodes;

namespace MyNetSensors.Repositories.Dapper
{
    public class LogicalNodesStatesRepositoryDapper : ILogicalNodesStatesRepository
    {
        public LogicalNodesStatesRepositoryDapper(string connectionString)
        {
            
        }

        public void AddState(NodeState state)
        {
            throw new NotImplementedException();
        }

        public List<NodeState> GetStatesForNode(string nodeId)
        {
            throw new NotImplementedException();
        }

        public void RemoveStatesForNode(string nodeId)
        {
            throw new NotImplementedException();
        }

        public void RemoveAllStates()
        {
            throw new NotImplementedException();
        }
    }
}