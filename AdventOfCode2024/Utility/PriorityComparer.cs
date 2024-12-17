using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Utility;

public class PriorityComparer : IComparer<(int, Point)>
{
    public int Compare((int, Point) x, (int, Point) y)
    {
        var result = x.Item1.CompareTo(y.Item1);

        Console.WriteLine($"returning {result} for comparing {x} and {y}");
        if (result == 0)
            return x.Item2.Equals(y.Item2) ? 0 : -1;
        return result;
    }
}
