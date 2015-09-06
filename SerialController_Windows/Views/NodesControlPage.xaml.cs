using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SerialController_Windows.Code;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SerialController_Windows.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NodesControlPage : Page
    {


        public NodesControlPage()
        {
            this.InitializeComponent();

            App.serialController.OnNewNodeEvent += AddNode;
            App.serialController.OnNodeUpdatedEvent += UpdateNode;
            App.serialController.OnSensorUpdatedEvent += UpdateSensor;


            if (App.serialController.IsConnected())
            {
                textBlock3.Visibility = Visibility.Collapsed;
                itemsControl1.Visibility = Visibility.Visible;
                ShowNodes();
            }
            else
            {
                textBlock3.Text = "Device is not connected";
                textBlock3.Visibility = Visibility.Visible;
                itemsControl1.Visibility = Visibility.Collapsed;
            }

        }


        private StackPanel CreateNode(Node node)
        {
            StackPanel nodePanel = new StackPanel();
            nodePanel.MinWidth = 200;
            nodePanel.Orientation = Orientation.Vertical;
            nodePanel.Margin = new Thickness(10);
            nodePanel.BorderThickness = new Thickness(1);
            nodePanel.BorderBrush = new SolidColorBrush(Colors.Black);
            //          nodePanel.Background = new SolidColorBrush(Colors.Gray);

            StackPanel titlePanel = new StackPanel();
            titlePanel.Background = new SolidColorBrush(Colors.Gainsboro);
            titlePanel.Height = 40;
            titlePanel.Children.Add(new TextBlock { Text = "Node " + node.nodeId, Margin = new Thickness(10), FontWeight = FontWeights.SemiBold });

            nodePanel.Children.Add(titlePanel);

            if (node.name != null)
                nodePanel.Children.Add(new TextBlock { Text = node.name, Margin = new Thickness(10) });



            foreach (Sensor sensor in node.sensors)
            {
                StackPanel sensorPanel = CreateSensor(sensor);
                nodePanel.Children.Add(sensorPanel);
            }

            if (node.batteryLevel != null)
                nodePanel.Children.Add(new TextBlock { Text = "Battery " + node.batteryLevel.Value, Margin = new Thickness(10), Foreground = new SolidColorBrush(Colors.Gray) });

            return nodePanel;

        }

        private StackPanel CreateSensor(Sensor sensor)
        {
            StackPanel sensorPanel = new StackPanel();

            sensorPanel.Orientation = Orientation.Vertical;
            sensorPanel.Margin = new Thickness(10);
            sensorPanel.BorderThickness = new Thickness(1);
            sensorPanel.BorderBrush = new SolidColorBrush(Colors.Black);
            //        sensorPanel.Background = new SolidColorBrush(Colors.Gray);



            string sType = MySensors.GetSimpleSensorType(sensor.GetSensorType());

            if (sensor.description != null)
                sType = String.Format("{0} [{1}]", sType, sensor.description);

            sensorPanel.Children.Add(new TextBlock { Text = sType, Margin = new Thickness(10), Foreground = new SolidColorBrush(Colors.Gray) });



            foreach (SensorData data in sensor.sensorData)
            {
                StackPanel dataPanel = CreateSensorData(data, sensor);
                sensorPanel.Children.Add(dataPanel);
            }
            return sensorPanel;
        }




        private StackPanel CreateSensorData(SensorData data, Sensor sensor)
        {
            StackPanel dataPanel = new StackPanel();

            //ON-OFF BUTTON
            if (data.dataType == SensorDataType.V_TRIPPED
                || data.dataType == SensorDataType.V_STATUS
                || data.dataType == SensorDataType.V_LIGHT)
            {
                ToggleButton button = CreateButton(sensor, data);
                dataPanel.Children.Add(button);
            }

            //0-100% SLIDER
            else if (data.dataType == SensorDataType.V_PERCENTAGE
                || data.dataType == SensorDataType.V_DIMMER
                || data.dataType == SensorDataType.V_LIGHT_LEVEL)
            {
                Slider slider = CreateSlider(sensor, data);
                dataPanel.Children.Add(slider);
            }
             
            //RGB/RGBW SLIDERS
            else if (data.dataType == SensorDataType.V_RGB)
            {
                List<Slider> sliders = CreateRGBSliders(sensor, data);
                foreach (var slider in sliders)
                    dataPanel.Children.Add(slider);
            }
            else if (data.dataType == SensorDataType.V_RGBW)
            {
                List<Slider> sliders = CreateRGBWSliders(sensor, data);
                foreach (var slider in sliders)
                    dataPanel.Children.Add(slider);
            }

            //HUMIDITY
            else if (data.dataType == SensorDataType.V_HUM)
            {
                string s = String.Format("{0} %", data.state);
                dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(10), Foreground = new SolidColorBrush(Colors.Gray) });
            }

            //TEMPERATURE
            else if (data.dataType == SensorDataType.V_TEMP)
            {
                string s = String.Format("{0} C", data.state);
                dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(10), Foreground = new SolidColorBrush(Colors.Gray) });
            }

            //WATT
            else if (data.dataType == SensorDataType.V_WATT)
            {
                string s = String.Format("{0} Watt", data.state);
                dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(10), Foreground = new SolidColorBrush(Colors.Gray) });
            }

            //KWH
            else if (data.dataType == SensorDataType.V_KWH)
            {
                string s = String.Format("{0} KWH", data.state);
                dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(10), Foreground = new SolidColorBrush(Colors.Gray) });
            }

            //Volt
            else if (data.dataType == SensorDataType.V_VOLTAGE)
            {
                string s = String.Format("{0} V", data.state);
                dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(10), Foreground = new SolidColorBrush(Colors.Gray) });
            }

            //Current
            else if (data.dataType == SensorDataType.V_CURRENT)
            {
                string s = String.Format("{0} ma", data.state);
                dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(10), Foreground = new SolidColorBrush(Colors.Gray) });
            }

            //Armed/Bypassed
            else if (data.dataType == SensorDataType.V_ARMED)
            {
                string s = data.state == "1" ? "Armed" : "Bypassed";
                dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(10), Foreground = new SolidColorBrush(Colors.Gray) });
           }

            //Locked/Unlocked
            else if (data.dataType == SensorDataType.V_LOCK_STATUS)
            {
                string s = data.state == "1" ? "Locked" : "Unlocked";
                dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(10), Foreground = new SolidColorBrush(Colors.Gray) });
            }

            //SIMPLE TEXT
            else
            {
                string s = String.Format("{0}", data.state);
                dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(10), Foreground = new SolidColorBrush(Colors.Gray) });

            }

            return dataPanel;
        }

        private ToggleButton CreateButton(Sensor sensor, SensorData data)
        {
            ToggleButton button = new ToggleButton();
            button.Tag = String.Format("{0};{1};{2}",
                sensor.ownerNode.nodeId,
                sensor.sensorId,
                data.dataType.ToString());

            button.HorizontalAlignment = HorizontalAlignment.Stretch;
            button.Margin = new Thickness(10);

            button.Click += button_Click;
            button.IsChecked = data.state == "1";

            button.Content = (button.IsChecked.Value) ? "ON" : "OFF";

            return button;
        }

        private Slider CreateSlider(Sensor sensor, SensorData data)
        {
            Slider slider = new Slider();
            slider.Tag = String.Format("{0};{1};{2}",
                sensor.ownerNode.nodeId,
                sensor.sensorId,
                data.dataType.ToString());

            slider.HorizontalAlignment = HorizontalAlignment.Stretch;
            slider.Margin = new Thickness(10, 0, 10, 0);

            slider.Maximum = 100;
            slider.Value = Int32.Parse(data.state);
            slider.ValueChanged += slider_ValueChanged;

            return slider;
        }

        private List<Slider> CreateRGBSliders(Sensor sensor, SensorData data)
        {
            List<Slider> list = new List<Slider>();

            int[] rgb = ColorUtils.ConvertRGBHexStringToIntArray(data.state);

            for (int i = 0; i < 3; i++)
            {
                Slider slider = new Slider();
                slider.Tag = String.Format("{0};{1};{2};{3}",
                    sensor.ownerNode.nodeId,
                    sensor.sensorId,
                    data.dataType.ToString(),
                    i);

                slider.HorizontalAlignment = HorizontalAlignment.Stretch;
                slider.Margin = new Thickness(10, 0, 10, 0);

                slider.Maximum = 255;
                slider.Value = rgb[i];
                slider.ValueChanged += sliderRGB_ValueChanged;

                list.Add(slider);
            }
            return list;
        }

        private List<Slider> CreateRGBWSliders(Sensor sensor, SensorData data)
        {
            List<Slider> list = new List<Slider>();

            int[] rgbw = ColorUtils.ConvertRGBWHexStringToIntArray(data.state);

            for (int i = 0; i < 4; i++)
            {
                Slider slider = new Slider();
                slider.Tag = String.Format("{0};{1};{2};{3}",
                    sensor.ownerNode.nodeId,
                    sensor.sensorId,
                    data.dataType.ToString(),
                    i);

                slider.HorizontalAlignment = HorizontalAlignment.Stretch;
                slider.Margin = new Thickness(10, 0, 10, 0);

                slider.Maximum = 255;
                slider.Value = rgbw[i];
                slider.ValueChanged += sliderRGBW_ValueChanged;

                list.Add(slider);
            }
            return list;
        }


        private void ShowNodes()
        {
            itemsControl1.Items.Clear();

            List<Node> nodes = App.serialController.GetNodes();

            foreach (var node in nodes)
            {
                AddNode(node);
            }

        }

        private void AddNode(Node node)
        {
            StackPanel nodePanel = CreateNode(node);

            itemsControl1.Items.Add(nodePanel);


        }

        private void UpdateNode(Node node)
        {
            ShowNodes();
        }

        private void UpdateSensor(Sensor node)
        {
            ShowNodes();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (ToggleButton)sender;
            string tag = button.Tag.ToString();
            string[] args = tag.Split(';');
            int nodeId = Int32.Parse(args[0]);
            int sensorId = Int32.Parse(args[1]);
            SensorDataType dataType;
            SensorDataType.TryParse(args[2], true, out dataType);
            string state = (button.IsChecked.Value) ? "1" : "0";
            SensorData data = new SensorData(dataType, state);

            button.Content = (button.IsChecked.Value) ? "ON" : "OFF";

            App.serialController.ChangeSensorState(nodeId, sensorId, data);
        }

        private void slider_ValueChanged(object sender, RoutedEventArgs e)
        {
            Slider slider = (Slider)sender;
            string tag = slider.Tag.ToString();
            string[] args = tag.Split(';');
            int nodeId = Int32.Parse(args[0]);
            int sensorId = Int32.Parse(args[1]);
            SensorDataType dataType;
            SensorDataType.TryParse(args[2], true, out dataType);
            string state = slider.Value.ToString();
            SensorData data = new SensorData(dataType, state);

            App.serialController.ChangeSensorState(nodeId, sensorId, data);
        }

        private void sliderRGB_ValueChanged(object sender, RoutedEventArgs e)
        {
            Slider slider = (Slider)sender;
            string tag = slider.Tag.ToString();
            string[] args = tag.Split(';');

            int nodeId = Int32.Parse(args[0]);
            int sensorId = Int32.Parse(args[1]);
            SensorDataType dataType;
            SensorDataType.TryParse(args[2], true, out dataType);
            int sliderIndex = Int32.Parse(args[3]);

            SensorData data = App.serialController
                .GetNode(nodeId)
                .GetSensor(sensorId)
                .GetData(dataType);

            int[] rgb = ColorUtils.ConvertRGBHexStringToIntArray(data.state);
            rgb[sliderIndex] = (int)slider.Value;
            data.state = ColorUtils.ConvertRGBIntArrayToHexString(rgb);


            App.serialController.ChangeSensorState(nodeId, sensorId, data);
        }

        private void sliderRGBW_ValueChanged(object sender, RoutedEventArgs e)
        {
            Slider slider = (Slider)sender;
            string tag = slider.Tag.ToString();
            string[] args = tag.Split(';');

            int nodeId = Int32.Parse(args[0]);
            int sensorId = Int32.Parse(args[1]);
            SensorDataType dataType;
            SensorDataType.TryParse(args[2], true, out dataType);
            int sliderIndex = Int32.Parse(args[3]);

            SensorData data = App.serialController
                .GetNode(nodeId)
                .GetSensor(sensorId)
                .GetData(dataType);

            int[] rgbw = ColorUtils.ConvertRGBWHexStringToIntArray(data.state);
            rgbw[sliderIndex] = (int)slider.Value;
            data.state = ColorUtils.ConvertRGBWIntArrayToHexString(rgbw);


            App.serialController.ChangeSensorState(nodeId, sensorId, data);
        }
    }

}
