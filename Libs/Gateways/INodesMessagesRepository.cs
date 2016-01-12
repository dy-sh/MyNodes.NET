using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Gateways
{
    public interface INodesMessagesRepository
    {
        event LogEventHandler OnLogInfo;
        event LogEventHandler OnLogError;

        void ConnectToGateway(Gateway gateway);

        void AddMessage(Message message);
        List<Message> GetMessages();
        void DropMessages();


        bool IsDbExist();
        void SetWriteInterval(int ms);
        void Enable(bool enable);
    }
}
