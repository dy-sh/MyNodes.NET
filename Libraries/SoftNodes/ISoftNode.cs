using MyNetSensors.Gateway;

namespace MyNetSensors.SoftNodes
{
    public interface ISoftNode
    {
        void SendMessage(Message message);
        void OnReceivedMessage(Message message);
        void SendSensorData(int sensorId, SensorData data);
    }
}
