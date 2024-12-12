namespace AdventOfCode2024.Utility;

public static class CollectionUtils
{
    public static T Middle<T>(this IEnumerable<T> target)
    {
        var tList = target.ToList();
        var middleIndex = tList.Count % 2 == 1 ? (tList.Count - 1) / 2 : tList.Count / 2;
        return tList[middleIndex];
    }

    public static IEnumerable<long> SumProductTree(this IEnumerable<long> source)
    {
        using var e = source.GetEnumerator();
        if (!e.MoveNext()) return [];

        List<long> lefts = [e.Current];
        List<long> newNodes = [];

        while (e.MoveNext())
        {
            newNodes = lefts.SelectMany(n => new[] { n + e.Current, n * e.Current }).ToList();
            lefts.Clear();
            lefts.AddRange(newNodes);
        }
        return newNodes;
    }

    public static IEnumerable<long> SumProductConcatenate(this IEnumerable<long> source)
    {
        using var e = source.GetEnumerator();
        if (!e.MoveNext()) return [];

        List<long> lefts = [e.Current];
        List<long> newNodes = [];

        while (e.MoveNext())
        {
            newNodes = lefts.SelectMany(n => new[] {
                n + e.Current,
                n * e.Current,
                long.Parse(string.Concat([n.ToString(), e.Current.ToString()]))
            }).ToList();
            lefts.Clear();
            lefts.AddRange(newNodes);
        }
        return newNodes;//.Where(n => n <= max);
    }

    public static IEnumerable<PointPair> Pairwise(this IEnumerable<Point> points)
    {
        var pairs = new List<PointPair>();
        var p = points.ToList();

        for (var i = 0; i < p.Count; i++)
        {
            var left = p.Skip(i + 1);
            if (left.Any())
                pairs.AddRange(left.Select(x => new PointPair(p[i], x)));
        }
        return pairs;
    }
}
