/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Text.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SerialController_Windows.Code
{
    public delegate void ReceivedDataEventHandler(string message);

    public class SerialPort
    {
        private bool showDebugMessages = false;

        private bool isConnected;

        public event ReceivedDataEventHandler ReceivedDataEvent;
        public event EventHandler OnConnectedEvent;
        public event EventHandler OnDisconnectedEvent;

        private SerialDevice serialPort = null;
        DataWriter dataWriteObject = null;
        DataReader dataReaderObject = null;

        private ObservableCollection<DeviceInformation> listOfDevices;
        private CancellationTokenSource ReadCancellationTokenSource;



  


        public SerialPort()
        {
            listOfDevices = new ObservableCollection<DeviceInformation>();
            FindDevices();
        }

        public bool IsConnected()
        {
            return isConnected;
        }


        public async Task<List<string>> GetDevicesList()
        {

            await FindDevices();

            List<string> devicesList = new List<string>();

            if (listOfDevices != null)
                foreach (var device in listOfDevices)
                {
                    devicesList.Add(device.Name);
                }

            return devicesList;
        }


        public async Task FindDevices()
        {
            try
            {
                string selector = SerialDevice.GetDeviceSelector();
                var devices = await DeviceInformation.FindAllAsync(selector);

                listOfDevices.Clear();

                for (int i = 0; i < devices.Count; i++)
                {
                    listOfDevices.Add(devices[i]);
                }
            }
            catch (Exception ex)
            {

            }
        }


        public async Task Connect(int deviceIndex,uint baudRate=9600)
        {

            DeviceInformation entry = listOfDevices[deviceIndex];

            try
            {
                serialPort = await SerialDevice.FromIdAsync(entry.Id);


                // Configure serial settings
                serialPort.WriteTimeout = TimeSpan.FromMilliseconds(10);
                serialPort.ReadTimeout = TimeSpan.FromMilliseconds(10);
                serialPort.BaudRate = baudRate;
                serialPort.Parity = SerialParity.None;
                serialPort.StopBits = SerialStopBitCount.One;
                serialPort.DataBits = 8;

                isConnected = true;

                // Create cancellation token object to close I/O operations when closing the device
                ReadCancellationTokenSource = new CancellationTokenSource();
                StartReading();

                if (OnConnectedEvent != null) OnConnectedEvent(this,null);
            }
            catch (Exception ex)
            {

            }
        }


        public async Task SendMessage(string message)
        {
       
            try
            {
                if (serialPort != null)
                {
                    // Create the DataWriter object and attach to OutputStream
                    dataWriteObject = new DataWriter(serialPort.OutputStream);

                    //Launch the WriteAsync task to perform the write
                    await WriteAsync(message);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                // Cleanup once complete
                if (dataWriteObject != null)
                {
                    dataWriteObject.DetachStream();
                    dataWriteObject = null;
                }
            }
        }


        private async Task WriteAsync(string message)
        {
            Log("Writing to serial: "+ message);

            Task<UInt32> writeAsyncTask;

            if (!string.IsNullOrEmpty(message))
            {
                // Load the text from the sendText input text box to the dataWriter object
                dataWriteObject.WriteString(message);

                // Launch an async task to complete the write operation
                writeAsyncTask = dataWriteObject.StoreAsync().AsTask();

                UInt32 bytesWritten = await writeAsyncTask;
            }
        }


        private async void StartReading()
        {
            try
            {
                dataReaderObject = new DataReader(serialPort.InputStream);
                while (isConnected)
                {
                    if (serialPort != null)
                    {
                        await ReadAsync(ReadCancellationTokenSource.Token);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name == "TaskCanceledException")
                {
                    CloseDevice();
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                // Cleanup once complete
                if (dataReaderObject != null)
                {
                    dataReaderObject.DetachStream();
                    dataReaderObject = null;
                }
            }
        }



        private async Task ReadAsync(CancellationToken cancellationToken)
        {
            UInt32 bytesRead=0;
            try
            {
                Task<UInt32> loadAsyncTask;

                uint ReadBufferLength = 1024;

                // If task cancellation was requested, comply
                cancellationToken.ThrowIfCancellationRequested();

                // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
                dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

                // Create a task object to wait for data on the serialPort.InputStream
                loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask(cancellationToken);

                // Launch the task and wait
                bytesRead = await loadAsyncTask;
            }
            catch { }

            if (bytesRead > 0)
            {
                SendDataRecievedEvents(dataReaderObject.ReadString(bytesRead));
            }
           

        }

        /// <summary>
        /// CancelReadTask:
        /// - Uses the ReadCancellationTokenSource to cancel read operations
        /// </summary>
        private void CancelReadTask()
        {
            if (ReadCancellationTokenSource != null)
            {
                if (!ReadCancellationTokenSource.IsCancellationRequested)
                {
                    ReadCancellationTokenSource.Cancel();
                }
            }
        }


        private void CloseDevice()
        {
            isConnected = false;
            if (OnDisconnectedEvent != null) OnDisconnectedEvent(this, null);

            if (serialPort != null)
            {
                serialPort.Dispose();
            }
            serialPort = null;
        }


        public void Disconnect()
        {
            try
            {
                CancelReadTask();
                CloseDevice();
            }
            catch (Exception ex)
            {

            }
        }


        private void SendDataRecievedEvents(string receivedText)
        {

            string[] messages = receivedText.Split(new char[] { '\r', '\n' },
                             StringSplitOptions.RemoveEmptyEntries);

            foreach (var message in messages)
            {
                Log("Readed from serial: " + message);


                if (ReceivedDataEvent != null)
                    ReceivedDataEvent(message);
            }
        }

        public void Log(string message)
        {
            if (showDebugMessages)
                Debug.WriteLine(message);
        }
    }
}
