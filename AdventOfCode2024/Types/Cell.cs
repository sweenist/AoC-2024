using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Types;

public record Cell(Point parent)
{
    public Point Parent { get; set; } = parent;
    public int TotalScore = int.MaxValue;
    public int Accumulated = int.MaxValue;
    public int Heuristic { get; set; } = int.MaxValue;
}

