/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;

namespace MyNetSensors.Gateways
{
    public delegate void OnDataReceivedEventHandler(string message);

    public interface IComPort
    {
        event OnDataReceivedEventHandler OnDataReceivedEvent;
        event Action OnConnectedEvent;
        event Action OnDisconnectedEvent;

        List<string> GetPortsList();
        void Connect(string portName, int baudRate);
        void Disconnect();
        void SendMessage(string message);
        bool IsConnected();
    }
}
