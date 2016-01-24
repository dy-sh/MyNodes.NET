using System.Collections.Generic;

namespace MyNetSensors.Gateways.MySensors.Serial
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
