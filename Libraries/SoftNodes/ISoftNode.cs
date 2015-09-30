using MyNetSensors.Gateway;

namespace MyNetSensors.SoftNodes
{
    public interface ISoftNode
    {
        void ConnectToServer(string url = "http://localhost:13122/");
        void Disconnect();
        bool IsConnected();
        void SendMessage(Message message);
        void OnReceivedMessage(Message message);
        void SendSensorData(int sensorId, SensorData data);
    }
}
