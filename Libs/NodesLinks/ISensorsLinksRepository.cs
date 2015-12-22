/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.NodesLinks
{
    public interface ISensorsLinksRepository
    {
        void CreateDb();
        bool IsDbExist();
        int AddOrUpdateLink(SensorLink link);
        int AddLink(SensorLink link);
        void UpdateLink(SensorLink link);
        SensorLink GetLink(int db_Id);
        List<SensorLink> GetLinksFrom(int nodeId, int sensorId);
        List<SensorLink> GetLinksTo(int nodeId, int sensorId);
        List<SensorLink> GetLinksFrom(int sensorDbId);
        List<SensorLink> GetLinksTo(int sensorDbId);
        List<SensorLink> GetAllLinks();
        void DeleteLink(int db_Id);
        void DeleteLinksFrom(int nodeId, int sensorId);
        void DeleteLinksTo(int nodeId, int sensorId);
        void DeleteLinksFrom(int sensorDbId);
        void DeleteLinksTo(int sensorDbId);
        void DropLinks();
    }
}
