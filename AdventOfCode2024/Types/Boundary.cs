
using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Types;
public record Boundary(int Height, int Width)
{
    public int Height { get; set; } = Height;
    public int Width { get; set; } = Width;
    public int BoundX => Width - 1;
    public int BoundY => Height - 1;

    public bool OutOfBounds(ICoordinate coordinate) => coordinate.X < 0 || coordinate.X > BoundX || coordinate.Y < 0 || coordinate.Y > BoundY;
}