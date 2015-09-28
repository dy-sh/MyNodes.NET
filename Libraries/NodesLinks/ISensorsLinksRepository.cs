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
        List<SensorLink> GetIncomingLinks(int nodeId, int sensorId);
        List<SensorLink> GetOutgoingLinks(int nodeId, int sensorId);
        List<SensorLink> GetIncomingLinks(int sensorDbId);
        List<SensorLink> GetOutgoingLinks(int sensorDbId);
        List<SensorLink> GetAllLinks();
        void DeleteLink(int db_Id);
        void DeleteIncomingLinks(int nodeId, int sensorId);
        void DeleteOutgoingLinks(int nodeId, int sensorId);
        void DeleteIncomingLinks(int sensorDbId);
        void DeleteOutgoingLinks(int sensorDbId);
        void DropAllLinks();
    }
}
