namespace MyNetSensors.Utils
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

        public static uint Map(uint value, uint fromMin, uint fromMax, uint toMin, uint toMax)
        {
            return (uint)Map((float)value, (float)fromMin, (float)fromMax, (float)toMin, (float)toMax);
        }

        public static int Map(int value, int fromMin, int fromMax, int toMin, int toMax)
        {
            return (int) Map((float) value, (float) fromMin, (float) fromMax, (float) toMin, (float) toMax);
        }

        public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        }
    }
}
