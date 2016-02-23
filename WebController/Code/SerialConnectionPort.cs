/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using MyNetSensors.Gateways;
using MyNetSensors.Gateways.MySensors;

namespace MyNetSensors.WebController.Code
{
    public class SerialConnectionPort : IGatewayConnectionPort
    {
        public event OnDataReceivedEventHandler OnDataReceived;
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event ExceptionEventHandler OnWritingError;
        public event ExceptionEventHandler OnConnectingError;
        public event LogEventHandler OnLogMessage;
        public event LogEventHandler OnLogInfo;
        public event LogEventHandler OnLogError;

        private bool isConnected;
        private SerialPort serialPort;

        private string portName;
        private int baudRate;

        public bool IsConnected()
        {
            return isConnected;
        }

        public string GetPortName()
        {
            return portName;
        }

        public void SetPortName(string portName)
        {
            Disconnect();
            this.portName = portName;
        }

        public int GetBaudRate()
        {
            return baudRate;
        }

        public void SetBaudRate(int baudRate)
        {
            Disconnect();
            this.baudRate = baudRate;
        }


        public static List<string> GetAvailablePorts()
        {
            return SerialPort.GetPortNames().ToList();
        }


        public SerialConnectionPort(string portName, int baudRate)
        {
            this.portName = portName;
            this.baudRate = baudRate;
        }





        public void SendMessage(string message)
        {
            if (serialPort == null || !isConnected)
            {
                LogError("Failed to write data. Port is not connected.");
                return;
            }

            LogMessage("TX: " + message.TrimEnd('\r', '\n'));

            try
            {
                serialPort.Write(message);
            }
            catch (Exception ex)
            {
                LogError($"Failed to write data. {ex.Message}");

                OnWritingError?.Invoke(ex);

                Disconnect();
            }
        }


        public void Connect()
        {
            try
            {
                serialPort = new SerialPort(portName)
                {
                    BaudRate = baudRate,
                    Parity = Parity.None,
                    StopBits = StopBits.One,
                    DataBits = 8
                };
                serialPort.DataReceived += serialPort_DataReceived;
                serialPort.Open();

                isConnected = true;
                LogInfo($"Connected to port {portName} at {baudRate} bits/s.");
                OnConnected?.Invoke();
            }
            catch (Exception ex)
            {
                LogError($"Failed to connect to port {portName}.");
                OnConnectingError?.Invoke(ex);
            }
        }

        public void Disconnect()
        {
            if (!isConnected || serialPort == null)
                return;

            isConnected = false;

            //LogPortState("Port disconnected.");

            serialPort?.Dispose();
            serialPort = null;

            OnDisconnected?.Invoke();
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadExisting();

            LogMessage("RX: " + data);

            OnDataReceived?.Invoke(data);
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
