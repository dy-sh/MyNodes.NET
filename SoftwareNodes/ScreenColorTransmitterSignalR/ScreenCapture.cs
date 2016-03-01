/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ScreenColorTransmitterSignalR
{
    public class ScreenCapture
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        private static extern IntPtr DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        private static extern IntPtr DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest,
            int nYDest, int nWidth, int nHeight, IntPtr hdcSrc,
            int nXSrc, int nYSrc, int dwRop);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc,
            int nWidth, int nHeight);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobjBmp);

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", EntryPoint = "GetDC")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        private static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        private static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);


        /// <summary>
        /// Capture upper part of screen
        /// </summary>
        /// <param name="heightPercent">1 means full screen. 0.5 means upper half of screen.</param>
        /// <returns></returns>
        public static Bitmap GetScreenshot(float heightPercent = 1)
        {
            int screenX;
            int screenY;
            IntPtr hBmp;
            IntPtr hdcScreen = GetDC(GetDesktopWindow());
            IntPtr hdcCompatible = CreateCompatibleDC(hdcScreen);

            screenX = GetSystemMetrics(0);
            screenY = GetSystemMetrics(1);

            screenY = (int)( screenY* heightPercent);

            hBmp = CreateCompatibleBitmap(hdcScreen, screenX, screenY);

            if (hBmp != IntPtr.Zero)
            {
                IntPtr hOldBmp = (IntPtr)SelectObject(hdcCompatible, hBmp);
                BitBlt(hdcCompatible, 0, 0, screenX, screenY, hdcScreen, 0, 0, 13369376);

                SelectObject(hdcCompatible, hOldBmp);
                DeleteDC(hdcCompatible);
                ReleaseDC(GetDesktopWindow(), hdcScreen);

                Bitmap bmp = Image.FromHbitmap(hBmp);

                DeleteObject(hBmp);
                GC.Collect();

                return bmp;
            }

            return null;
        }


        


        public static Color GetScreenAverageColor(float heightPercent = 1)
        {
           // Stopwatch sw = new Stopwatch();
            Bitmap screenshot = GetScreenshot(heightPercent);
           // sw.Start();
            // Console.WriteLine(sw.ElapsedMilliseconds);
            Color avarageColor = CalculateAvarageColorUnsafe(screenshot);
            //Console.WriteLine(sw.ElapsedMilliseconds);
            // Console.WriteLine("----");
            //sw.Stop();
            screenshot.Dispose();
            return avarageColor;
        }




        unsafe static private Color CalculateAvarageColorUnsafe(Bitmap bitmap)
        {
            //1920*540
            //5ms

            //1920*1080
            //10ms

            //lock bitmap in system memory
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                bitmap.PixelFormat);

            //get 1-pixel size
            int pixelSize;

            if (bitmap.PixelFormat == PixelFormat.Format24bppRgb)
                pixelSize = 3;
            else if (bitmap.PixelFormat == PixelFormat.Format32bppRgb)
                pixelSize = 4;
            else
                throw new Exception($"Can`t use pixel-format {bitmap.PixelFormat}. Use 24bppRgb or 32bppRgb.");


            //get summ of all pixels
            long summR = 0;
            long summG = 0;
            long summB = 0;

            for (int y = 0; y < bitmapData.Height; ++y)
            {
                byte* row = (byte*)bitmapData.Scan0 + (bitmapData.Stride * y);
                for (int x = 0; x < bitmapData.Width; ++x)
                {
                    int i = x * pixelSize;
                    summR += row[i + 2];
                    summG += row[i + 1];
                    summB += row[i];
                }
            }

            //get avarage color
            long pixelsCount = bitmapData.Width * bitmapData.Height;
            int r = (int)(summR / pixelsCount);
            int g = (int)(summG / pixelsCount);
            int b = (int)(summB / pixelsCount);

            //unlock bitmap from memory
            bitmap.UnlockBits(bitmapData);

            Color resultColor = Color.FromArgb(r, g, b);

            return resultColor;
        }
        

        static private Color CalculateAvarageColorSafe(Bitmap bitmap)
        {
            //1920*540
            //730ms

            //1920*1080
            //1820ms

            int r = 0;
            int g = 0;
            int b = 0;

            int total = 0;

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color clr = bitmap.GetPixel(x, y);

                    r += clr.R;
                    g += clr.G;
                    b += clr.B;

                    total++;
                }
            }

            r /= total;
            g /= total;
            b /= total;

            Color averageColor = Color.FromArgb(r, g, b);

            return averageColor;
        }




    }
}
