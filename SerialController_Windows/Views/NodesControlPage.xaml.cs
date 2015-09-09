using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
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
        //store node id when for which is sending, to prevent slidres update when drug from event
        private int lastSendedNodeId;
        private int lastSendedSensorId;

        private DispatcherTimer refrashTimer;


        public NodesControlPage()
        {
            this.InitializeComponent();

            App.serialController.OnNewNodeEvent += AddNode;
            App.serialController.OnNodeUpdatedEvent += UpdateNode;
            App.serialController.OnSensorUpdatedEvent += UpdateSensor;
            App.serialController.OnClearNodesList += OnClearNodesList;


            if (App.serialController.IsConnected())
            {
                textBlock3.Visibility = Visibility.Collapsed;
                panel1.Visibility = Visibility.Visible;
                ShowNodes();
            }
            else
            {
                textBlock3.Text = "Device is not connected";
                textBlock3.Visibility = Visibility.Visible;
                panel1.Visibility = Visibility.Collapsed;
            }

            refrashTimer = new DispatcherTimer();
            refrashTimer.Interval = TimeSpan.FromMilliseconds(100);
            refrashTimer.Tick += RefrashTimer;
            refrashTimer.Start();

        }

        private void RefrashTimer(object sender, object e)
        {
            ShowNodes();
        }

        private void OnClearNodesList(object sender, EventArgs e)
        {
            ShowNodes();
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
            titlePanel.Height = 30;

            Grid titleGrid = CreateNodeTitle(node);
            titlePanel.Children.Add(titleGrid);

            nodePanel.Children.Add(titlePanel);

            if (node.name != null)
                nodePanel.Children.Add(new TextBlock { Text = node.name, Margin = new Thickness(5) });



            foreach (Sensor sensor in node.sensors)
            {
                StackPanel sensorPanel = CreateSensor(sensor);
                nodePanel.Children.Add(sensorPanel);
            }

            if (node.batteryLevel != null)
                nodePanel.Children.Add(new TextBlock { Text = "Battery " + node.batteryLevel.Value, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Gray) });

            return nodePanel;

        }

        List<Button> titleButtons = new List<Button>();
        private Grid CreateNodeTitle(Node node)
        {
            Button button = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
                Content = new SymbolIcon { Symbol = Symbol.More },
                Height = 30
                // Margin = new Thickness(5)
            };

            button.Tag = node.nodeId;
            button.Click += buttonNodeMenu_Click;

    
            float lastSeen = (float)DateTime.Now.Subtract(node.lastSeen).TotalSeconds;
            if (lastSeen < 5)
            {
                int color = 255-(int)MathUtils.Map(lastSeen, 0, 5, 0, 255);
                button.Foreground = new SolidColorBrush(Color.FromArgb(255, (byte)color, 0, 0));
            }

            titleButtons.Add(button);

            /*
             MenuFlyout menu = new MenuFlyout();
             MenuFlyoutItem item1 = new MenuFlyoutItem {Text = "Reboot Node"};
             item1.Click += buttonNodeMenu_Click;
             menu.Items.Add(item1);
             menu.Items[0].Tag = node.nodeId;

             menu.Items.Add(new MenuFlyoutItem { Text = "Delete Node" });
             MenuFlyoutItem n=new MenuFlyoutItem();
             button.Flyout = menu;
             */

            Grid grid = new Grid
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            grid.Children.Add(new TextBlock
            {
                Text = "Node " + node.nodeId,
                Margin = new Thickness(5),
                FontWeight = FontWeights.SemiBold,
            });


            grid.Children.Add(button);


            return grid;
        }

        private async void buttonNodeMenu_Click(object sender, RoutedEventArgs e)
        {
            int nodeId = (int)((ButtonBase) sender).Tag;


            var newCoreAppView = CoreApplication.CreateNewView();
            var appView = ApplicationView.GetForCurrentView();
            await newCoreAppView.Dispatcher.RunAsync(CoreDispatcherPriority.Low, async() =>
            {
                var window = Window.Current;
                var newAppView = ApplicationView.GetForCurrentView();

#if WINDOWS_UAP
                newAppView.SetPreferredMinSize(new Windows.Foundation.Size(400, 300));
#endif
                var frame = new Frame();
                window.Content = frame;
                frame.Navigate(typeof(NodePage), nodeId);
                window.Activate();

                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newAppView.Id, ViewSizePreference.UseMore, appView.Id, ViewSizePreference.Default);

#if WINDOWS_UAP
                var success = newAppView.TryResizeView(new Windows.Foundation.Size(400, 400));
#endif
            });
        }


        private StackPanel CreateSensor(Sensor sensor)
        {
            StackPanel sensorPanel = new StackPanel();

            sensorPanel.Orientation = Orientation.Vertical;
            sensorPanel.Margin = new Thickness(5);
            sensorPanel.BorderThickness = new Thickness(1);
            sensorPanel.BorderBrush = new SolidColorBrush(Colors.Black);
            //        sensorPanel.Background = new SolidColorBrush(Colors.Gray);



            string sType = MySensors.GetSimpleSensorType(sensor.GetSensorType());

            if (sensor.description != null)
                sType = String.Format("{0} {1}", sType, sensor.description);

            sensorPanel.Children.Add(new TextBlock { Text = sType, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Gray) });



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
                dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Black) });
            }

            //TEMPERATURE
            else if (data.dataType == SensorDataType.V_TEMP)
            {
                string s = String.Format("{0} C", data.state);
                dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Black) });
            }

            //WATT
            else if (data.dataType == SensorDataType.V_WATT)
            {
                string s = String.Format("{0} Watt", data.state);
                dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Black) });
            }

            //KWH
            else if (data.dataType == SensorDataType.V_KWH)
            {
                string s = String.Format("{0} KWH", data.state);
                dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Black) });
            }

            //Volt
            else if (data.dataType == SensorDataType.V_VOLTAGE)
            {
                string s = String.Format("{0} V", data.state);
                dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Black) });
            }

            //Current
            else if (data.dataType == SensorDataType.V_CURRENT)
            {
                string s = String.Format("{0} ma", data.state);
                dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Black) });
            }

            //Armed/Bypassed
            else if (data.dataType == SensorDataType.V_ARMED)
            {
                string s = data.state == "1" ? "Armed" : "Bypassed";
                dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Black) });
            }

            //Locked/Unlocked
            else if (data.dataType == SensorDataType.V_LOCK_STATUS)
            {
                string s = data.state == "1" ? "Locked" : "Unlocked";
                dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Black) });
            }

            //SIMPLE TEXT
            else
            {
                string s = String.Format("{0}", data.state);
                dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Black) });

            }

            return dataPanel;
        }

        private ToggleButton CreateButton(Sensor sensor, SensorData data)
        {
            ToggleButton button = new ToggleButton();
            button.Tag = String.Format("{0};{1};{2}",
                sensor.ownerNodeId,
                sensor.sensorId,
                data.dataType.ToString());

            button.HorizontalAlignment = HorizontalAlignment.Stretch;
            button.Margin = new Thickness(5);

            button.Click += button_Click;
            button.IsChecked = data.state == "1";

            button.Content = (button.IsChecked.Value) ? "ON" : "OFF";

            return button;
        }

        private Slider CreateSlider(Sensor sensor, SensorData data)
        {
            Slider slider = new Slider();
            slider.Tag = String.Format("{0};{1};{2}",
                sensor.ownerNodeId,
                sensor.sensorId,
                data.dataType.ToString());

            slider.HorizontalAlignment = HorizontalAlignment.Stretch;
            slider.Margin = new Thickness(5, 0, 5, 0);

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
                    sensor.ownerNodeId,
                    sensor.sensorId,
                    data.dataType.ToString(),
                    i);

                slider.HorizontalAlignment = HorizontalAlignment.Stretch;
                slider.Margin = new Thickness(5, 0, 5, 0);

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
                    sensor.ownerNodeId,
                    sensor.sensorId,
                    data.dataType.ToString(),
                    i);

                slider.HorizontalAlignment = HorizontalAlignment.Stretch;
                slider.Margin = new Thickness(5, 0, 5, 0);

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
            titleButtons.Clear();

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
            //prevent slidres update when drug
            if (node.NodeId==lastSendedNodeId 
                && node.sensorId==lastSendedSensorId)
                return;
            
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

            SendSensorState(nodeId, sensorId, data);
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

            SendSensorState(nodeId, sensorId, data);
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

            SendSensorState(nodeId, sensorId, data);
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


            SendSensorState(nodeId, sensorId, data);
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            App.serialController.SendRebootToAllNodes();
        }

        private async void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var dialog = new ContentDialog()
            {
                Title = "Delete nodes",
                //RequestedTheme = ElementTheme.Dark,
                //FullSizeDesired = true,
                MaxWidth = this.ActualWidth // Required for Mobile!
            };

            // Setup Content
            var panel = new StackPanel();

            panel.Children.Add(new TextBlock
            {
                Text = "\r\n"
                + "Deleting nodes from the database will release their ID. "
                + "After that, you will need to turn on all "
                + "previously registered nodes to reinitialize the database. "
                + "If this is not done, any new node will take the ID "
                + "of the other device, and there is a conflict! \r\n\r\n"
                + "If there is a conflict, you should: "
                + "1. Turn off all nodes. "
                + "2. Delete all nodes from the database. "
                + "3. Erase EEPROM on conflicty nodes and reflash again firmwares. "
                + "4. Turn on all non-conflicting nodes. "
                + "5. Turn on conflicty nodes.\r\n\r\n"
                + "Again. After deleting nodes from the database, "
                + "do not run the new nodes (which has not received the ID previously) "
                + "before turn on all known nodes! ",
                TextWrapping = TextWrapping.Wrap,
            });

            dialog.Content = panel;

            // Add Buttons
            dialog.PrimaryButtonText = "OK";
            dialog.PrimaryButtonClick += delegate
            {
                App.serialController.ClearNodesList();
            };

            dialog.SecondaryButtonText = "Cancel";


            // Show Dialog
            var result = await dialog.ShowAsync();
        }

        private void SendSensorState(int nodeId, int sensorId, SensorData data)
        {
            lastSendedNodeId = nodeId;
            lastSendedSensorId = sensorId;
            App.serialController.SendSensorState(nodeId, sensorId, data);
            lastSendedNodeId = -1;
            lastSendedSensorId = -1;
        }
    }

}
