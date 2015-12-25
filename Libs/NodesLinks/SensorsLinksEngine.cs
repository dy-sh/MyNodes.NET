/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using MyNetSensors.Gateway;

namespace MyNetSensors.NodesLinks
{
    public class SensorsLinksEngine
    {
        private SerialGateway gateway;
        private ISensorsLinksRepository db;

        private List<SensorLink> links = new List<SensorLink>();

        public SensorsLinksEngine(SerialGateway gateway, ISensorsLinksRepository db)
        {
            this.db = db;
            this.gateway = gateway;

            gateway.OnClearNodesListEvent += OnClearNodesListEvent;
            gateway.OnSensorUpdatedEvent += OnSensorUpdatedEvent;

            db.CreateDb();
            GetLinksFromRepository();
        }

        public void GetLinksFromRepository()
        {
            links = db.GetAllLinks();
        }

        private void OnClearNodesListEvent()
        {
            links.Clear();
            db.DropLinks();
        }

        private void OnSensorUpdatedEvent(Sensor sensor)
        {
            foreach (var link in links)
            {
                if (link.fromNodeId == sensor.nodeId
                    && link.fromSensorId == sensor.sensorId)
                {
                    string state = sensor.ConvertSensorData(link.toDataType);
                    gateway.SendSensorState(link.toNodeId, link.toSensorId, state);
                }
            }
        }


    }
}
