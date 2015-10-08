/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        //if writeInterval==0, every message will be instantly writing to DB
        //and this will increase the reliability of the system, 
        //but this greatly slows down the performance.
        //If you set writeInterval>0, the state of all sensors 
        //will be writed to DB with this interval.
        //writeInterval should be large enough (3000 ms is ok)
        private int writeInterval = 3000;

        //slows down the performance
        private bool storeLogMessages = false;






        private string path;
        private SQLiteConnection conn;

        private SerialController serialController;

        private DispatcherTimer updateDbTimer;

        //store id-s of updated nodes, to write to db by timer
        private List<int> updatedNodesId = new List<int>();


        public SQLiteRepository(SerialController serialController)
        {
            InitializeDB();
            ConnectToController(serialController);
        }



        private void ConnectToController(SerialController serialController)
        {
            this.serialController = serialController;

            List<Message> messages = GetMessages();
            foreach (var message in messages)
                serialController.messagesLog.AddNewMessage(message);

            List<Node> nodes = GetNodes();
            foreach (var node in nodes)
                serialController.AddNode(node);

            serialController.messagesLog.OnClearMessages += OnClearMessages;
            serialController.OnClearNodesListEvent += OnClearNodesListEvent;

            if (storeLogMessages)
            {
                serialController.messagesLog.OnNewMessageLogged += OnNewMessage;
            }

            if (writeInterval == 0)
            {
                serialController.OnNewNodeEvent += AddOrUpdateNode;
                serialController.OnNodeUpdatedEvent += AddOrUpdateNode;
                serialController.OnNewSensorEvent += AddOrUpdateSensor;
                serialController.OnSensorUpdatedEvent += AddOrUpdateSensor;
            }
            else
            {
                serialController.OnNewNodeEvent += OnNodeUpdated;
                serialController.OnNodeUpdatedEvent += OnNodeUpdated;
                serialController.OnNewSensorEvent += OnSensorUpdated;
                serialController.OnSensorUpdatedEvent += OnSensorUpdated;

                updateDbTimer = new DispatcherTimer();
                updateDbTimer.Interval = TimeSpan.FromMilliseconds(writeInterval);
                updateDbTimer.Tick += UpdateDbTimer;
                updateDbTimer.Start();
            }
        }

 

        private void InitializeDB()
        {
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.sqlite");

            conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            conn.CreateTable<Message>();
            conn.CreateTable<Node>();
            conn.CreateTable<Sensor>();
            conn.CreateTable<SensorData>();
        }

        public void DropMessages()
        {
            conn.DropTable<Message>();

            conn.CreateTable<Message>();
        }

        public void DropNodes()
        {
            updatedNodesId.Clear();

            conn.DropTable<Node>();
            conn.DropTable<Sensor>();
            conn.DropTable<SensorData>();

            conn.CreateTable<Node>();
            conn.CreateTable<Sensor>();
            conn.CreateTable<SensorData>();
        }


        private void OnClearMessages()
        {
            DropMessages();
        }

        private void OnClearNodesListEvent()
        {
            DropNodes();
        }


        public List<Message> GetMessages()
        {
            List<Message> list = conn.Table<Message>().ToList();
            return list;
        }

        public void OnNewMessage(Message message)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            conn.Insert(message);
            sw.Stop();
            Debug.WriteLine("Writing to DB: " + sw.ElapsedMilliseconds + " ms");
        }

        public List<Node> GetNodes()
        {

            List<Node> list = conn.GetAllWithChildren<Node>(null, true);
            foreach (var node in list)
            {
                node.registered = node.registered.ToLocalTime();
                node.lastSeen = node.lastSeen.ToLocalTime();
            }
            return list;
        }

        public void AddOrUpdateNode(Node node)
        {
            Node oldNode = conn.Query<Node>("select * from Node where nodeId = ?", node.nodeId).FirstOrDefault();

            if (oldNode != null)
                conn.UpdateWithChildren(node);
            else
                conn.InsertWithChildren(node);

        }

        public void AddOrUpdateSensor(Sensor sensor)
        {
            Sensor oldSensor = conn.Table<Sensor>().Where(x => x.NodeId == sensor.NodeId).FirstOrDefault(x => x.sensorId == sensor.sensorId);

            if (oldSensor != null)
                conn.InsertOrReplaceWithChildren(sensor, true);
            else
                conn.InsertWithChildren(sensor);

        }

        public void WriteAllNodes()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<Node> nodes = serialController.GetNodes();
            foreach (var node in nodes)
                conn.InsertOrReplaceWithChildren(node, true);

            sw.Stop();
            Debug.WriteLine(sw.ElapsedMilliseconds);
        }

        private void UpdateDbTimer(object sender, object e)
        {
            WriteUpdatedNodes();
        }






        private void OnNodeUpdated(Node node)
        {
            if (!updatedNodesId.Contains(node.nodeId))
                updatedNodesId.Add(node.nodeId);
        }

        private void OnSensorUpdated(Sensor sensor)
        {
            if (!updatedNodesId.Contains(sensor.nodeId))
                updatedNodesId.Add(sensor.nodeId);
        }

        public void WriteUpdatedNodes()
        {
            if (!updatedNodesId.Any()) return;
            
            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<Node> nodes = serialController.GetNodes();
            foreach (var id in updatedNodesId)
            {
                Node node = nodes.FirstOrDefault(x => x.nodeId == id);
                conn.InsertOrReplaceWithChildren(node, true);
            }
            updatedNodesId.Clear();

            sw.Stop();
            Debug.WriteLine("Writing to DB: "+sw.ElapsedMilliseconds+" ms");
        }
    }

}
