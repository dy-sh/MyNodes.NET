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
    public sealed partial class NodesPage : Page
    {

        //    public ObservableCollection<Model> Models { get; set; }
        public NodesPage()
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
            nodePanel.Orientation = Orientation.Vertical;
            nodePanel.Margin = new Thickness(10);
            nodePanel.BorderThickness = new Thickness(1);
            nodePanel.BorderBrush = new SolidColorBrush(Colors.Black);
            nodePanel.Background = new SolidColorBrush(Colors.Gray);

            StackPanel titlePanel = new StackPanel();
            titlePanel.Background = new SolidColorBrush(Colors.Gainsboro);
            titlePanel.Height = 40;

            titlePanel.Children.Add(new TextBlock { Text = "Node id: " + node.nodeId, Margin = new Thickness(10), FontWeight = FontWeights.SemiBold });

            nodePanel.Children.Add(titlePanel);

            nodePanel.Children.Add(new TextBlock { Text = "Last seen: " + node.lastSeen, Margin = new Thickness(10), Foreground = new SolidColorBrush( Colors.Gainsboro) });

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
            sensorPanel.Margin = new Thickness(10);
            sensorPanel.BorderThickness = new Thickness(1);
            sensorPanel.BorderBrush = new SolidColorBrush(Colors.Black);
            sensorPanel.Background = new SolidColorBrush(Colors.Gray);

            StackPanel titlePanel = new StackPanel();
            titlePanel.Background = new SolidColorBrush(Colors.Gainsboro);
            titlePanel.Height = 40;
            titlePanel.Children.Add(new TextBlock { Text = "Sensor id: " + sensor.sensorId, Margin = new Thickness(10), FontWeight = FontWeights.SemiBold });

            string sType = (sensor.sensorType == null) ? "unknown": sensor.sensorType.ToString(); 

            sensorPanel.Children.Add(new TextBlock { Text = "Sensor type: " + sType, Margin = new Thickness(10), Foreground = new SolidColorBrush(Colors.Gainsboro) });


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

            string s =String.Format("Data: {0}, State: {1}", data.dataType, data.state); 
            dataPanel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(10), Foreground = new SolidColorBrush(Colors.Gainsboro) });

     /*       Button button1 = new Button();
          //  button1.Name = String.Format("{0}", data.state);
            button1.Width = 200;
            button1.Height = 50;
            button1.Margin = new Thickness(10, 10, 10, 10);
            button1.Content = data.state;
            button1.Click += button_Click;
            dataPanel.Children.Add(button1);*/
            return dataPanel;
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

         /*   for (int i = 0; i < 10; i++)
            {
                StackPanel nodePanel = CreateNode(i.ToString());

                itemsControl1.Items.Add(nodePanel);
            }*/
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
            
        }

    }




}
