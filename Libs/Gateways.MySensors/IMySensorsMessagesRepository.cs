/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.Gateways.MySensors
{
    public interface IMySensorsMessagesRepository
    {
        event LogEventHandler OnLogInfo;
        event LogEventHandler OnLogError;

        void AddMessage(Message message);
        List<Message> GetMessages();
        void RemoveAllMessages();

        void SetWriteInterval(int ms);
    }
}
