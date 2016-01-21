using System.Collections.Generic;

namespace MyNetSensors.Gateways.MySensors.Serial
{
    public interface IMySensorsMessagesRepository
    {
        event LogEventHandler OnLogInfo;
        event LogEventHandler OnLogError;

        void ConnectToGateway(Gateway gateway);

        void AddMessage(Message message);
        List<Message> GetMessages();
        void RemoveAllMessages();


        bool IsDbExist();
        void SetWriteInterval(int ms);
        void Enable(bool enable);
    }
}
