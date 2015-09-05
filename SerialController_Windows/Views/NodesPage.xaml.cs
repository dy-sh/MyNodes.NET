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


        private StackPanel CreateNode(int nodeId)
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

            titlePanel.Children.Add(new TextBlock { Text = "Node id:" + nodeId, Margin = new Thickness(10), FontWeight = FontWeights.SemiBold });

            nodePanel.Children.Add(titlePanel);


            Button button2 = new Button();
            button2.Width = 200;
            button2.Height = 50;
            button2.Margin = new Thickness(10, 10, 10, 10);
            button2.Content = "Кнопка2";
            nodePanel.Children.Add(button2);
            return nodePanel;
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
            StackPanel nodePanel = CreateNode(node.nodeId);

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
