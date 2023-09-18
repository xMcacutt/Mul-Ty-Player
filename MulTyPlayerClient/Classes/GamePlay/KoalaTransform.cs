using MulTyPlayerClient.Classes.Networking;
using System;
using System.Diagnostics;

namespace MulTyPlayerClient.Classes.GamePlay
{
    
    struct TransformSnapshot
    {
        public float[] Transform;
        public DateTime Timestamp;

        public TransformSnapshot()
        {
            Transform = new float[KoalaTransform.TRANSFORM_MEMBER_COUNT];
            Timestamp = DateTime.MinValue;
        }

        public TransformSnapshot(float[] transform)
        {
            Transform = transform;
            Timestamp = DateTime.Now;
        }
    }

    class KoalaTransform
    {
        public const int TRANSFORM_MEMBER_COUNT = 6;

        public TransformSnapshot Old;
        public TransformSnapshot New;
        public float[] ResultTransform;

        public KoalaTransform()
        {
            Old = new TransformSnapshot();
            New = new TransformSnapshot();
            ResultTransform = new float[6];
        }

        public void Update(TransformSnapshot newTS)
        {
            Old = New;
            New = newTS;
        }

        public float[] GetTransform(KoalaInterpolationMode mode)
        {
            switch (mode)
            {
                case KoalaInterpolationMode.None:
                    {
                        //Do nothing, write coords as received
                        //Performant, reliable, but looks stuttery
                        return New.Transform;
                    }
                case KoalaInterpolationMode.Interpolate:
                    {
                        // Lerp between the second last transform and the last transform received, based on how much time has passed.
                        // Stops once the last transform has been reached with no further packets received
                        // Looks smoother, but costs cpu, and adds latency to movement (~20-30ms)
                        Interpolation.LerpFloatsNonAllocClamped(
                            Old.Transform, Old.Timestamp,
                            New.Transform, New.Timestamp,
                            ResultTransform, TRANSFORM_MEMBER_COUNT);
                        break;
                    }
                case KoalaInterpolationMode.Extrapolate:
                    {
                        // Lerp between the second last transform and the last transform received, based on how much time has passed.
                        // Does not stop upon reaching the latest transform, attempts to extrapolate further
                        // Looks smoother, but costs cpu, and adds latency to movement (~20-30ms)
                        // May be better than interpolate for players with shaky connections                        
                        Interpolation.LerpFloatsNonAlloc(
                            Old.Transform, Old.Timestamp,
                            New.Transform, New.Timestamp,
                            ResultTransform, TRANSFORM_MEMBER_COUNT);
                        break;
                    }
                case KoalaInterpolationMode.Predictive:
                    {
                        // Predict and extrapolate the koalas movement based on the last two transforms received
                        // Smoother than none, no latency, but costs cpu
                        // Could be unpredictable/innaccurate with shaky connections
                        Interpolation.PredictFloatsNonAlloc(
                            Old.Transform, Old.Timestamp,
                            New.Transform, New.Timestamp,
                            ResultTransform, TRANSFORM_MEMBER_COUNT);
                        break;
                    }
            }
            return ResultTransform;
        }

        public static string DebugTransform(float[] transform)
        {
            return $"(x: {transform[0]}, y: {transform[1]}, z: {transform[2]}, pitch: {transform[3]}, yaw: {transform[4]}, roll: {transform[5]})";

        }
    }
}
