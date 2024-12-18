using AdventOfCode2024.Types;
using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Utility;

public class PriorityComparer : IComparer<(int, Actor)>
{
    public int Compare((int, Actor) x, (int, Actor) y)
    {
        var result = x.Item1.CompareTo(y.Item1);

        if (result == 0)
        {
            result = x.Item2.Location.CompareTo(y.Item2.Location);
            if (result == 0)
                return x.Item2.Direction.CompareTo(y.Item2.Direction);
        }
        return result;
    }
}
