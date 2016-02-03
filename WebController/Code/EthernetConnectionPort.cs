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
    public class EthernetConnectionPort : IGatewayConnectionPort
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
        private string ip;
        private int port;

        private bool isConnected;


        public EthernetConnectionPort(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }

        public string GetIp()
        {
            return ip;
        }

        public int GetPort()
        {
            return port;
        }

        public void SetIpAndPort(string ip, int port)
        {
            Disconnect();
            this.ip = ip;
            this.port = port;
        }


        public void Connect()
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(ip, port);
                tcpStream = tcpClient.GetStream();

                isConnected = true;

                LogInfo($"Connected to {ip}:{port}");
                OnConnected?.Invoke();

                ReadFromSocket();
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.TimedOut)
                    LogError($"Failed to connect to {ip}:{port}. Remote host is not responding.");
                else
                    LogError($"Failed to connect to {ip}:{port}. {ex.ToString()}");
                OnConnectingError?.Invoke(ex);
            }
        }

        private async void ReadFromSocket()
        {
            while (isConnected)
            {
                try
                {
                    if (tcpStream.CanRead)
                    {
                        // Buffer to store the response bytes.
                        byte[] readBuffer = new byte[tcpClient.ReceiveBufferSize];
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
                            var fullServerReply = Encoding.UTF8.GetString(writer.ToArray());

                            LogMessage("RX: " + fullServerReply.TrimEnd('\r', '\n'));
                            OnDataReceived?.Invoke(fullServerReply);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!(ex is IOException))
                        LogError("Failed to read data from port. " + ex.Message);
                }

                await Task.Delay(1);
            }
        }

        public void Disconnect()
        {
            isConnected = false;

            if (!IsConnected())
                return;

            tcpClient.Close();
            OnDisconnected?.Invoke();
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
