namespace AdventOfCode2024.Utility;

public record Point(int X, int Y)
{
    public int X { get; set; } = X;
    public int Y { get; set; } = Y;

    public static Point North => new Point(0, -1);

    public static Point East => new Point(1, 0);

    public static Point South => new Point(0, 1);

    public static Point West => new Point(-1, 0);

    public static List<Point> CardinalPoints => [North, East, South, West];


    public static Point operator +(Point source, Point target) => new(source.X + target.X, source.Y + target.Y);

    public static Point operator -(Point source, Point target) => new(target.X - source.X, target.Y - source.Y);

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
