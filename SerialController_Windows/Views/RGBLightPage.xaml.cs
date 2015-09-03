using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SerialController_Windows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RGBLightPage : Page
    {
        private uint stripChangeColorTime = 1000;
        private uint stripEnableTime = 2000;


        public bool sendControls;


        public RGBLightPage()
        {
            this.InitializeComponent();

            if (App.ledStripController!=null)
            App.ledStripController.stateRecievedEvent += UpdateSliders;

            textBlock3.Visibility = Visibility.Visible;
            Panel1.Visibility = Visibility.Collapsed;


            if (App.serialPort.IsConnected())
            {
                textBlock3.Text = "Reading data from device...";
                App.ledStripController.GetState();
            }
            else
            {
                textBlock3.Text = "Device is not connected";
            }
        }

        ~RGBLightPage()
        {
            App.ledStripController.stateRecievedEvent -= UpdateSliders;
        }


        private void toggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!sendControls) return;

            App.ledStripController.TurnOnOff(toggleSwitch1.IsOn, stripEnableTime);
        }



        private void slider1_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!sendControls) return;

            App.ledStripController.Fade(
                (uint)slider1.Value,
                (uint)slider2.Value,
                (uint)slider3.Value,
                stripChangeColorTime);
        }

        private void UpdateSliders(uint r,uint g,uint b, bool isOn)
        {
            sendControls = false;

            slider1.Value = r;
            slider2.Value = g;
            slider3.Value = b;
            toggleSwitch1.IsOn = isOn;

            textBlock3.Visibility = Visibility.Collapsed;
            Panel1.Visibility = Visibility.Visible;

            sendControls = true;

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            App.ledStripController.StorePreset();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (RGBLightAdvancedPage));
        }
    }
}
