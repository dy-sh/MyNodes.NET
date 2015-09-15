/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

        private DispatcherTimer interfaceRefrashTimer;
        private DispatcherTimer delayedSendTimer;

        List<SymbolIcon> activityIcons = new List<SymbolIcon>();
        List<TextBlock> batteryPanels = new List<TextBlock>();
        List<StackPanel> nodePanels = new List<StackPanel>();
        List<StackPanel> sensorPanels = new List<StackPanel>();

        //Queue list of sensors to sending
        List<Sensor> sendSensorsList = new List<Sensor>();


        public NodesControlPage()
        {
            this.InitializeComponent();

            App.serialController.OnNewNodeEvent += AddNode;
            App.serialController.OnNewSensorEvent += AddSensor;
            App.serialController.OnNodeUpdatedEvent += UpdateNode;
            App.serialController.OnSensorUpdatedEvent += UpdateSensor;
            App.serialController.OnNodeBatteryUpdatedEvent += UpdateBattery;
            App.serialController.OnClearNodesList += OnClearNodesList;


            if (App.serialController.IsConnected())
            {
                textBlock3.Visibility = Visibility.Collapsed;
                panel1.Visibility = Visibility.Visible;
                RedrawAllNodes();
            }
            else
            {
                textBlock3.Text = "Device is not connected";
                textBlock3.Visibility = Visibility.Visible;
                panel1.Visibility = Visibility.Collapsed;
            }

            interfaceRefrashTimer = new DispatcherTimer();
            interfaceRefrashTimer.Interval = TimeSpan.FromMilliseconds(50);
            interfaceRefrashTimer.Tick += InterfaceRefrashTimer;
            interfaceRefrashTimer.Start();

            delayedSendTimer = new DispatcherTimer();
            delayedSendTimer.Interval = TimeSpan.FromMilliseconds(10);
            delayedSendTimer.Tick += DelayedSendTimer;
            delayedSendTimer.Start();

        }



        private void AddSensor(Sensor sensor)
        {
            StackPanel oldPanel = GetSensorPanel(sensor.ownerNodeId, sensor.sensorId);
            if (oldPanel != null)
                sensorPanels.Remove(oldPanel);

            StackPanel nodePanel = GetNodePanel(sensor.ownerNodeId);
            StackPanel sensorPanel = CreateSensorPanel(sensor);
            nodePanel.Children.Add(sensorPanel);
            sensorPanels.Add(sensorPanel);
        }

        private void UpdateBattery(Node node)
        {
            TextBlock text = GetBatteryPanel(node.nodeId);
            if (text == null)
            {
                TextBlock oldPanel = GetBatteryPanel(node.nodeId);
                if (oldPanel != null)
                    batteryPanels.Remove(oldPanel);

                text = CreateBatteryPanel(node);
                StackPanel nodePanel = GetNodePanel(node.nodeId);
                nodePanel.Children.Add(text);
                batteryPanels.Add(text);
            }

            text.Text = "Battery " + node.batteryLevel.Value;
        }


        private void InterfaceRefrashTimer(object sender, object e)
        {
            foreach (var button in activityIcons)
            {
                int nodeId = (int)button.Tag;
                Node node = App.serialController.GetNode(nodeId);
                float lastSeen = (float)DateTime.Now.Subtract(node.lastSeen).TotalSeconds;
                if (lastSeen < 1)
                {
                    int color = (int)MathUtils.Map(lastSeen, 0, 1, 50, 220);
                    button.Foreground = new SolidColorBrush(Color.FromArgb(255, (byte)color, (byte)color, (byte)color));
                }
                else button.Foreground = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));

            }
        }

        private void OnClearNodesList(object sender, EventArgs e)
        {
            sendSensorsList.Clear();
            RedrawAllNodes();
        }


        private StackPanel CreateNodePanel(Node node)
        {
            StackPanel nodePanel = new StackPanel
            {
                MinWidth = 200,
                Orientation = Orientation.Vertical,
                Margin = new Thickness(10),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Tag = node.nodeId
            };

            UpdateNodePanel(node, nodePanel);

            return nodePanel;

        }

        private void UpdateNodePanel(Node node, StackPanel nodePanel)
        {
            nodePanel.Children.Clear();

            StackPanel titlePanel = new StackPanel()
            {
                Background = new SolidColorBrush(Colors.Gainsboro),
                Height = 30
            };

            Grid titleGrid = CreateNodeTitle(node);
            titlePanel.Children.Add(titleGrid);

            nodePanel.Children.Add(titlePanel);

            if (node.name != null)
                nodePanel.Children.Add(new TextBlock { Text = node.name, Margin = new Thickness(5) });

            foreach (Sensor sensor in node.sensors)
            {
                StackPanel oldPanel = GetSensorPanel(node.nodeId, sensor.sensorId);
                if (oldPanel != null)
                    sensorPanels.Remove(oldPanel);

                StackPanel sensorPanel = CreateSensorPanel(sensor);
                nodePanel.Children.Add(sensorPanel);
                sensorPanels.Add(sensorPanel);
            }

            if (node.batteryLevel != null)
            {
                TextBlock oldPanel = GetBatteryPanel(node.nodeId);
                if (oldPanel != null)
                    batteryPanels.Remove(oldPanel);

                TextBlock text = CreateBatteryPanel(node);
                nodePanel.Children.Add(text);
                batteryPanels.Add(text);
            }
        }

        private TextBlock CreateBatteryPanel(Node node)
        {
            TextBlock text = new TextBlock
            {
                Text = "Battery " + node.batteryLevel.Value,
                Margin = new Thickness(5),
                Foreground = new SolidColorBrush(Colors.Gray),
                Tag = node.nodeId
            };
            return text;
        }

        private Grid CreateNodeTitle(Node node)
        {
            Grid grid = new Grid
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            SymbolIcon activityIcon = new SymbolIcon
            {
                Symbol = Symbol.Remove,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5),
                Foreground = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220)),
                Tag = node.nodeId
            };
            activityIcons.Add(activityIcon);
            grid.Children.Add(activityIcon);

     /*       Button button = new Button
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
*/



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



            grid.Children.Add(new TextBlock
            {
                Text = "Node " + node.nodeId,
                Margin = new Thickness(5, 5, 5, 5),
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,

            });


          //  grid.Children.Add(button);


            return grid;
        }

        private async void buttonNodeMenu_Click(object sender, RoutedEventArgs e)
        {
            int nodeId = (int)((ButtonBase)sender).Tag;


            var newCoreAppView = CoreApplication.CreateNewView();
            var appView = ApplicationView.GetForCurrentView();
            await newCoreAppView.Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
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


        private StackPanel CreateSensorPanel(Sensor sensor)
        {
            StackPanel sensorPanel = new StackPanel()
            {

                Orientation = Orientation.Vertical,
                Margin = new Thickness(5),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Tag = String.Format("{0};{1}", sensor.ownerNodeId, sensor.sensorId)
            };

            UpdateSensorPanel(sensor, sensorPanel);

            return sensorPanel;
        }

        private void UpdateSensorPanel(Sensor sensor, StackPanel sensorPanel)
        {
            sensorPanel.Children.Clear();

            string sType = MySensors.GetSimpleSensorType(sensor.GetSensorType());

            if (sensor.description != null)
                sType = String.Format("{0} {1}", sType, sensor.description);

            sensorPanel.Children.Add(new TextBlock { Text = sType, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Gray) });

            foreach (SensorData data in sensor.sensorData)
            {
                StackPanel dataPanel = CreateSensorData(data, sensor);
                sensorPanel.Children.Add(dataPanel);
            }
        }

        private StackPanel GetSensorPanel(int nodeId, int sensorId)
        {
            string tag = String.Format("{0};{1}", nodeId, sensorId);
            StackPanel panel = sensorPanels.FirstOrDefault(x => (string)x.Tag == tag);
            return panel;

        }

        private StackPanel GetNodePanel(int nodeId)
        {
            StackPanel panel = nodePanels.FirstOrDefault(x => (int)x.Tag == nodeId);
            return panel;

        }

        private TextBlock GetBatteryPanel(int nodeId)
        {
            TextBlock panel = batteryPanels.FirstOrDefault(x => (int)x.Tag == nodeId);
            return panel;
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

            //IR Send
            else if (data.dataType == SensorDataType.V_IR_SEND)
            {
                StackPanel textEdit = CreateTextBox(sensor, data);
                dataPanel.Children.Add(textEdit);
            }

            //IR Received
            else if (data.dataType == SensorDataType.V_IR_RECEIVE)
            {
                string s = String.Format("Received: {0}", data.state);
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

        private StackPanel CreateTextBox(Sensor sensor, SensorData data)
        {
            StackPanel textBoxGrid = new StackPanel();

            TextBox box = new TextBox()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(5,0, 5,0)
            };

            box.TextChanged += textBoxChanged;
            box.Tag = String.Format("{0};{1};{2}",
                sensor.ownerNodeId,
                sensor.sensorId,
                data.dataType.ToString());

            Button button = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(5),
                Content = "Send"
            };

            button.Tag = String.Format("{0};{1};{2}",
                sensor.ownerNodeId,
                sensor.sensorId,
                data.dataType.ToString());
            button.Click += textBoxClick;



            textBoxGrid.Children.Add(box);
            textBoxGrid.Children.Add(button);

            return textBoxGrid;
        }

        private void textBoxClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string tag = button.Tag.ToString();
            string[] args = tag.Split(';');
            int nodeId = Int32.Parse(args[0]);
            int sensorId = Int32.Parse(args[1]);
            SensorDataType dataType;
            SensorDataType.TryParse(args[2], true, out dataType);

            Node node = App.serialController.GetNode(nodeId);
            Sensor sensor = node.GetSensor(sensorId);
            SensorData data = sensor.GetData(dataType);

            SendSensorState(nodeId, sensorId, data);
        }

        private void textBoxChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            string tag = box.Tag.ToString();
            string[] args = tag.Split(';');
            int nodeId = Int32.Parse(args[0]);
            int sensorId = Int32.Parse(args[1]);
            SensorDataType dataType;
            SensorDataType.TryParse(args[2], true, out dataType);
            string state = box.Text;

            Node node = App.serialController.GetNode(nodeId);
            Sensor sensor = node.GetSensor(sensorId);
            sensor.AddOrUpdateData(dataType, state);
        }

        private ToggleButton CreateButton(Sensor sensor, SensorData data)
        {
            ToggleButton button = new ToggleButton
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(5),
               
            };
            button.Tag = String.Format("{0};{1};{2}",
                sensor.ownerNodeId,
                sensor.sensorId,
                data.dataType.ToString());

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


        private void RedrawAllNodes()
        {
            itemsControl1.Items.Clear();
            activityIcons.Clear();
            nodePanels.Clear();
            sensorPanels.Clear();
            batteryPanels.Clear();

            List<Node> nodesList = App.serialController.GetNodes();

            foreach (var node in nodesList)
            {
                AddNode(node);
            }

        }

        private void AddNode(Node node)
        {
            StackPanel oldPanel = GetNodePanel(node.nodeId);
            if (oldPanel != null)
                nodePanels.Remove(oldPanel);

            StackPanel nodePanel = CreateNodePanel(node);
            itemsControl1.Items.Add(nodePanel);
            nodePanels.Add(nodePanel);
        }

        private void UpdateNode(Node node)
        {
            StackPanel panel = GetNodePanel(node.nodeId);
            UpdateNodePanel(node, panel);
        }



        private void UpdateSensor(Sensor sensor)
        {
            //prevent slidres update when drug
            if (sensor.ownerNodeId == lastSendedNodeId
                && sensor.sensorId == lastSendedSensorId)
                return;

            StackPanel panel = GetSensorPanel(sensor.ownerNodeId, sensor.sensorId);
            UpdateSensorPanel(sensor, panel);
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

            DelayedSendSensorState(nodeId, sensorId, data);
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

            DelayedSendSensorState(nodeId, sensorId, data);
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


            DelayedSendSensorState(nodeId, sensorId, data);
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




        private void DelayedSendSensorState(int nodeId, int sensorId, SensorData data)
        {
            Sensor sensor = sendSensorsList
                .Where(x => x.ownerNodeId == nodeId)
                .FirstOrDefault(x => x.sensorId == sensorId);
            if (sensor == null)
            {
                sensor = new Sensor { sensorId = sensorId, ownerNodeId = nodeId };
                sendSensorsList.Add(sensor);
            }

            sensor.AddOrUpdateData(data);

        }

        private void SendSensorState(int nodeId, int sensorId, SensorData data)
        {
            lastSendedNodeId = nodeId;
            lastSendedSensorId = sensorId;
            App.serialController.SendSensorState(nodeId, sensorId, data);
            lastSendedNodeId = -1;
            lastSendedSensorId = -1;
        }

        private void DelayedSendTimer(object sender, object e)
        {
            if (!sendSensorsList.Any())
                return;

            //for prevent update from another thread
            Sensor[] sendSensors = new Sensor[sendSensorsList.Count];
            sendSensorsList.CopyTo(sendSensors);
            sendSensorsList.Clear();

            foreach (var sensor in sendSensors)
            {
                foreach (var data in sensor.sensorData)
                {
                    SendSensorState(sensor.ownerNodeId, sensor.sensorId, data);
                }
            }

        }

    }

}
