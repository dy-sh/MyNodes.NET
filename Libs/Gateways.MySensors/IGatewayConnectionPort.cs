/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.Gateways.MySensors
{
    public delegate void OnDataReceivedEventHandler(string message);

    public interface IGatewayConnectionPort
    {
        event OnDataReceivedEventHandler OnDataReceived;
        event Action OnConnected;
        event Action OnDisconnected;
        event ExceptionEventHandler OnWritingError;
        event ExceptionEventHandler OnConnectingError;
        event LogEventHandler OnLogMessage;
        event LogEventHandler OnLogInfo;
        event LogEventHandler OnLogError;

        void Connect();
        void Disconnect();
        void SendMessage(string message);
        bool IsConnected();
    }
}
