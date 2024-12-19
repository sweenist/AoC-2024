using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Types;

public class Actor(Point Location, Vector Direction) : IComparable<Actor>
{
    public Point Location { get; set; } = Location;
    public Vector Direction { get; set; } = Direction;

    public override string ToString()
    {
        return $"{Location} {Direction}";
    }

    public int CompareTo(Actor? y)
    {
        if (y is null) return 0;
        var result = Location.CompareTo(y.Location);
        return result != 0 ? result : Direction.CompareTo(y.Direction);
    }
}
