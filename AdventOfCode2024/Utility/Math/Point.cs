using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode2024.Utility.Math;
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable CS0661
public struct Point(int X, int Y) : ICoordinate
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning restore CS0661
{
    public int X { get; set; } = X;
    public int Y { get; set; } = Y;

    public static Point operator +(Point source, Point target) => new(source.X + target.X, source.Y + target.Y);
    public static Point operator -(Point source, Point target) => new(target.X - source.X, target.Y - source.Y);
    public static Point operator +(Point source, Vector target) => new(source.X + target.X, source.Y + target.Y);
    public static Point operator -(Point source, Vector target) => new(target.X - source.X, target.Y - source.Y);
    public static bool operator ==(Point a, Point b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(Point a, Point b) => a.X != b.X || a.Y != b.Y;

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is not Point)
            return false;
        return base.Equals(obj);
    }

    public readonly bool Equals(Point other)
    {
        return X == other.X && Y == other.Y;
    }
    public override string ToString()
    {
        return $"Point:<{X}, {Y}>";
    }
}

public record PointPair(Point A, Point B)
{
    public Point A { get; set; } = A;
    public Point B { get; set; } = B;

    public Point GetSlopeDistance()
    {
        return A - B;
    }

    public override string ToString()
    {
        return $"A:<{A}> B:<{B}>";
    }
}


