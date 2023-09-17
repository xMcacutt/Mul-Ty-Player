using System;

namespace MulTyPlayerClient.Classes.Networking
{
    public static class Interpolation
    {
        //Given two floats and their respective timestamps, predict what the float should be now
        //bTime should be AFTER aTime, otherwise it will produce the opposite result
        public static float PredictFloat(float a, DateTime aTime, float b, DateTime bTime)
        {
            double vel = (b - a) / bTime.Subtract(aTime).TotalMilliseconds;
            return (float)(b + (vel * DateTime.Now.Subtract(bTime).TotalMilliseconds));
        }

        public static void PredictFloatsNonAlloc(float[] f1, DateTime aTime, float[] f2, DateTime bTime, float[] result, int size)
        {
            double timeSinceB = DateTime.Now.Subtract(bTime).TotalMilliseconds;
            double timeBetween = bTime.Subtract(aTime).TotalMilliseconds;
            float mTime = (float)(timeSinceB / timeBetween) + 1f;
            for (int i = 0; i < size; i++)
            {
                result[i] = f2[i] + mTime * (f2[i] - f1[i]);
            }
        }

        public static void LerpFloatsNonAlloc(float[] f1, DateTime dt1, float[] f2, DateTime dt2, float[] result, int size)
        {
            double dtGap = (dt2 - dt1).TotalMilliseconds;
            double dtNow = (DateTime.Now - dt2).TotalMilliseconds;
            float t = (float)(dtNow / dtGap);
            if (t <= 0.0f)
            {
                f1.CopyTo(result, 0);
                return;
            }
            for (int i = 0; i < size; i++)
            {
                result[i] = Lerpf(f1[i], f2[i], t);
            }
        }

        public static void LerpFloatsNonAllocClamped(float[] f1, DateTime dt1, float[] f2, DateTime dt2, float[] result, int size)
        {
            double dtGap = (dt2 - dt1).TotalMilliseconds;
            double dtNow = (DateTime.Now - dt2).TotalMilliseconds;
            float t = (float)(dtNow / dtGap);

            if (t <= 0.0f)
            {
                f1.CopyTo(result, 0);
                return;
            }
            else if (t >= 1.0f)
            {
                f2.CopyTo(result, 0);
                return;
            }

            for (int i = 0; i < size; i++)
            {
                result[i] = Lerpf(f1[i], f2[i], t);
            }
        }

        private static float Lerpf(float f1, float f2, float t)
        {
            return f1 * (1 - t) + f2 * t;
        }
    }

    public enum InterpolationMode
    {
        None,
        Interpolate,
        Extrapolate,
        Predictive
    }
}
