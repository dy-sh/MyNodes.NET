using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class MessagesLogPage : Page
    {
        int? filterNodeId = null;
        int? filterSensorId = null;
        MessageType? filterMesType = null;

        public MessagesLogPage()
        {
            this.InitializeComponent();


            App.serialController.OnMessageRecievedEvent += AddMessageEvent;
            App.serialController.OnMessageSendEvent += AddMessageEvent;

            if (App.serialController.IsConnected())
            {
                textBlock3.Visibility = Visibility.Collapsed;
                Panel1.Visibility = Visibility.Visible;
                ShowFilteredMessages();
                UpdateCombos();
            }
            else
            {
                textBlock3.Text = "Device is not connected";
                textBlock3.Visibility = Visibility.Visible;
                Panel1.Visibility = Visibility.Collapsed;
            }

        }

        private void ShowAllMessages()
        {
            listView1.Items.Clear();

            List<Message> messages =
                App.serialController.messagesLog.GetAllMessages();

            foreach (var logMessage in messages)
            {
                AddMessage(logMessage);
            }

            scrollViewer1.UpdateLayout();
            scrollViewer1.ChangeView(0.0f, double.MaxValue, 1.0f);
        }

        private void ShowFilteredMessages()
        {
            listView1.Items.Clear();



            List<Message> messages =
                App.serialController.messagesLog.GetFilteredMessages(filterNodeId, filterSensorId, filterMesType);

            foreach (var logMessage in messages)
            {
                AddMessage(logMessage);
            }

            scrollViewer1.UpdateLayout();
            scrollViewer1.ChangeView(0.0f, double.MaxValue, 1.0f);
        }

        private void AddMessage(Message message)
        {
            listView1.Items.Add(message.ToString());
        }

        private void AddMessageEvent(Message message)
        {
            UpdateCombos();

            if (filterNodeId != null
                && filterNodeId != message.nodeId)
                return;

            if (filterSensorId != null
                && filterSensorId != message.sensorId)
                return;

            if (filterMesType != null
                && filterMesType != message.messageType)
                return;

            AddMessage(message);
            

            scrollViewer1.UpdateLayout();
            scrollViewer1.ChangeView(0.0f, double.MaxValue, 1.0f);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            filterNodeId = null;
            filterSensorId = null;
            filterMesType = null;

            App.serialController.messagesLog.ClearLog();
            UpdateCombos();
            ShowFilteredMessages();
        }



        private void UpdateCombos()
        {
            object selected = comboBox1.SelectedItem;

            comboBox1.Items.Clear();
            comboBox1.Items.Add("");

            List<int> nodesId = App.serialController.messagesLog.GetAllNodesId();

            foreach (int i in nodesId)
            {
                comboBox1.Items.Add(i.ToString());
            }

            comboBox1.SelectedItem = selected;




            selected = comboBox2.SelectedItem;

            comboBox2.Items.Clear();
            comboBox2.Items.Add("");


            List<int> sensorId = App.serialController.messagesLog.GetAllSensorsId();

            foreach (int i in sensorId)
            {
                comboBox2.Items.Add(i.ToString());
            }

            comboBox2.SelectedItem = selected;


            selected = comboBox3.SelectedItem;

            comboBox3.Items.Clear();
            comboBox3.Items.Add("");

            List<MessageType> mesTypes = App.serialController.messagesLog.GetAllMessageTypes();

            foreach (MessageType i in mesTypes)
            {
                comboBox3.Items.Add(i.ToString());
            }

            comboBox3.SelectedItem = selected;
        }



        private void button2_Click(object sender, RoutedEventArgs e)
        {

            filterNodeId = null;
            filterSensorId = null;
            filterMesType = null;

            if (comboBox1.SelectedItem != null && comboBox1.SelectedItem.ToString() != "")
                filterNodeId = Int32.Parse(comboBox1.SelectedItem.ToString());

            if (comboBox2.SelectedItem != null && comboBox2.SelectedItem.ToString() != "")
                filterSensorId = Int32.Parse(comboBox2.SelectedItem.ToString());

            if (comboBox3.SelectedItem != null && comboBox3.SelectedItem.ToString() != "")
            {
                MessageType outType;
                MessageType.TryParse(comboBox3.SelectedItem.ToString(), true, out outType);
                filterMesType = outType;
            }

            ShowFilteredMessages();
        }
    }
}

