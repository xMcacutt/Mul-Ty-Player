using System;

namespace MulTyPlayerClient.Classes.Networking
{
    public static class Interpolation
    {
        //Given two floats and their respective timestamps, predict what the float should be now
        //bTime should be AFTER aTime, otherwise it will produce the opposite result
        public static float PredictFloat(float a, DateTime aTime, float b, DateTime bTime)
        {
            float vel = (b - a) / bTime.Subtract(aTime).Milliseconds;
            return b + (vel * DateTime.Now.Subtract(bTime).Milliseconds);
        }

        public static float[] LerpFloats(float[] f1, float[] f2, float t, int size)
        {
            float[] result = new float[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = Lerpf(f1[i], f2[i], t);
            }
            return result;
        }

        public static void LerpFloatsNonAlloc(float[] f1, float[] f2, float t, float[] result, int size)
        {
            for (int i = 0; i < size; i++)
            {
                result[i] = Lerpf(f1[i], f2[i], t);
            }
        }

        //Use a value of t between 0 -> 1 to lerp from a -> b
        //This is not clamped so using other values will result in extrapolation
        private static float Lerpf(float a, float b, float t)
        {
            return a * t + (1 - t) * b;
        }
    }
}
