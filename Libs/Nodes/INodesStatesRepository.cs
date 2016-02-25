/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.Nodes
{


    public interface INodesStatesRepository
    {
        event LogEventHandler OnLogInfo;
        event LogEventHandler OnLogError;
        void SetWriteInterval(int ms);

        void AddState(NodeState state,int maxStatesCount);
        List<NodeState> GetStatesForNode(string nodeId);
        void RemoveStatesForNode(string nodeId);
        void RemoveAllStates();
    }
}
