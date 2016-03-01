namespace MyNodes.Utils
{
    public static class MathUtils
    {
        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }


        public static uint Clamp(uint value, uint min, uint max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }


        public static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static double Clamp(double value, double min, double max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static uint Remap(uint value, uint fromMin, uint fromMax, uint toMin, uint toMax)
        {
            return (uint)Map(value, fromMin, fromMax, toMin, toMax);
        }

        public static int Map(int value, int fromMin, int fromMax, int toMin, int toMax)
        {
            return (int)Map(value, fromMin, fromMax, toMin, toMax);
        }

        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return (float)Map(value, fromMin, fromMax, toMin, toMax);
        }

        public static double Remap(double value, double fromMin, double fromMax, double toMin, double toMax)
        {
            return (double)Map(value, fromMin, fromMax, toMin, toMax);
        }

        public static double Map(double value, double fromMin, double fromMax, double toMin, double toMax)
        {
            return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        }
    }
}
