namespace AdventOfCode2024.Utility;

public record Point(int X, int Y)
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
public record Vector(int X, int Y, int? Z = null)
{
    public int X { get; set; } = X;
    public int Y { get; set; } = Y;
    public int? Z { get; set; } = Z;

    public static Vector North => new(0, -1);
    public static Vector East => new(1, 0);
    public static Vector South => new(0, 1);
    public static Vector West => new(-1, 0);
    
    public static List<Vector> CardinalPoints => [North, East, South, West];

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
