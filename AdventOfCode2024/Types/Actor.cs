using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Types;

public class Actor(Point Location, Vector Direction) : IComparable<Actor>
{
    public Point Location { get; set; } = Location;
    public Vector Direction { get; set; } = Direction;

    public int Compare(Actor? x, Actor? y)
    {
        var result = x.Location.CompareTo(y.Location);
        return result != 0 ? result : x.Direction.CompareTo(y.Direction.CompareTo);
    }
}
