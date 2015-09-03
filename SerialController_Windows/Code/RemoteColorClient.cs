using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace SerialController_Windows.Code
{
    public delegate void ColorRecievedEventHandler(uint r, uint g, uint b);


    public class RemoteColorClient
    {
        const int REQUEST_DELAY = 10;


        private HttpClient httpClient;
        private string serverURL;
        private bool isRunning;
        private bool isConnected;

        Stopwatch debugSW = new Stopwatch();

        public event ColorRecievedEventHandler colorRecievedEvent;
        public event EventHandler onConnectedEvent;
        public event EventHandler onDisconnectedEvent;


        public RemoteColorClient(string serverURL)
        {
            httpClient = new HttpClient();

            // Limit the max buffer size for the response so we don't get overwhelmed
        //    httpClient.MaxResponseContentBufferSize = 256000;
          //  httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            this.serverURL = serverURL;
        }

        public void SetUrl(string serverURL)
        {
            this.serverURL = serverURL;
        }

        public void StartService()
        {
            if (isRunning) return;

            isRunning = true;
            RequestColorLoop();
        }

        public void StopService()
        {
            isRunning = false;

            if (isConnected)
            {
                isConnected = false;
                if (onDisconnectedEvent != null) onDisconnectedEvent(this, null);
            }
        }

        public bool IsRunning()
        {
            return isRunning;
        }

        public bool IsConnected()
        {
            return isConnected;
        }


        public async void RequestColorLoop()
        {
            while (isRunning)
            {
                //debugSW.Restart();

                try
                {
                    await Task.Delay(REQUEST_DELAY);

                    HttpResponseMessage response = await httpClient.GetAsync(serverURL);

                    // response.EnsureSuccessStatusCode();


                    string serponseSting = await response.Content.ReadAsStringAsync();

                    //debugSW.Stop();
                    //Debug.WriteLine(debugSW.ElapsedMilliseconds);

                    ParseColorResponse(serponseSting);

                    //   Debug.WriteLine(serponseSting);

                    //isRunning can change outside, while waiting resporse
                    if (isRunning && !isConnected)
                    {
                        isConnected = true;
                        if (onConnectedEvent != null) onConnectedEvent(this,null);
                    }
                }
                catch (Exception ex)
                {
                    if (isConnected)
                    {
                        isConnected = false;
                        if (onDisconnectedEvent != null) onDisconnectedEvent(this, null);
                    }

                    // debugSW.Stop();
                    Debug.WriteLine(ex.ToString());
                    //Debug.WriteLine(debugSW.ElapsedMilliseconds);
                }
            }
        }


        private void ParseColorResponse(string response)
        {
            JsonObject root = JsonValue.Parse(response).GetObject();

            string rString = root["r"].GetString();
            string gString = root["g"].GetString();
            string bString = root["b"].GetString();

            uint r = UInt32.Parse(rString);
            uint g = UInt32.Parse(gString);
            uint b = UInt32.Parse(bString);

            if (colorRecievedEvent != null) colorRecievedEvent(r, g, b);
        }

    }
}
