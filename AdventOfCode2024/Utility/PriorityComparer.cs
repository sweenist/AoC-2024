namespace AdventOfCode2024.Utility;

public class PriorityComparer<T> : IComparer<T> where T : Tuple<IComparable, IComparable>
{
    // public int Compare((int, IComparable) x, (int, IComparable) y)
    // {
    //     var result = x.Item1.CompareTo(y.Item1);

    //     return result == 0 ? x.Item2.CompareTo(y.Item2) : result;
    // }

    public int Compare(T? x, T? y)
    {
        var result = x?.Item1.CompareTo(y?.Item1) ?? 0;

        return result == 0 ? (x?.Item2.CompareTo(y?.Item2) ?? 0) : result;
    }
}
