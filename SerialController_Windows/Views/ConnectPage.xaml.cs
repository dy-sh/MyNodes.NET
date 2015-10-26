/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SerialController.Windows;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SerialController.Windows.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConnectPage : Page
    {



        public ConnectPage()
        {
            this.InitializeComponent();

            RefrashInterface();

            App.serialPort.OnConnectedEvent += DeviceConnected;
            App.serialPort.OnDisconnectedEvent += DeviceDisconnected;

            if (!App.serialPort.IsConnected())
                ConnectToLast();
        }

        ~ConnectPage()
        {
            App.serialPort.OnConnectedEvent -= DeviceConnected;
            App.serialPort.OnDisconnectedEvent -= DeviceDisconnected;
        }


        public async void GetSerialList()
        {
            List<string> devices = await App.serialPort.GetDevicesList();

            listbox1.Items.Clear();

            foreach (var device in devices)
                listbox1.Items.Add(device);
        }



        private async void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (!App.serialPort.IsConnected())
            {

                int selection = listbox1.SelectedIndex;

                if (selection < 0) return;

                await App.serialPort.Connect(selection, 115200);

                if (!App.serialPort.IsConnected())
                {
                    ShowConnectionFailedDialog();
                    return;
                }

                Frame.Navigate(typeof (NodesControlPage));

            }
            else
            {
                App.serialPort.Disconnect();
            }
            //  RefrashInterface();
        }

        private void buttonRefrash_Click(object sender, RoutedEventArgs e)
        {
            GetSerialList();
        }

        private void RefrashInterface()
        {
            GetSerialList();

            if (!App.serialPort.IsConnected())
            {
                buttonConnect.Content = "Connect";
            }
            else
            {
                buttonConnect.Content = "Disconnect";
            }
        }

        public void DeviceConnected()
        {
            RefrashInterface();
        }

        public void DeviceDisconnected()
        {
            RefrashInterface();
        }


        private async void ShowConnectionFailedDialog()
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Connection failed";
            dialog.Content = "Try to select another device";
            dialog.PrimaryButtonText = "OK";
            await dialog.ShowAsync();
        }


        private async void ConnectToLast()
        {
            return;
            //todo connect to last device
            try
            {

                await Task.Delay(100);

                int selection = 0;

                await App.serialPort.Connect(selection, 115200);

                if (!App.serialPort.IsConnected())
                {
                    ShowConnectionFailedDialog();
                    return;
                }

                Frame.Navigate(typeof (NodesControlPage));
            }

            catch
            {

            }


        }
    }
}