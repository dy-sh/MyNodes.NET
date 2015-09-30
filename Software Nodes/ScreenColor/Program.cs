/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyNetSensors.Gateway;
using MyNetSensors.SoftNodes;
using MyNetSensors.SoftNodesSignalRClient;


namespace ScreenColor
{
    class Program
    {
        //SETTINGS
        private static string serverURL = "http://localhost:13122/";

        static int captureUpdateDelay = 0;
        static float heightFromTop = 0.4f;

        static float rTune = 1f;
        static float gTune = 0.5f;
        static float bTune = 0.3f;

        static bool isWorking;
        static Color screenColor;

        private static int sensorId = 1;
        private static int nodeId = 255;
        private static string nodeName = "Screen Color";
        private static string nodeVersion = "1.0";
        private static string sensorDescription = "Average Screen Color";

        private static DateTime captureStartDate = DateTime.Now;
        private static int screensCount;

        private static ISoftNodeClient softNodeClient;
        private static SoftNode softNode;

        private static DateTime messageStartDate = DateTime.Now;
        private static int messagesCount;

        static void Main(string[] args)
        {
            ReadSettings();

            softNodeClient = new SoftNodeClient();
            softNode = new SoftNode(softNodeClient, nodeId, nodeName, nodeVersion);
            softNode.OnIdResponseReceived += OnIdResponseReceived;
            softNode.ConnectToServer(serverURL);


            Sensor sensor = new Sensor();
            sensor.sensorId = sensorId;
            sensor.sensorType = SensorType.S_RGB_LIGHT;
            sensor.description = sensorDescription;
            softNode.AddSensor(sensor);

            StartScreenCapture();
            Console.WriteLine("Screen capture started");

            while (true)
            {
                Console.ReadLine();
            }
        }

        private static void OnIdResponseReceived(int nodeid)
        {

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove("NodeId");
            config.AppSettings.Settings.Add("NodeId", nodeid.ToString());
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

        }

        static void ReadSettings()
        {
            //Set up dot instead of comma in float values
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            //read app.config
            rTune = float.Parse(ConfigurationManager.AppSettings["RTune"]);
            gTune = float.Parse(ConfigurationManager.AppSettings["GTune"]);
            bTune = float.Parse(ConfigurationManager.AppSettings["BTune"]);
            heightFromTop = float.Parse(ConfigurationManager.AppSettings["ScreenHeightFromTop"]);
            captureUpdateDelay = int.Parse(ConfigurationManager.AppSettings["CapturingDelay"]);
            serverURL = ConfigurationManager.AppSettings["SoftNodesServerURL"];
            nodeId = Int32.Parse(ConfigurationManager.AppSettings["NodeId"]);
            nodeName = ConfigurationManager.AppSettings["NodeName"];
            nodeVersion = ConfigurationManager.AppSettings["NodeVersion"];
        }

        private static String ColorToHex(Color color)
        {
            return /* "#" + */ color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public static void SendColor(Color color)
        {
            SensorData data = new SensorData(SensorDataType.V_RGB, ColorToHex(color));
            softNode.SendSensorData(sensorId, data);
        }

        private static async void StartScreenCapture()
        {
            if (isWorking) return;

            isWorking = true;

            while (isWorking)
            {
                await Task.Delay(captureUpdateDelay);

                await Task.Run(() =>
                {
                    CalculateCapturesPerSec();

                    Color newScreenColor = ScreenCapture.GetScreenAverageColor(heightFromTop);

                    newScreenColor = TuneColor(newScreenColor);

                    if (newScreenColor != screenColor)
                    {
                        screenColor = newScreenColor;
                        SendColor(screenColor);
                        CalculateMessagesPerSec();
                    }
                });
            }

        }

        private static Color TuneColor(Color newScreenColor)
        {
            int r = newScreenColor.R;
            int g = newScreenColor.G;
            int b = newScreenColor.B;

            r = (int)((float)(r) * rTune);
            g = (int)((float)(g) * gTune);
            b = (int)((float)(b) * bTune);

            return Color.FromArgb(r, g, b);
        }

        private static void StopScreenCapture()
        {
            isWorking = false;
        }

        private static void CalculateCapturesPerSec()
        {
            screensCount++;

            DateTime now = DateTime.Now;
            TimeSpan elapsed = now.Subtract(captureStartDate);

            if (elapsed.TotalSeconds < 1)
                return;

            float capturesPerSecond = screensCount / (float)elapsed.TotalSeconds;

            captureStartDate = DateTime.Now;
            screensCount = 0;

            if (capturesPerSecond > 1)
                Console.WriteLine("Captured " + (int)capturesPerSecond + " screens/second");
            else
                Console.WriteLine("Captured " + capturesPerSecond.ToString("0.00") + " screens/second");

        }

        private static void CalculateMessagesPerSec()
        {
            messagesCount++;

            DateTime now = DateTime.Now;
            TimeSpan elapsed = now.Subtract(messageStartDate);

            if (elapsed.TotalSeconds < 1)
                return;

            float messagesPerSecond = messagesCount / (float)elapsed.TotalSeconds;

            messageStartDate = DateTime.Now;
            messagesCount = 0;

            if (messagesPerSecond > 1)
                Console.WriteLine("                               Sended " + (int)messagesPerSecond + " messages/second");
            else
                Console.WriteLine("                               Sended " + messagesPerSecond.ToString("0.00") + " messages/second");

        }

    }
}
