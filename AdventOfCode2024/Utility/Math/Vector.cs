using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode2024.Utility.Math;

/// <summary>Kind of like <seealso cref="Point" but with magnitude and direction/></summary>
/// <param name="X">Left/Right component.</param>
/// <param name="Y">Up/Down component.</param>
/// <param name="Z">(Optional) Depth component.</param>
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable CS0661
public struct Vector(int X, int Y, int? Z = null) : ICoordinate
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning restore CS0661
{
    public int X { get; set; } = X;
    public int Y { get; set; } = Y;
    public int? Z { get; set; } = Z;

    public static Vector Zero => new(0, 0);

    public static Vector North => new(0, -1);
    public static Vector East => new(1, 0);
    public static Vector South => new(0, 1);
    public static Vector West => new(-1, 0);

    public static Vector NorthEast => North + East;
    public static Vector NorthWest => North + West;
    public static Vector SouthEast => South + East;
    public static Vector SouthWest => South + West;

    public static List<Vector> CardinalPoints => [North, East, South, West];
    public static List<Vector> OrdinalPoints => [NorthWest, NorthEast, SouthWest, SouthEast];
    public static List<Vector> OmniDirections => CardinalPoints.Zip(OrdinalPoints, (f, s) => new[] { f, s })
                                                               .SelectMany(x => x)
                                                               .ToList();

    public static Vector operator +(Vector source, Vector target) => new(source.X + target.X, source.Y + target.Y);
    public static Vector operator *(Vector source, int factor) => new(source.X * factor, source.Y * factor);

    public static bool operator ==(Vector a, Vector b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(Vector a, Vector b) => a.X != b.X || a.Y != b.Y;

    public static Vector Delta(ICoordinate a, ICoordinate b) => new(a.X - b.X, a.Y - b.Y);

    public static Vector Clockwise(Vector v) => new(-v.Y, v.X);

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is not Vector)
            return false;
        return base.Equals(obj);
    }
    public readonly bool Equals(Vector other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }
    public override readonly string ToString()
    {
        return $"Vector:<{X}, {Y}{(Z.HasValue ? $", {Z}" : "")}>";
    }
}

public static class VectorExtensions
{
    public static Vector Invert(this Vector v) => new(-v.X, -v.Y);

    public static Vector Clockwise(this Vector v) => new(-v.Y, v.X);
    public static Vector AntiClockwise(this Vector v) => new(v.Y, -v.X);
}


