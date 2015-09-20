/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public sealed partial class NodesViewPage : Page
    {

        private DispatcherTimer refrashTimer;

        //    public ObservableCollection<Model> Models { get; set; }
        public NodesViewPage()
        {
            this.InitializeComponent();

            App.serialController.OnNewNodeEvent += AddNode;
            App.serialController.OnNodeUpdatedEvent += UpdateNode;
            App.serialController.OnSensorUpdatedEvent += UpdateSensor;
            App.serialController.OnClearNodesListEvent += OnClearNodesListEvent;

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

            refrashTimer = new DispatcherTimer();
            refrashTimer.Interval = TimeSpan.FromMilliseconds(100);
            refrashTimer.Tick += RefrashInfoTimer;
            refrashTimer.Start();

        }

        private void OnClearNodesListEvent(object sender, EventArgs e)
        {
            ShowNodes();
        }

        private void RefrashInfoTimer(object sender, object e)
        {
            ShowNodes();
        }


        private StackPanel CreateNode(Node node)
        {
            StackPanel nodePanel = new StackPanel();
            nodePanel.Orientation = Orientation.Vertical;
            nodePanel.Margin = new Thickness(5);
            nodePanel.BorderThickness = new Thickness(1);
            nodePanel.BorderBrush = new SolidColorBrush(Colors.Black);
            nodePanel.Background = new SolidColorBrush(Colors.Gray);

            StackPanel titlePanel = new StackPanel();
            titlePanel.Background = new SolidColorBrush(Colors.Gainsboro);
            titlePanel.Height = 30;

            titlePanel.Children.Add(new TextBlock { Text = "Node id: " + node.nodeId, Margin = new Thickness(5), FontWeight = FontWeights.SemiBold });

            nodePanel.Children.Add(titlePanel);

            if (node.name != null)
                nodePanel.Children.Add(new TextBlock { Text = node.name, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Gainsboro) });

            if (node.version != null)
                nodePanel.Children.Add(new TextBlock { Text = "Version: " + node.version, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Gainsboro) });

            nodePanel.Children.Add(new TextBlock { Text = "Registered: " + node.registered, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Gainsboro) });

            string lastSeenAgo = string.Format("{0:hh\\:mm\\:ss}",  DateTime.Now.Subtract(node.lastSeen));
            nodePanel.Children.Add(new TextBlock { Text = "Last seen: " + lastSeenAgo+" ago", Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Gainsboro) });



            if (node.isRepeatingNode == null)
                nodePanel.Children.Add(new TextBlock
                {
                    Text = "Repeating: Unknown",
                    Margin = new Thickness(5),
                    Foreground = new SolidColorBrush(Colors.Gainsboro)
                });
            else if (node.isRepeatingNode.Value)
                nodePanel.Children.Add(new TextBlock
                {
                    Text = "Repeating: Yes",
                    Margin = new Thickness(5),
                    Foreground = new SolidColorBrush(Colors.Gainsboro)
                });
            else
                nodePanel.Children.Add(new TextBlock
                {
                    Text = "Repeating: No",
                    Margin = new Thickness(5),
                    Foreground = new SolidColorBrush(Colors.Gainsboro)
                });

            if (node.batteryLevel != null)
                nodePanel.Children.Add(new TextBlock { Text = "Battery: " + node.batteryLevel.Value, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Gainsboro) });



            foreach (Sensor sensor in node.sensors)
            {
                StackPanel sensorPanel = CreateSensor(sensor);
                nodePanel.Children.Add(sensorPanel);
            }
            return nodePanel;

        }

        private StackPanel CreateSensor(Sensor sensor)
        {
            StackPanel sensorPanel = new StackPanel();

            sensorPanel.Orientation = Orientation.Vertical;
            sensorPanel.Margin = new Thickness(5);
            sensorPanel.BorderThickness = new Thickness(1);
            sensorPanel.BorderBrush = new SolidColorBrush(Colors.Gainsboro);
            sensorPanel.Background = new SolidColorBrush(Colors.Gray);


            sensorPanel.Children.Add(new TextBlock { Text = "Sensor id: " + sensor.sensorId, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Gainsboro) });

            string sType = (sensor.GetSensorType() == null) ? "unknown" : sensor.GetSensorType().ToString();

            sensorPanel.Children.Add(new TextBlock { Text = "Sensor type: " + sType, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Gainsboro) });

            if (sensor.description != null)
                sensorPanel.Children.Add(new TextBlock { Text = "Description: " + sensor.description, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Gainsboro) });


            foreach (SensorData data in sensor.sensorData)
            {
                StackPanel dataPanel = CreateSensorData(data);
                sensorPanel.Children.Add(dataPanel);
            }
            return sensorPanel;
        }


        private StackPanel CreateSensorData(SensorData data)
        {
            StackPanel dataPanel = new StackPanel();

            string s = String.Format("Data: {0}, State: {1}", data.dataType, data.state);
            dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(5), Foreground = new SolidColorBrush(Colors.Gainsboro) });

            return dataPanel;
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


    }




}
