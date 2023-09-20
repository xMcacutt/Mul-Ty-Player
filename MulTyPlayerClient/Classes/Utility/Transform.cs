using MulTyPlayerClient.Classes.Networking;
using System;
using System.Diagnostics;

namespace MulTyPlayerClient.Classes.GamePlay
{
    interface ITransformData
    {
        float[] AsFloats();
    }
    public struct Position : ITransformData
    {
        float[] data;
        public float X => data[0];
        public float Y => data[1];
        public float Z => data[2];

        public Position(float x, float y, float z)
        {
            data = new float[3] { x, y, z };
        }
        public Position() : this(0f, 0f, 0f) { }
        public Position(float[] position) : this(position[0], position[1], position[2]) { }

        public void Set(float x, float y, float z)
        {
            data[0] = x;
            data[1] = y;
            data[2] = z;
        }

        public void Set(float[] newPosition)
        {
            data = newPosition[0..3];
        }

        public float[] AsFloats()
        {
            return data;
        }
    }

    public struct Rotation : ITransformData
    {
        float[] data;
        public float Pitch => data[0];
        public float Yaw => data[1];
        public float Roll => data[2];

        public Rotation(float pitch, float yaw, float roll)
        {
            data = new float[] { pitch, yaw, roll };
        }
        public Rotation() : this(0f, 0f, 0f) { }
        public Rotation(float[] rotation) : this(rotation[0], rotation[1], rotation[2]) { }

        public void Set(float pitch, float yaw, float roll)
        {
            data[0] = pitch;
            data[1] = yaw;
            data[2] = roll;
        }

        public void Set(float[] newRotation)
        {
            data = newRotation[0..3];
        }

        public float[] AsFloats()
        {
            return data;
        }
    }

    public class Transform
    {
        public Position Position;
        public Rotation Rotation;

        public Transform(Position Position, Rotation Rotation)
        {
            this.Position = Position; this.Rotation = Rotation;
        }
        public Transform(float[] transform) : this(new Position(transform[0..3]), new Rotation(transform[3..6])) { }
        public Transform() : this(new Position(), new Rotation()) { }
    }

    public record struct TransformSnapshot(Transform Transform, DateTime Timestamp)
    {
        public TransformSnapshot(Transform transform) : this(transform, DateTime.Now) { }
        public TransformSnapshot(float[] transform) : this(new(transform), DateTime.Now) { }
        public TransformSnapshot() : this(new Transform(), DateTime.Now) { }
    }

    public class TransformSnapshots
    {
        public TransformSnapshot Old;
        public TransformSnapshot New;

        public TransformSnapshots()
        {
            Old = new TransformSnapshot();
            New = new TransformSnapshot();
        }

        public void Update(TransformSnapshot newTransformSnapshot)
        {
            Old = New;
            New = newTransformSnapshot;
        }
    }
}
