using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialController_Windows
{

    static class RGBColors
    {
        /* 
        Hue1536:

        255         0-255
        >0-255
        0

        <255-0      256-511
        255
        0

        0           512-767
        255  
        >0-255

        0           758-1023
        <255-0
        255

        >0-255      1024-1279
        0
        255

        255         1280-1536
        0
        <255-0



        Hue768:

        <255-0      0-255
        >0-255
        0

        0           256-511
        <255-0
        >0-255

        >0-255      512-767
        0  
        <255-0

        */

        static Random random = new Random();

        public static void ConvertRGBToHue1536(uint r, uint g, uint b, out uint hue, out float brightness)
        {

            hue = 0;
            brightness = 0;

            //calculate brightness
            if (b > r && b >= g)
            {
                brightness = (float)(b) / 255;
                b = 255;

                if (r > g)
                {
                    r = (uint)(1f / brightness * r);
                    g = 0;
                }
                else
                {
                    r = 0;
                    g = (uint)(1f / brightness * g);
                }
            }
            else if (g > r && g > b)
            {
                brightness = (float)(g) / 255;
                g = 255;

                if (r > b)
                {
                    r = (uint)(1f / brightness * r);
                    b = 0;
                }
                else
                {
                    r = 0;
                    b = (uint)(1f / brightness * b);

                }
            }
            else
            {
                brightness = (float)(r) / 255;
                r = 255;
                if (g > b)
                {
                    g = (uint)(1f / brightness * g);
                    b = 0;
                }
                else
                {
                    g = 0;
                    b = (uint)(1f / brightness * b);

                }
            }



            //calculate color
            if (r == 255 && g >= 0 && b == 0)
            {
                hue = 0 + g;
            }
            else if (r >= 0 && g == 255 && b == 0)
            {
                hue = 256 + (255 - r);
            }
            else if (r == 0 && g == 255 && b >= 0)
            {
                hue = 512 + b;
            }
            else if (r == 0 && g >= 0 && b == 255)
            {
                hue = 768 + (255 - g);
            }
            else if (r >= 0 && g == 0 && b == 255)
            {
                hue = 1024 + r;
            }
            else if (r == 255 && g == 0 && b >= 0)
            {
                hue = 1280 + (255 - b);
            }
        }




         public static void ConvertRGBToHue768(uint r, uint g, uint b, out uint hue, out float brightness)
          {
            hue = 0;
            brightness = 0;

            //exclude min value
            uint minColor = Math.Min(r, Math.Min(g, b));
             if (r == minColor)
                 r = 0;
            else if (g == minColor)
                g = 0;
            else if (b == minColor)
                b = 0;


            //calculate color
            if (r >= 0 && g >= 0 && b == 0)
            {
                brightness =  (float)(r + g)/255;
                r = (uint)(1f / brightness * r);
                g = (uint)(1f / brightness * g);
                hue = g;
            }
            else if (r == 0 && g >= 0 && b >= 0)
            {
                brightness = (float)(g + b) / 255;
                g = (uint)(1f / brightness * g);
                b = (uint)(1f / brightness * b);
                hue = 256+b;
            }
            else //if (r >= 0 && g == 0 && b >= 0)
            {
                brightness = (float)(r + b) / 255;
                r = (uint)(1f / brightness * r);
                b = (uint)(1f / brightness * b);
                hue = 512 + r;
            }

        }



        public static void ConvertHue1536ToRGB(out uint r, out uint g, out uint b, uint hue, float brightness = 1)
        {
            r = 0;
            g = 0;
            b = 0;

            brightness = MathUtils.Clamp(brightness, 0f, 1f);
            hue = MathUtils.Clamp(hue, 0, 1535);

            if (hue < 256)
            {
                r = 255;
                g = hue;
                b = 0;
            }
            else if (hue >= 256 && hue < 512)
            {
                r = 512 - hue - 1;
                g = 255;
                b = 0;
            }
            else if (hue >= 512 && hue < 768)
            {
                r = 0;
                g = 255;
                b = hue - 512;
            }
            else if (hue >= 768 && hue < 1024)
            {
                r = 0;
                g = 1024 - hue - 1;
                b = 255;
            }
            else if (hue >= 1024 && hue < 1280)
            {
                r = hue - 1024;
                g = 0;
                b = 255;
            }
            else
            {
                r = 255;
                g = 0;
                b = 1536 - hue - 1;
            }


            r = (uint)(brightness * r);
            g = (uint)(brightness * g);
            b = (uint)(brightness * b);
        }




        public static void ConvertHue768ToRGB(out uint r, out uint g, out uint b, uint hue, float brightness = 1)
        {
            r = 0;
            g = 0;
            b = 0;

            brightness = MathUtils.Clamp(brightness, 0f, 1f);
            hue = MathUtils.Clamp(hue, 0, 767);

            if (hue < 256)
            {
                r = 256 - hue - 1;
                g = hue;
            }
            else if (hue >= 256 && hue < 512)
            {
                g = 512 - hue - 1;
                b = hue - 256;
            }
            else
            {
                b = 768 - hue - 1;
                r = hue - 512;
            }

            r = (uint)(brightness * r);
            g = (uint)(brightness * g);
            b = (uint)(brightness * b);
        }

        public static float GetRandomBrightness()
        {
            return random.Next(0, 1);
        }

        public static uint GetRandomHue1536()
        {
            return (uint)random.Next(0, 1535);
        }

        public static uint GetRandomHue768()
        {
            return (uint)random.Next(0, 767);
        }

        public static void GetRandomRGB(out uint r, out uint g, out uint b)
        {
            r = (uint)random.Next(0, 255);
            g = (uint)random.Next(0, 255);
            b = (uint)random.Next(0, 255);
        }
    }
}
