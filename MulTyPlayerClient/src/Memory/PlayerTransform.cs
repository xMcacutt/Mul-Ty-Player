using MulTyPlayerCommon;
using System.Runtime.InteropServices;
using MulTyPlayerClient.Networking;
using System.Windows.Automation;

namespace MulTyPlayerClient.Memory
{
    public class PlayerTransform
    {
        static int positionAddress = Addresses.TY_POSITION_ADDRESS;
        static int rotationAddress = Addresses.TY_ROTATION_ADDRESS;

        public static float[] Position => transform.Position;
        public static float[] Rotation => transform.Rotation;

        private static Transform transform;
        [StructLayout(LayoutKind.Explicit)]
        private struct Transform
        {
            public Transform()
            {
                Position = new float[3];
                Rotation = new float[3];
            }

            [FieldOffset(0)]
            public float[] Position = new float[3];
            [FieldOffset(12)]
            public float[] Rotation = new float[3];
        }

        public static void Update()
        {
            ProcessHandler.TryReadBytes(positionAddress, transform.Position, 12, true);
            ProcessHandler.TryReadBytes(rotationAddress, transform.Rotation, 12, true);
        }

        public static void CheckOutbackSafari(int levelId)
        {
            if (levelId == Levels.OutbackSafari.Id)
            {
                positionAddress = Addresses.BP_POSITION_ADDRESS;
                rotationAddress = Addresses.BP_ROTATION_ADDRESS;
            }
            else
            {
                positionAddress = Addresses.TY_POSITION_ADDRESS;
                rotationAddress = Addresses.TY_ROTATION_ADDRESS;
            }
        }
    }
}
