using AdventOfCode2024.Types;
using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Utility;

public class AStarComparer : IComparer<(int, Point)>
{
    public int Compare((int, Point) x, (int, Point) y)
    {
        var result = x.Item1.CompareTo(y.Item1);

        return result == 0 ? x.Item2.CompareTo(y.Item2) : result;
    }
}

public class PriorityComparer : IComparer<(int, Actor)>
{
    public int Compare((int, Actor) x, (int, Actor) y)
    {
        var result = x.Item1.CompareTo(y.Item1);

        return result == 0 ? x.Item2.CompareTo(y.Item2) : result;
    }
}
