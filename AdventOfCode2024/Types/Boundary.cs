
using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Types;
public record Boundary
{
    public Boundary(int height, int width)
    {
        Height = height;
        Width = width;
    }
    public Boundary(Point dimension)
    {
        Height = dimension.Y + 1;
        Width = dimension.X + 1;
    }

    public int Height { get; set; }
    public int Width { get; set; }
    public int BoundX => Width - 1;
    public int BoundY => Height - 1;

    public bool OutOfBounds(ICoordinate coordinate) => coordinate.X < 0 || coordinate.X > BoundX || coordinate.Y < 0 || coordinate.Y > BoundY;
}