/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Dapper;
using MyNetSensors.Gateways;

namespace MyNetSensors.Repositories.Dapper
{




    public class NodesMessagesRepositoryDapper : INodesMessagesRepository
    {
        //if writeInterval==0, every message will be instantly writing to DB
        //and this will increase the reliability of the system, 
        //but this greatly slows down the performance.
        //If you set writeInterval>0, the state of all sensors 
        //will be writed to DB with this interval.
        //writeInterval should be large enough (3000 ms is ok)
        private int writeInterval = 5000;

        private bool enable = true;

        private Gateway gateway;
        private Timer updateDbTimer = new Timer();

        //messages list, to write to db by timer
        private List<Message> newMessages = new List<Message>();

        private string connectionString;

        public event LogEventHandler OnLogStateMessage;

        public NodesMessagesRepositoryDapper(string connectionString)
        {
            updateDbTimer.Elapsed += UpdateDbTimerEvent;

            this.connectionString = connectionString;
            CreateDb();
        }


        public void ConnectToGateway(Gateway gateway)
        {

            this.gateway = gateway;

            List<Message> messages = GetMessages();
            foreach (var message in messages)
                gateway.messagesLog.AddNewMessage(message);

            gateway.messagesLog.OnNewMessageLogged += OnNewMessage;
            gateway.messagesLog.OnClearMessages += OnClearMessages;

            if (writeInterval > 0)
            {
                updateDbTimer.Interval = writeInterval;
                updateDbTimer.Start();
            }
        }
        

        private void CreateDb()
        {
            using (var db = new SqlConnection(connectionString + ";Database= master"))
            {

                try
                {
                    //db = new SqlConnection("Data Source=.\\sqlexpress; Database= master; Integrated Security=True;");
                    db.Open();
                    db.Execute("CREATE DATABASE [MyNetSensors]");
                }
                catch
                {
                }
            }

            using (var db = new SqlConnection(connectionString))
            {

                try
                {
                    db.Open();

                    db.Execute(
                        @"CREATE TABLE [dbo].[Messages](
	            [Id] [int] IDENTITY(1,1) NOT NULL,
	            [nodeId] [int] NOT NULL,
	            [sensorId] [int] NOT NULL,
	            [messageType] [int] NOT NULL,
	            [ack] [bit] NOT NULL,
	            [subType] [int] NOT NULL,
	            [payload] [nvarchar](max) NULL,
	            [isValid] [bit] NOT NULL,
	            [incoming] [bit] NOT NULL,
	            [dateTime] [datetime] NOT NULL ) ON [PRIMARY] ");
                }
                catch
                {
                }
            }
        }

        public void DropMessages()
        {
            newMessages.Clear();

            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                db.Query("TRUNCATE TABLE [Messages]");
            }
        }

      
        private void OnClearMessages()
        {
            DropMessages();
        }

   
        public List<Message> GetMessages()
        {
            List<Message> messages;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                messages = db.Query<Message>("SELECT * FROM Messages").ToList();
            }
            return messages;
        }

        private void OnNewMessage(Message message)
        {
            if (!enable) return;

            if (writeInterval == 0)
                AddMessage(message);
            else
                newMessages.Add(message);
        }

        public void AddMessage(Message message)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery = "INSERT INTO Messages (nodeId, sensorId, messageType, ack, subType ,payload, isValid, incoming, dateTime) "
                               +
                               "VALUES(@nodeId, @sensorId, @messageType, @ack, @subType, @payload, @isValid, @incoming, @dateTime); "
                               + "SELECT CAST(SCOPE_IDENTITY() as int)";
                db.Query(sqlQuery, message);
            }
        }

    


        private void WriteNewMessages()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                //to prevent changing of collection while writing to db is not yet finished
                Message[] messages = new Message[newMessages.Count];
                newMessages.CopyTo(messages);
                newMessages.Clear();

                var sqlQuery = "INSERT INTO Messages (nodeId, sensorId, messageType, ack, subType ,payload, isValid, incoming, dateTime) "
                               +
                               "VALUES(@nodeId, @sensorId, @messageType, @ack, @subType, @payload, @isValid, @incoming, @dateTime); "
                               + "SELECT CAST(SCOPE_IDENTITY() as int)";
                db.Execute(sqlQuery, messages);
            }
        }
        

        public bool IsDbExist()
        {
            //todo check if db exist
            return true;
        }



        public void Enable(bool enable)
        {
            this.enable = enable;
        }



        
        






        private void UpdateDbTimerEvent(object sender, object e)
        {
            updateDbTimer.Stop();
            try
            {
                int messages = newMessages.Count;
                if (messages == 0)
                {
                    updateDbTimer.Start();
                    return;
                }
                ;
                Stopwatch sw = new Stopwatch();
                sw.Start();


                WriteNewMessages();

                sw.Stop();
                long elapsed = sw.ElapsedMilliseconds;
                float messagesPerSec = (float) messages/(float) elapsed*1000;
                Log($"Writing to DB: {elapsed} ms ({messages} inserts, {(int) messagesPerSec} inserts/sec)");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            updateDbTimer.Start();
        }

        public void SetWriteInterval(int ms)
        {
            writeInterval = ms;
            updateDbTimer.Stop();
            if (writeInterval > 0)
            {
                updateDbTimer.Interval = writeInterval;
                if (gateway != null)
                    updateDbTimer.Start();
            }
        }

        public void Log(string message)
        {
            OnLogStateMessage?.Invoke(message);
        }

    }



}
