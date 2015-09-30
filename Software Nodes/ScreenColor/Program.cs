using System;
using System.Collections.Generic;
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
using ScreenColorServer;


namespace ScreenColor
{
    class Program
    {
        //SETTINGS
        const int CAPTURE_UPDATE_DELAY = 0;
        const float HEIGHT_FROM_TOP = 0.4f;

        static float rTune = 1f;
        static float gTune = 0.5f;
        static float bTune = 0.3f;

        static bool isWorking;
        static Color screenColor;

        private static DateTime captureStartDate = DateTime.Now;
        private static int screensCount;

        private static ISoftNodeClient softNodeClient;
        private static SoftNode softNode;

        private static DateTime messageStartDate = DateTime.Now;
        private static int messagesCount;

        static void Main(string[] args)
        {
            softNodeClient = new SoftNodeClient();
            softNode = new SoftNode(softNodeClient);
            softNode.ConnectToServer();
            StartScreenCapture();
            Console.WriteLine("Screen capture started");

            while (true)
            {
                Console.ReadLine();
            }
        }

        private static String ColorToHex(Color color)
        {
            return /* "#" + */ color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public static void SendColor(Color color)
        {
            SensorData data = new SensorData(SensorDataType.V_RGB, ColorToHex(color));
            softNode.SendSensorData(1, data);
        }

        private static async void StartScreenCapture()
        {
            if (isWorking) return;

            isWorking = true;

            while (isWorking)
            {
                await Task.Delay(CAPTURE_UPDATE_DELAY);

                await Task.Run(() =>
                {
                    CalculateCapturesPerSec();

                    Color newScreenColor = ScreenCapture.GetScreenAverageColor(HEIGHT_FROM_TOP);

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

            if(capturesPerSecond>1)
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
