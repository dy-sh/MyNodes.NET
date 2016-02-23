/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.Nodes
{
    public interface IUITimerNodesRepository
    {
        int AddTask(UITimerTask task);
        void UpdateTask(UITimerTask task);
        void UpdateTasks(List<UITimerTask> tasks);
        UITimerTask GetTask(int id);
        List<UITimerTask> GetTasksForNode(string nodeId);
        List<UITimerTask> GetAllTasks();
        void RemoveTask(int id);
        void RemoveTasks(List<UITimerTask> list);
        void RemoveTasksForNode(string nodeId);
        void RemoveAllTasks();
    }
}
