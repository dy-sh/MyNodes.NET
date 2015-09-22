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

        public Node GetNode(int db_Id)
        {
            Node node = db.Query<Node>("SELECT * FROM Nodes WHERE db_Id = @db_Id", new { db_Id }).SingleOrDefault();

            return node;
        }

        public Sensor GetSensor(int db_Id)
        {
            Sensor sensor = db.Query<Sensor>("SELECT * FROM Sensors WHERE db_Id = @db_Id", new { db_Id }).SingleOrDefault();

            return sensor;
        }
    }
}