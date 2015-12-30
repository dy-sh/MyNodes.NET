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
        void CreateDb();
        bool IsDbExist();
        int AddOrUpdateTask(NodeTask task);
        int AddTask(NodeTask task);
        void UpdateTask(NodeTask task);
        void UpdateTask(int db_Id, bool isCompleted, DateTime executionDate, string executionValue, int repeatingDoneCount);
        void UpdateTask(int db_Id, bool enabled, bool isCompleted, DateTime executionDate, int repeatingDoneCount);
        void UpdateTaskEnabled(int db_Id, bool enabled);
        NodeTask GetTask(int db_Id);
        List<NodeTask> GetTasks(int nodeId, int sensorId);
        List<NodeTask> GetAllTasks();
        void DeleteTask(int db_Id);
        void DeleteTasks(int nodeId,int sensorId);
        void DeleteTasks(int sensorDbId);
        void DeleteCompleted();
        void DeleteCompleted(int nodeId, int sensorId);
        void DropTasks();
        void DisableTasks();
    }
}
