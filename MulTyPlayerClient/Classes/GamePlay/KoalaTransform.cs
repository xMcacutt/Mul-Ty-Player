using MulTyPlayerClient.Classes.Networking;
using System;
using System.Diagnostics;

namespace MulTyPlayerClient.Classes.GamePlay
{
    
    struct PositionSnapshot
    {
        public float[] Position;
        public DateTime Timestamp;

        public PositionSnapshot()
        {
            Position = new float[3];
            Timestamp = DateTime.MinValue;
        }

        public PositionSnapshot(float[] transform)
        {
            Position = new float[3];
            Position[0] = transform[0];
            Position[1] = transform[1];
            Position[2] = transform[2];

            Timestamp = DateTime.Now;
        }
    }

    class KoalaTransform
    {
        public PositionSnapshot Old;
        public PositionSnapshot New;
        public float[] Position;
        public float[] Rotation;

        public KoalaTransform()
        {
            Old = new PositionSnapshot();
            New = new PositionSnapshot();
            Position = new float[3];
            Rotation = new float[3];
        }

        public void UpdatePosition(PositionSnapshot ps)
        {
            Old = New;
            New = ps;
        }

        public float[] GetPosition(KoalaInterpolationMode mode)
        {
            switch (mode)
            {
                case KoalaInterpolationMode.None:
                    {
                        //Do nothing, write coords as received
                        //Performant, reliable, but looks stuttery
                        return New.Position;
                    }
                case KoalaInterpolationMode.Interpolate:
                    {
                        // Lerp between the second last transform and the last transform received, based on how much time has passed.
                        // Stops once the last transform has been reached with no further packets received
                        // Looks smoother, but costs cpu, and adds latency to movement (~20-30ms)
                        Interpolation.LerpFloatsNonAllocClamped(
                            Old.Position, Old.Timestamp,
                            New.Position, New.Timestamp,
                            Position, 3);
                        break;
                    }
                case KoalaInterpolationMode.Extrapolate:
                    {
                        // Lerp between the second last transform and the last transform received, based on how much time has passed.
                        // Does not stop upon reaching the latest transform, attempts to extrapolate further
                        // Looks smoother, but costs cpu, and adds latency to movement (~20-30ms)
                        // May be better than interpolate for players with shaky connections                        
                        Interpolation.LerpFloatsNonAlloc(
                            Old.Position, Old.Timestamp,
                            New.Position, New.Timestamp,
                            Position, 3);
                        break;
                    }
                case KoalaInterpolationMode.Predictive:
                    {
                        // Predict and extrapolate the koalas movement based on the last two transforms received
                        // Smoother than none, no latency, but costs cpu
                        // Could be unpredictable/innaccurate with shaky connections
                        Interpolation.PredictFloatsNonAlloc(
                            Old.Position, Old.Timestamp,
                            New.Position, New.Timestamp,
                            Position, 3);
                        break;
                    }
            }
            return Position;
        }

        public float[] GetRotation()
        {
            return Rotation;
        }

        internal void UpdateRotation(float[] rotation)
        {
            Rotation = rotation;
        }
    }
}
