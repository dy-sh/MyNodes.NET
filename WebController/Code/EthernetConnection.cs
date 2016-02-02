using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyNetSensors.Gateways.MySensors.Serial;

namespace MyNetSensors.WebController.Code
{
    public class EthernetConnection : IComPort
    {
        public event OnDataReceivedEventHandler OnDataReceived;
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event ExceptionEventHandler OnWritingError;
        public event ExceptionEventHandler OnConnectingError;
        public event LogEventHandler OnLogMessage;
        public event LogEventHandler OnLogInfo;
        public event LogEventHandler OnLogError;

        private TcpClient tcpClient;
        NetworkStream tcpStream;
        public List<string> GetPortsList()
        {

            return null;
        }

        public void Connect(string portName, int baudRate = 115200)
        {
            try
            {
                tcpClient = new TcpClient();

                tcpClient.Connect("192.168.88.20", 5003);

                tcpStream = tcpClient.GetStream();

                LogInfo($"Connected to 192.168.88.20");
                OnConnected?.Invoke();

                ReadFromSocket();
            }
            catch (SocketException ex)
            {
                LogError($"Failed to connect. {ex.ToString()}");
                OnConnectingError?.Invoke(ex);
            }
        }

        private async void ReadFromSocket()
        {
            while (IsConnected())
            {

                try
                {// Server Reply
                    if (tcpStream.CanRead)
                    {
                        // Buffer to store the response bytes.
                        byte[] readBuffer = new byte[tcpClient.ReceiveBufferSize];
                        string fullServerReply = null;
                        using (var writer = new MemoryStream())
                        {
                            do
                            {
                                int numberOfBytesRead = tcpStream.Read(readBuffer, 0, readBuffer.Length);
                                if (numberOfBytesRead <= 0)
                                {
                                    break;
                                }
                                writer.Write(readBuffer, 0, numberOfBytesRead);
                            } while (tcpStream.DataAvailable);
                            fullServerReply = Encoding.UTF8.GetString(writer.ToArray());
                            LogMessage("RX: " + fullServerReply.TrimEnd('\r', '\n'));
                            OnDataReceived?.Invoke(fullServerReply);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogError("Failed to read data from socket. " + ex.Message);
                }

                await Task.Delay(1);
            }
        }

        public void Disconnect()
        {
            if (!IsConnected())
                return;

            tcpClient.Close();
        }

        public void SendMessage(string message)
        {
            if (tcpClient == null || tcpStream == null || !IsConnected())
            {
                LogError("Failed to write data. Port is not connected.");
                return;
            }

            LogMessage("TX: " + message.TrimEnd('\r', '\n'));

            try
            {
                byte[] sendBytes = Encoding.UTF8.GetBytes(message);
                tcpStream.Write(sendBytes, 0, sendBytes.Length);
            }
            catch (Exception ex)
            {
                LogError($"Failed to write data. {ex.Message}");

                OnWritingError?.Invoke(ex);

                Disconnect();
            }
        }

        public bool IsConnected()
        {
            return tcpClient.Connected;
        }

        public string GetPortName()
        {
            return null;
        }

        private void LogMessage(string message)
        {
            OnLogMessage?.Invoke(message);
        }

        private void LogInfo(string message)
        {
            OnLogInfo?.Invoke(message);
        }

        private void LogError(string message)
        {
            OnLogError?.Invoke(message);
        }
    }
}
