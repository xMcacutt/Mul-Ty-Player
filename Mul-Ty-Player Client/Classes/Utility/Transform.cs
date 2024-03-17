using System;

namespace MulTyPlayerClient.Classes.GamePlay;

internal interface ITransformData
{
    float[] AsFloats();
}

public struct Position : ITransformData
{
    private float[] data;
    public float X => data[0];
    public float Y => data[1];
    public float Z => data[2];

    public Position(float x, float y, float z)
    {
        data = new float[3] { x, y, z };
    }

    public Position() : this(0f, 0f, 0f)
    {
    }

    public Position(float[] position) : this(position[0], position[1], position[2])
    {
    }

    public void Set(float x, float y, float z)
    {
        data[0] = x;
        data[1] = y;
        data[2] = z;
    }

    public void Set(float[] newPosition)
    {
        data = newPosition[..3];
    }

    public float[] AsFloats()
    {
        return data;
    }
}

public struct Rotation : ITransformData
{
    private float[] data;
    public float Pitch => data[0];
    public float Yaw => data[1];
    public float Roll => data[2];

    public Rotation(float pitch, float yaw, float roll)
    {
        data = new[] { pitch, yaw, roll };
    }

    public Rotation() : this(0f, 0f, 0f)
    {
    }

    public Rotation(float[] rotation) : this(rotation[0], rotation[1], rotation[2])
    {
    }

    public void Set(float pitch, float yaw, float roll)
    {
        data[0] = pitch;
        data[1] = yaw;
        data[2] = roll;
    }

    public void Set(float[] newRotation)
    {
        data = newRotation[..3];
    }

    public float[] AsFloats()
    {
        return data;
    }
}

public class Transform
{
    public int LevelId;
    public bool OnMenu;
    public Position Position;
    public Rotation Rotation;

    public Transform(Position position, Rotation rotation, bool onMenu, int levelId)
    {
        this.Position = position;
        this.Rotation = rotation;
        this.OnMenu = onMenu;
        this.LevelId = levelId;
    }

    public Transform(float[] transform, bool onMenu, int levelId) : 
        this(new Position(transform[..3]), 
        new Rotation(transform[3..6]),
        onMenu, levelId)
    {
    }

    public Transform() : this(new Position(), new Rotation(), false, 0)
    {
    }
}

public record struct TransformSnapshot(Transform Transform, DateTime Timestamp)
{
    public TransformSnapshot(Transform transform) : this(transform, DateTime.Now)
    {
    }

    public TransformSnapshot(float[] transform, bool onMenu, int levelId) : this(new Transform(transform, onMenu, levelId), DateTime.Now)
    {
    }

    public TransformSnapshot() : this(new Transform(), DateTime.Now)
    {
    }
}

public class TransformSnapshots
{
    public TransformSnapshot New;
    public TransformSnapshot Old;

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