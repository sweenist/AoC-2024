
namespace AdventOfCode2024.Types;
public record Boundary(int height, int width)
{
    public int Height { get; set; } = height;
    public int Width { get; set; } = width;
    public int BoundX => Width - 1;
    public int BoundY => Height - 1;
}