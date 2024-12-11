using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode2024.Utility;

public struct Point(int X, int Y)
{
    public int X { get; set; } = X;
    public int Y { get; set; } = Y;

    public static Point operator +(Point source, Point target) => new(source.X + target.X, source.Y + target.Y);
    public static Point operator -(Point source, Point target) => new(target.X - source.X, target.Y - source.Y);
    public static Point operator +(Point source, Vector target) => new(source.X + target.X, source.Y + target.Y);
    public static Point operator -(Point source, Vector target) => new(target.X - source.X, target.Y - source.Y);

    public override string ToString()
    {
        return $"Point:<{X}, {Y}>";
    }
}

/// <summary>Kind of like <seealso cref="Point" but with magnitude and direction/></summary>
/// <param name="X">Left/Right component.</param>
/// <param name="Y">Up/Down component.</param>
/// <param name="Z">(Optional) Depth component.</param>
public struct Vector(int X, int Y, int? Z = null)
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

    public readonly bool Equals(Vector other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }
    public override readonly string ToString()
    {
        return $"Vector:<{X}, {Y}{(Z.HasValue ? $", {Z}" : "")}>";
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
