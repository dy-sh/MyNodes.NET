using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SerialController_Windows.Code;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SerialController_Windows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RGBRemotePage : Page
    {
        public float calibrateR = 1;
        public float calibrateG = 0.5f;
        public float calibrateB = 0.3f;






        public RGBRemotePage()
        {
            this.InitializeComponent();

            if (App.remoteColorClient == null)
            {
                App.remoteColorClient = new RemoteColorClient(InputAddress.Text);
            }
            App.remoteColorClient.colorRecievedEvent += ColorRecieved;
            App.remoteColorClient.onConnectedEvent += OnConnected;
            App.remoteColorClient.onDisconnectedEvent += OnDisconnected;
            RefrashInterface();
        }



        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!App.remoteColorClient.IsRunning())
            {
                App.remoteColorClient.SetUrl(InputAddress.Text);
                App.remoteColorClient.StartService();
            }
            else
            {
                App.remoteColorClient.StopService();
            }

            RefrashInterface();
        }

        private void RefrashInterface()
        {
            if (!App.remoteColorClient.IsRunning())
            {
                StartButton.Content = "Start";
            }
            else
            {
                StartButton.Content = "Stop";
            }
        }

        public void ColorRecieved(uint r, uint g, uint b)
        {
            Color color = new Color();

            color.R = (byte)r;
            color.G = (byte)g;
            color.B = (byte)b;
            color.A = 255;

            SolidColorBrush brush = new SolidColorBrush(color);
            ColorRect.Fill = brush;

            //calibration
            r = (uint)(r * calibrateR);
            g = (uint)(g * calibrateG);
            b = (uint)(b * calibrateB);

            App.ledStripController.SetColor(r, g, b);
        }

        public void OnConnected(object sender, object e)
        {
            StatusText.Text = "Connected";
        }

        public void OnDisconnected(object sender, object e)
        {
            StatusText.Text = "Disconnected";
        }

        public void SetCalibration(float calibrateR, float calibrateG, float calibrateB)
        {
            this.calibrateR = calibrateR;
            this.calibrateG = calibrateG;
            this.calibrateB = calibrateB;
        }
    }
}
