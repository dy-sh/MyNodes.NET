using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using MyNetSensors.SerialGateway;

namespace MyNetSensors.WebController.Code
{
    public class EnittyOneToManyMapper<TP, TC, TPk>
    {
        private readonly IDictionary<TPk, TP> _lookup = new Dictionary<TPk, TP>();
        public Action<TP, TC> AddChildAction { get; set; }
        public Func<TP, TPk> ParentKey { get; set; }

        public virtual TP Map(TP parent, TC child)
        {
            TP entity;
            var found = true;
            var primaryKey = ParentKey(parent);
            if (!_lookup.TryGetValue(primaryKey, out entity))
            {
                _lookup.Add(primaryKey, parent);
                entity = parent;
                found = false;
            }
            AddChildAction(entity, child);
            return !found ? entity : default(TP);
        }
    }


    public class SensorsRepositoryDapper:ISensorsRepository
    {
        private IDbConnection db;

        public SensorsRepositoryDapper(string connectionString)
        {
            db = new SqlConnection(connectionString);
        }

        public List<SensorData> GetSensorDataLog(int db_Id)
        {
            string req = String.Format("SELECT * FROM Sensor{0}", db_Id);

            List<SensorData> list=null;
            try
            {
                list = db.Query<SensorData>(req).ToList();
            }
            catch { }

            return list;
        }

        public Node GetNodeByNodeId(int nodeId)
        {
            var mapper = new EnittyOneToManyMapper<Node, Sensor, int>()
            {
                AddChildAction = (node, sensor) =>
                {
                    if (node.sensors == null)
                        node.sensors = new List<Sensor>();

                    node.sensors.Add(sensor);
                },
                ParentKey = (node) => node.db_Id
            };

            string joinQuery = String.Format("SELECT * FROM Nodes n JOIN Sensors s ON n.db_Id = s.Node_db_Id WHERE n.nodeId = {0}", nodeId);

            Node result = db.Query<Node, Sensor, Node>(joinQuery, mapper.Map, splitOn: "db_Id").FirstOrDefault();


            return result;
        }

        public Node GetNodeByDbId(int db_Id)
        {
            var mapper = new EnittyOneToManyMapper<Node, Sensor, int>()
            {
                AddChildAction = (node, sensor) =>
                {
                    if (node.sensors == null)
                        node.sensors = new List<Sensor>();

                    node.sensors.Add(sensor);
                },
                ParentKey = (node) => node.db_Id
            };

            string joinQuery = String.Format("SELECT * FROM Nodes n JOIN Sensors s ON n.db_Id = s.Node_db_Id WHERE n.db_Id = {0}", db_Id);

            Node result = db.Query<Node, Sensor, Node>(joinQuery, mapper.Map, splitOn: "db_Id").FirstOrDefault();


            return result;
        }

        public Sensor GetSensorByDbId(int db_Id)
        {
            Sensor sensor = db.Query<Sensor>("SELECT * FROM Sensors WHERE db_Id = @db_Id", new { db_Id }).FirstOrDefault();

            return sensor;
        }

        public Sensor GetSensorBySensorId(int ownerNodeId, int sensorId)
        {
            Sensor sensor = db.Query<Sensor>("SELECT * FROM Sensors WHERE ownerNodeId = @ownerNodeId AND sensorId = @sensorId", new { ownerNodeId, sensorId }).FirstOrDefault();

            return sensor;
        }
    }
}