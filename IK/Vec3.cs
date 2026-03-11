using System;

namespace ik;

public struct Vec3
{
    public float X;
    public float Y;
    public float Z;

    public Vec3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Vec3 operator +(Vec3 a, Vec3 b) => new Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vec3 operator -(Vec3 a, Vec3 b) => new Vec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vec3 operator *(Vec3 v, float s) => new Vec3(v.X * s, v.Y * s, v.Z * s);
    public static Vec3 operator /(Vec3 v, float s) => new Vec3(v.X / s, v.Y / s, v.Z / s);

    public float Length() => MathF.Sqrt(X * X + Y * Y + Z * Z);
    public float LengthSquared() => X * X + Y * Y + Z * Z;

    public Vec3 Normalized()
    {
        float len = Length();
        if (len < 1e-6f) return new Vec3(0, 0, 0);
        return this / len;
    }

    public static float Dot(Vec3 a, Vec3 b)
        => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    public static Vec3 Cross(Vec3 a, Vec3 b)
        => new Vec3(
            a.Y * b.Z - a.Z * b.Y,
            a.Z * b.X - a.X * b.Z,
            a.X * b.Y - a.Y * b.X
        );
}
