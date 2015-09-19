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

    public class ComPort: IComPort
    {
        private bool showDebugMessages = false;
        private bool showConsoleMessages = false;

        public event OnDataReceivedEventHandler OnDataReceivedEvent;
        public event EventHandler OnConnectedEvent;
        public event EventHandler OnDisconnectedEvent;

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

            }
            catch (Exception ex)
            {

            }
        }





        public void SendMessage(string message)
        {
            Log("Writing to serial: " + message);

            try
            {
                serialPort.Write(message);
            }
            catch (Exception ex)
            {

            }
        }


        public void Disconnect()
        {
            isConnected = false;
            if (OnDisconnectedEvent != null) OnDisconnectedEvent(this, null);

            if (serialPort != null)
            {
                serialPort.Dispose();
            }
            serialPort = null;
        }


        private void SendDataRecievedEvents(string receivedData)
        {

            string[] messages = receivedData.Split(new char[] { '\r', '\n' },
                             StringSplitOptions.RemoveEmptyEntries);

            foreach (var message in messages)
            {
                Log("Readed from serial: " + message);


                if (OnDataReceivedEvent != null)
                    OnDataReceivedEvent(message);
            }
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadExisting();
            SendDataRecievedEvents(data);
        }

        public void Log(string message)
        {
            if (showDebugMessages)
                Debug.WriteLine(message);
            if (showConsoleMessages)
                Console.WriteLine(message);
        }
    }
}
