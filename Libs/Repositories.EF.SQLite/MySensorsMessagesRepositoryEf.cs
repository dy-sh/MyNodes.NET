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
using Microsoft.Data.Entity;
using MyNetSensors.Gateways;
using MyNetSensors.Gateways.MySensors.Serial;

namespace MyNetSensors.Repositories.EF.SQLite
{




    public class MySensorsMessagesRepositoryEf : IMySensorsMessagesRepository
    {
        private string connectionString;

        //if writeInterval==0, every message will be instantly writing to DB
        //and this will increase the reliability of the system, 
        //but this greatly slows down the performance.
        //If you set writeInterval>0, the state of all sensors 
        //will be writed to DB with this interval.
        //writeInterval should be large enough (3000 ms is ok)
        private int writeInterval = 5000;

        private bool enable = true;

        private Timer updateDbTimer = new Timer();

        //messages list, to write to db by timer
        private List<Message> newMessages = new List<Message>();

        private MySensorsMessagesDbContext db;

        public event LogEventHandler OnLogInfo;
        public event LogEventHandler OnLogError;

        public MySensorsMessagesRepositoryEf(MySensorsMessagesDbContext mySensorsDbContext)
        {
            updateDbTimer.Elapsed += UpdateDbTimerEvent;

            this.db = mySensorsDbContext;
            CreateDb();

            if (writeInterval > 0)
            {
                updateDbTimer.Interval = writeInterval;
                updateDbTimer.Start();
            }
        }



        public void CreateDb()
        {
            try
            {
                db.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public void RemoveAllMessages()
        {
            newMessages.Clear();

            db.Messages.RemoveRange(db.Messages);
            db.SaveChanges();
        }


 
        public List<Message> GetMessages()
        {
            return db.Messages.ToList();
        }



        public void AddMessage(Message message)
        {
            if (writeInterval == 0)
            {
                db.Messages.Add(message);
                db.SaveChanges();
            }
            else
                newMessages.Add(message);
        }


 
        private void WriteNewMessages()
        {
            Message[] messages = new Message[newMessages.Count];
            newMessages.CopyTo(messages);
            newMessages.Clear();

            db.Messages.AddRange(messages);
            db.SaveChanges();
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

                Stopwatch sw = new Stopwatch();
                sw.Start();


                WriteNewMessages();

                sw.Stop();
                long elapsed = sw.ElapsedMilliseconds;
                float messagesPerSec = (float) messages/(float) elapsed*1000;
                LogInfo($"Writing gateway messages: {elapsed} ms ({messages} inserts, {(int) messagesPerSec} inserts/sec)");
            }
            catch(Exception ex)
            {
                
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
                updateDbTimer.Start();
            }
        }

        public void LogInfo(string message)
        {
            OnLogInfo?.Invoke(message);
        }

        public void LogError(string message)
        {
            OnLogError?.Invoke(message);
        }
    }



}
