using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI.Xaml;
using SQLite.Net;
using SQLite.Net.Interop;
using SQLiteNetExtensions.Extensions;

namespace SerialController_Windows.Code
{
    public class SQLiteRepository
    {

        string path;
        SQLite.Net.SQLiteConnection conn;

        public SQLiteRepository()
        {

            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.sqlite");

            conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            conn.CreateTable<Message>();
            conn.CreateTable<Node>();
            conn.CreateTable<Sensor>();
            conn.CreateTable<SensorData>();
        }

        public List<Message> GetMessages()
        {
            List<Message> list = conn.Table<Message>().ToList();
            return list;
        }

        public void AddMessage(Message message)
        {
            conn.Insert(message);
        }

        public List<Node> GetNodes()
        {
   
            List<Node> list = conn.GetAllWithChildren< Node >(null, true);
            return list;
        }

        public void AddNode(Node node)
        {

            //Node oldNode = conn.Get<Node>(x => x.nodeId == node.nodeId);

            Node oldNode = conn.Query<Node>("select * from Node where nodeId = ?", node.nodeId).FirstOrDefault();

            if (oldNode != null)
            {
             //   oldNode = conn.GetWithChildren<Node>(oldNode.Id,true);
              //  oldNode.lastSeen = node.lastSeen;
               // node.Id = oldNode.Id;
                conn.InsertOrReplaceWithChildren(node,true);

            }
            else
            {
                conn.InsertWithChildren(node);
            }
        }

        public void AddSensor(Sensor sensor)
        {
             Sensor oldSensor = conn.Table<Sensor>().Where(x => x.NodeId==sensor.NodeId).FirstOrDefault(x=>x.sensorId==sensor.sensorId);

            if (oldSensor != null)
            {
               // oldSensor = conn.GetWithChildren<Sensor>(oldSensor.Id, true);
                //  oldNode.lastSeen = node.lastSeen;
               // sensor.Id = oldSensor.Id;
                conn.InsertOrReplaceWithChildren(sensor, true);

            }
            else
            {
                conn.InsertWithChildren(sensor);
            }
        }
    }

}
