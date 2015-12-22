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
                    SensorData oldData = sensor.GetData(link.fromDataType.Value);
                    SensorData newData = ConvertSensorData(oldData, link.toDataType);
                    gateway.SendSensorState(link.toNodeId, link.toSensorId, newData);
                }
            }
        }

        private SensorData ConvertSensorData(SensorData oldData, SensorDataType? newDataType)
        {
            SensorData newData = (SensorData)oldData.Clone();
            newData.dataType = newDataType;

            //convert binary to percentage 
            if ((oldData.IsBinary())
                && (newData.IsPercentage()))
            {
                if (oldData.state == "0")
                    newData.state = "0";
                else
                    newData.state = "100";
            }

            //convert  percentage to binary
            if ((newData.IsBinary())
                && (oldData.IsPercentage()))
            {
                if (oldData.state == "0")
                    newData.state = "0";
                else
                    newData.state = "1";
            }

            return newData;
        }
    }
}
