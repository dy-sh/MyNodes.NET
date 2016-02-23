/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyNetSensors.SoftNodes;


namespace ScreenColor
{
    class Program
    {
        //SETTINGS

        static int captureUpdateDelay = 0;
        static float heightFromTop = 0.4f;

        static bool isWorking;
        static Color screenColor;

        private static DateTime captureStartDate = DateTime.Now;
        private static int screensCount;

        private static DateTime messageStartDate = DateTime.Now;
        private static int messagesCount;

        private static SoftNodeTransmitter transmitter;

        static string serverAddress = "http://localhost:1312";
        static int receiverChannel = 0;
        static string receiverPassword;

        static void Main(string[] args)
        {
            ReadSettings();

            transmitter = new SoftNodeTransmitter(serverAddress, receiverChannel, receiverPassword);
            transmitter.OnLogError += LogError;
           // transmitter.OnLogInfo += LogInfo;

            StartScreenCapture();
            LogCapture("Screen capture started");

            while (true)
            {
                Console.ReadLine();
            }
        }




        static void ReadSettings()
        {
            //Set up dot instead of comma in float values
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            //read app.config
            heightFromTop = float.Parse(ConfigurationManager.AppSettings["ScreenHeightFromTop"]);
            captureUpdateDelay = int.Parse(ConfigurationManager.AppSettings["CapturingDelay"]);
            serverAddress = ConfigurationManager.AppSettings["ServerAddress"];
            receiverChannel = int.Parse(ConfigurationManager.AppSettings["ReceiverChannel"]);
            receiverPassword = ConfigurationManager.AppSettings["ReceiverPassword"];
        }

        private static String ColorToHex(Color color)
        {
            return /* "#" + */ color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public static void SendColor(Color color)
        {

            string value = ColorToHex(color);

            transmitter.Send(value);

            CalculateMessagesPerSec();
        }

        private static void LogError(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("TRANSMITTER: "+text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void LogInfo(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("TRANSMITTER : " + text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void LogCapture(string message)
        {
            Console.WriteLine("CAPTURE: " + message);
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

                    if (newScreenColor != screenColor)
                    {
                        screenColor = newScreenColor;
                        SendColor(screenColor);
                    }
                });
            }

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
                LogCapture("Captured " + (int)capturesPerSecond + " screens/second");
            else
                LogCapture("Captured " + capturesPerSecond.ToString("0.00") + " screens/second");

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
                LogCapture("                               Sent " + (int)messagesPerSecond + " messages/second");
            else
                LogCapture("                               Sent " + messagesPerSecond.ToString("0.00") + " messages/second");

        }






    }
}
