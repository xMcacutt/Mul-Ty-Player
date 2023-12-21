using System;
using MulTyPlayerClient.Classes.GamePlay;

namespace MulTyPlayerClient.Classes.Networking;

public static class Interpolation
{
    public static float[] PredictFloats(float[] f1, DateTime dt1, float[] f2, DateTime dt2, int size)
    {
        var result = new float[size];
        PredictFloatsNonAlloc(f1, dt1, f2, dt2, ref result, size);
        return result;
    }

    public static void PredictFloatsNonAlloc(float[] f1, DateTime aTime, float[] f2, DateTime bTime, ref float[] result,
        int size)
    {
        var timeSinceB = DateTime.Now.Subtract(bTime).TotalMilliseconds;
        var timeBetween = bTime.Subtract(aTime).TotalMilliseconds;
        var mTime = (float)(timeSinceB / timeBetween) + 1f;
        for (var i = 0; i < size; i++) result[i] = f2[i] + mTime * (f2[i] - f1[i]);
    }

    public static float[] LerpFloats(float[] f1, DateTime dt1, float[] f2, DateTime dt2, int size)
    {
        var result = new float[size];
        LerpFloatsNonAlloc(f1, dt1, f2, dt2, ref result, size);
        return result;
    }

    public static void LerpFloatsNonAlloc(float[] f1, DateTime dt1, float[] f2, DateTime dt2, ref float[] result,
        int size)
    {
        var dtGap = (dt2 - dt1).TotalMilliseconds;
        var dtNow = (DateTime.Now - dt2).TotalMilliseconds;
        var t = (float)(dtNow / dtGap);
        if (t <= 0.0f)
        {
            f1.CopyTo(result, 0);
            return;
        }

        for (var i = 0; i < size; i++) result[i] = Lerpf(f1[i], f2[i], t);
    }

    public static Position LerpPosition(TransformSnapshots ts, KoalaInterpolationMode mode)
    {
        var pos = new float[3];
        switch (mode)
        {
            case KoalaInterpolationMode.None:
            {
                pos = ts.New.Transform.Position.AsFloats();
                break;
            }
            case KoalaInterpolationMode.Interpolate:
            {
                pos = LerpFloatsClamped(
                    ts.Old.Transform.Position.AsFloats(), ts.Old.Timestamp,
                    ts.New.Transform.Position.AsFloats(), ts.New.Timestamp,
                    3);
                break;
            }
            case KoalaInterpolationMode.Extrapolate:
            {
                pos = LerpFloats(
                    ts.Old.Transform.Position.AsFloats(), ts.Old.Timestamp,
                    ts.New.Transform.Position.AsFloats(), ts.New.Timestamp,
                    3);
                break;
            }
            case KoalaInterpolationMode.Predictive:
            {
                pos = PredictFloats(
                    ts.Old.Transform.Position.AsFloats(), ts.Old.Timestamp,
                    ts.New.Transform.Position.AsFloats(), ts.New.Timestamp,
                    3);
                break;
            }
        }

        return new Position(pos);
    }

    public static float[] LerpFloatsClamped(float[] f1, DateTime timestamp1, float[] f2, DateTime timestamp2, int size)
    {
        var result = new float[size];
        LerpFloatsNonAllocClamped(f1, timestamp1, f2, timestamp2, ref result, size);
        return result;
    }

    public static void LerpFloatsNonAllocClamped(float[] f1, DateTime dt1, float[] f2, DateTime dt2, ref float[] result,
        int size)
    {
        var dtGap = (dt2 - dt1).TotalMilliseconds;
        var dtNow = (DateTime.Now - dt2).TotalMilliseconds;
        var t = (float)(dtNow / dtGap);

        if (t <= 0.0f)
        {
            f1.CopyTo(result, 0);
            return;
        }

        if (t >= 1.0f)
        {
            f2.CopyTo(result, 0);
            return;
        }

        for (var i = 0; i < size; i++) result[i] = Lerpf(f1[i], f2[i], t);
    }

    private static float Lerpf(float f1, float f2, float t)
    {
        return f1 * (1 - t) + f2 * t;
    }
}

public enum KoalaInterpolationMode
{
    //Do nothing, write coords as received
    //Performant, reliable, but looks stuttery
    None,

    // Lerp between the second last transform and the last transform received, based on how much time has passed.
    // Stops once the last transform has been reached with no further packets received
    // Looks smoother, but costs cpu, and adds latency to movement (~20-30ms)
    Interpolate,

    // Lerp between the second last transform and the last transform received, based on how much time has passed.
    // Does not stop upon reaching the latest transform, attempts to extrapolate further
    // Looks smoother, but costs cpu, and adds latency to movement (~20-30ms)
    // May be better than interpolate for players with shaky connections   
    Extrapolate,

    // Predict and extrapolate the koalas movement based on the last two transforms received
    // Smoother than none, no latency, but costs cpu
    // Could be unpredictable/innaccurate with shaky connections
    Predictive
}