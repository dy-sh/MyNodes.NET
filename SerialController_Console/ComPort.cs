/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using MyNetSensors.SerialGateway;

namespace MyNetSensors.SerialController_Console
{

    public class ComPort : IComPort
    {
        public event OnDataReceivedEventHandler OnDataReceivedEvent;
        public event EventHandler OnConnectedEvent;
        public event EventHandler OnDisconnectedEvent;
        public event ExceptionEventHandler OnWritingError;
        public event ExceptionEventHandler OnConnectingError;
        public event DebugMessageEventHandler OnDebugTxRxMessage;
        public event DebugMessageEventHandler OnDebugPortStateMessage;

        private bool isConnected;
        private SerialPort serialPort;
        private List<string> portsList = new List<string>();


        public bool IsConnected()
        {
            return isConnected;
        }


        public List<string> GetPortsList()
        {
            FindDevices();

            return portsList;
        }


        public void FindDevices()
        {
            portsList.Clear();

            foreach (string portName in SerialPort.GetPortNames())
            {
                portsList.Add(portName);
            }

        }


        public void Connect(int portIndex, int baudRate = 115200)
        {
            string portName = portsList[portIndex];
            Connect(portName, baudRate);
        }

        public void Connect(string portName, int baudRate = 115200)
        {

            try
            {
                serialPort = new SerialPort(portName);

                serialPort.BaudRate = baudRate;
                serialPort.Parity = Parity.None;
                serialPort.StopBits = StopBits.One;
                serialPort.DataBits = 8;
                serialPort.DataReceived += serialPort_DataReceived;

                serialPort.Open();

                isConnected = true;

                DebugPortState(String.Format("Connected to port {0}.", portName));

                if (OnConnectedEvent != null)
                    OnConnectedEvent(this,null);
            }
            catch (Exception ex)
            {
                DebugPortState(String.Format("Can`t connect to port {0}.", portName));

                if (OnConnectingError != null)
                    OnConnectingError(ex);
            }
        }





        public void SendMessage(string message)
        {
            if (serialPort == null || !isConnected)
            {
                DebugPortState("Can`t writing to serial. Port closed.");
                return;
            }

            DebugTxRx("TX: " + message.TrimEnd('\r', '\n'));

            try
            {
                serialPort.Write(message);
            }
            catch (Exception ex)
            {
                DebugPortState(String.Format("Can`t writing to serial. {0}", ex.Message));

                if (OnWritingError != null)
                    OnWritingError(ex);

                Disconnect();
            }
        }


        public void Disconnect()
        {
            isConnected = false;

            DebugPortState(String.Format("Port disconnected."));

            if (serialPort != null)
            {
                serialPort.Dispose();
            }
            serialPort = null;

            if (OnDisconnectedEvent != null)
                OnDisconnectedEvent(this, null);
        }


        private void SendDataRecievedEvents(string receivedData)
        {

            string[] messages = receivedData.Split(new char[] { '\r', '\n' },
                             StringSplitOptions.RemoveEmptyEntries);

            foreach (var message in messages)
            {
                DebugTxRx("RX: " + message);

                if (OnDataReceivedEvent != null)
                    OnDataReceivedEvent(message);
            }
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadExisting();
            SendDataRecievedEvents(data);
        }

        private void DebugTxRx(string message)
        {
            if (OnDebugTxRxMessage != null)
                OnDebugTxRxMessage(message);
        }

        private void DebugPortState(string message)
        {
            if (OnDebugPortStateMessage != null)
                OnDebugPortStateMessage(message);
        }
    }
}
