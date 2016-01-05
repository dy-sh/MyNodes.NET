/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using MyNetSensors.Gateways;

namespace MyNetSensors.SerialControllers
{
    public class ComPort : IComPort
    {
        public event OnDataReceivedEventHandler OnDataReceivedEvent;
        public event Action OnConnectedEvent;
        public event Action OnDisconnectedEvent;
        public event ExceptionEventHandler OnWritingError;
        public event ExceptionEventHandler OnConnectingError;
        public event Gateways.LogEventHandler OnLogMessage;
        public event Gateways.LogEventHandler OnLogState;

        private bool isConnected;
        private SerialPort serialPort;


        public bool IsConnected()
        {
            return isConnected;
        }


        public List<string> GetPortsList()
        {
            return SerialPort.GetPortNames().ToList();
        }
        

        public void Connect(string portName, int baudRate)
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
                LogPortState($"Connected to port {portName}.");
                OnConnectedEvent?.Invoke();
            }
            catch (Exception ex)
            {
                LogPortState($"Failed to connect to port {portName}.");
                OnConnectingError?.Invoke(ex);
            }
        }





        public void SendMessage(string message)
        {
            if (serialPort == null || !isConnected)
            {
                LogPortState("Failed to write data. Port is not connected.");
                return;
            }

            LogMessage("TX: " + message.TrimEnd('\r', '\n'));

            try
            {
                serialPort.Write(message);
            }
            catch (Exception ex)
            {
                LogPortState($"Failed to write data. {ex.Message}");

                OnWritingError?.Invoke(ex);

                Disconnect();
            }
        }


        public void Disconnect()
        {
            if (isConnected || serialPort != null)
            {
                isConnected = false;

                LogPortState("Port disconnected.");

                serialPort?.Dispose();
                serialPort = null;

                OnDisconnectedEvent?.Invoke();
            }
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadExisting();
            InvokeDataRecievedEvents(data);
        }

        private void InvokeDataRecievedEvents(string receivedData)
        {

            string[] messages = receivedData.Split(new char[] { '\r', '\n' },
                             StringSplitOptions.RemoveEmptyEntries);

            foreach (var message in messages)
            {
                LogMessage("RX: " + message);

                OnDataReceivedEvent?.Invoke(message);
            }
        }



        private void LogMessage(string message)
        {
            OnLogMessage?.Invoke(message);
        }

        private void LogPortState(string message)
        {
            OnLogState?.Invoke(message);
        }
    }
}
