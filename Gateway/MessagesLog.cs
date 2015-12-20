/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNetSensors.Gateway
{
    public delegate void OnNewMessageLoggedEventHandler(Message message);

    public class MessagesLog
    {
        //store last N messages
        public int maxMessaages=100;

        private List<Message> messages = new List<Message>();

        public event OnNewMessageLoggedEventHandler OnNewMessageLogged;
        public event Action OnClearMessages;


        public List<Message> GetAllMessages()
        {
            return messages;
        }

        public List<Message> GetLastMessages(int count)
        {
            List<Message> result = messages.Skip(
                Math.Max(0, messages.Count() - count)).ToList();

            return result;
        }

        public void AddNewMessage(Message message)
        {
            if (messages.Count >= maxMessaages)
                messages.Remove(messages.First());

            messages.Add(message);

            if (OnNewMessageLogged != null)
                OnNewMessageLogged(message);
        }

        public void ClearLog()
        {
            if (OnClearMessages != null)
                OnClearMessages();

            messages.Clear();
        }

        public List<Message> GetFilteredMessages(
            int? nodeId = null,
            int? sensorId = null,
            MessageType? messageType = null
            )
        {
            List<Message> result = messages;
            result = FilterMessages(result, nodeId, sensorId, messageType);

            return result;
        }

        public List<Message> FilterMessages(
            List<Message> messages,
            int? nodeId = null,
            int? sensorId = null,
            MessageType? messageType = null)
        {

            IEnumerable<Message> result = messages;

            if (nodeId!=null)
                result = from x in result
                         where x.nodeId == nodeId
                        select x;

            if (sensorId != null)
                result = from x in result
                              where x.sensorId == sensorId
                              select x;


            if (messageType != null)
                result = from x in result
                         where x.messageType == messageType
                              select x;



            return result.ToList();
        }


        public List<int> GetAllNodesId()
        {
            List<int> result = new List<int>();

            foreach (Message message in messages)
            {
                if (!result.Contains(message.nodeId))
                    result.Add(message.nodeId);
            }

            return result;
        }

        public List<int> GetAllSensorsId()
        {
            List<int> result = new List<int>();

            foreach (Message message in messages)
            {
                if (!result.Contains(message.sensorId))
                    result.Add(message.sensorId);
            }

            return result;
        }

        public List<MessageType> GetAllMessageTypes()
        {
            List<MessageType> result = new List<MessageType>();

            foreach (Message message in messages)
            {
                if (!result.Contains(message.messageType))
                    result.Add(message.messageType);
            }

            return result;
        }
    }
}
