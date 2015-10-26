/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SerialController.Windows.Code.MySensors;
using SerialController.Windows;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SerialController.Windows.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NodesListPage : Page
    {
        public NodesListPage()
        {
            this.InitializeComponent();
            App.serialController.OnNewNodeEvent += AddNode;
            App.serialController.OnNodeUpdatedEvent += UpdateNode;
            App.serialController.OnSensorUpdatedEvent += UpdateSensor;


            if (App.serialController.IsConnected())
            {
                textBlock3.Visibility = Visibility.Collapsed;
                Panel1.Visibility = Visibility.Visible;
                ShowNodes();
            }
            else
            {
                textBlock3.Text = "Device is not connected";
                textBlock3.Visibility = Visibility.Visible;
                Panel1.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowNodes()
        {
            listView1.Items.Clear();

            List<Node> nodes = App.serialController.GetNodes();

            foreach (var node in nodes)
            {
                AddNode(node);
            }

        }

        private void AddNode(Node node)
        {
            listView1.Items.Add(node.ToString());

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

