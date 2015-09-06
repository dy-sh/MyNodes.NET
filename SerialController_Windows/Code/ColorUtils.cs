using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;

namespace SerialController_Windows.Code
{
    public static class ColorUtils
    {
        /// <summary>
        /// Convert string like "00AAFF" or "#00AAFF" (R/G/B/W) to int variables
        /// </summary>
        public static void ConvertRGBHexStringToInts(string hexString,out int r,out int g,out int b)
        {
            if (hexString[0] == '#')
                hexString= hexString.Remove(0, 1);

            r = int.Parse(hexString.Substring(0, 2), NumberStyles.HexNumber);
            g = int.Parse(hexString.Substring(2, 2), NumberStyles.HexNumber);
            b = int.Parse(hexString.Substring(4, 2), NumberStyles.HexNumber);
        }


        /// <summary>
        /// Convert string like "00AAFF99" or "#00AAFF99" (R/G/B/W) to int variables
        /// </summary>

        public static void ConvertRGBWHexStringToInts(string hexString, out int r, out int g, out int b,out int w)
        {
            if (hexString[0] == '#')
                hexString = hexString.Remove(0, 1);

            r = int.Parse(hexString.Substring(0, 2), NumberStyles.HexNumber);
            g = int.Parse(hexString.Substring(2, 2), NumberStyles.HexNumber);
            b = int.Parse(hexString.Substring(4, 2), NumberStyles.HexNumber);
            w = int.Parse(hexString.Substring(6, 2), NumberStyles.HexNumber);
        }

        /// <summary>
        /// Convert string like "00AAFF" or "#00AAFF" (R/G/B/W) to int array
        /// </summary>
        public static int[] ConvertRGBHexStringToIntArray(string hexString )
        {
            int[] rgb =new int[3];

            ConvertRGBHexStringToInts(hexString,out rgb[0], out rgb[1],out rgb[2]);

            return rgb;
        }


        /// <summary>
        /// Convert string like "00AAFF99" or "#00AAFF99" (R/G/B/W) to int array
        /// </summary>

        public static int[] ConvertRGBWHexStringToIntArray(string hexString)
        {
            int[] rgbw = new int[4];

            ConvertRGBWHexStringToInts(hexString, out rgbw[0], out rgbw[1], out rgbw[2], out rgbw[3]);

            return rgbw;
        }







        /// <summary>
        /// Convert r,g,b int variables to string like "00AAFF" or "#00AAFF" (if useSharp)
        /// </summary>
        public static string ConvertRGBIntsToHexString( int r, int g, int b,bool useSharp=false)
        {
            string result=(useSharp)?"#":"";

            result += r.ToString("X2")
                              + g.ToString("X2")
                              + b.ToString("X2");

            return result;
        }


        /// <summary>
        /// Convert r,g,b,w int variables to string like "00AAFF99" or "#00AAFF99" (if useSharp)
        /// </summary>

        public static string ConvertRGBWIntsToHexString(int r, int g, int b, int w, bool useSharp = false)
        {
            string result = (useSharp) ? "#" : "";

            result += r.ToString("X2")
                              + g.ToString("X2")
                              + b.ToString("X2")
                              + w.ToString("X2");

            return result;
        }

        /// <summary>
        /// Convert r,g,b int array to string like "00AAFF" or "#00AAFF" (if useSharp)
        /// </summary>
        public static string ConvertRGBIntArrayToHexString(int[] rgb, bool useSharp = false)
        {
            return ConvertRGBIntsToHexString(rgb[0], rgb[1], rgb[2], useSharp);
        }


        /// <summary>
        /// Convert r,g,b,w int array to string like "00AAFF99" or "#00AAFF99" (if useSharp)
        /// </summary>

        public static string ConvertRGBWIntArrayToHexString(int[] rgbw, bool useSharp = false)
        {
            return ConvertRGBWIntsToHexString(rgbw[0], rgbw[1], rgbw[2], rgbw[3], useSharp);

        }
    }
}
