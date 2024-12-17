using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Types;

public class Actor(Point Location, Vector Direction)
{
    public Point Location { get; set; } = Location;
    public Vector Direction { get; set; } = Direction;
}
