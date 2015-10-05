/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNetSensors.Gateway;

namespace MyNetSensors.NodeTasks
{
    public interface ISensorsTasksRepository
    {
        void CreateDb();
        bool IsDbExist();
        int AddOrUpdateTask(SensorTask task);
        int AddTask(SensorTask task);
        void UpdateTask(SensorTask task);
        void UpdateTask(int db_Id, bool isCompleted, DateTime executionDate, string executionValue, int repeatingDoneCount);
        void UpdateTask(int db_Id, bool enabled, bool isCompleted, DateTime executionDate, int repeatingDoneCount);
        void UpdateTaskEnabled(int db_Id, bool enabled);
        SensorTask GetTask(int db_Id);
        List<SensorTask> GetTasks(int nodeId, int sensorId);
        List<SensorTask> GetAllTasks();
        void DeleteTask(int db_Id);
        void DeleteTasks(int nodeId,int sensorId);
        void DeleteTasks(int sensorDbId);
        void DeleteCompleted();
        void DeleteCompleted(int nodeId, int sensorId);
        void DropAllTasks();
        void DisableAllTasks();
    }
}
