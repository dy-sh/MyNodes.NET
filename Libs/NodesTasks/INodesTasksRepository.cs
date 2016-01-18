/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;

namespace MyNetSensors.NodesTasks
{
    public interface INodesTasksRepository
    {
        int AddOrUpdateTask(NodeTask task);
        int AddTask(NodeTask task);
        void UpdateTask(NodeTask task);
        void UpdateTask(int id, bool isCompleted, DateTime executionDate, string executionValue, int repeatingDoneCount);
        void UpdateTask(int id, bool enabled, bool isCompleted, DateTime executionDate, int repeatingDoneCount);
        void UpdateTaskEnabled(int id, bool enabled);
        NodeTask GetTask(int id);
        List<NodeTask> GetTasksForNode(string nodeId);
        List<NodeTask> GetAllTasks();
        void RemoveTask(int id);
        void RemoveTasksForNode(string nodeId);
        void RemoveCompletedTasks();
        void RemoveCompletedTasksForNode(string nodeId);
        void RemoveAllTasks();
        void DisableTasks();
    }
}
