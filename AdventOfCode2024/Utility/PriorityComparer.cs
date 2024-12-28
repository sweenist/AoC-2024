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

public class PriorityComparer<T> : IComparer<(int, T)> where T : Actor
{
    public int Compare((int, T) x, (int, T) y)
    {
        var result = x.Item1.CompareTo(y.Item1);

        return result == 0 ? x.Item2.CompareTo(y.Item2) : result;
    }
}
