namespace AdventOfCode2024.Utility;

public static class CollectionUtils
{
    public static bool HasAnyAdjacent<T>(this IEnumerable<T> target, T match)
    {
        var iterationLength = target.Count() - 1;
        var left = target.Take(iterationLength).ToArray();
        var right = target.Skip(1).ToArray();
        for (var i = 0; i < iterationLength; i++)
            if (left[i]!.Equals(match) && right[i]!.Equals(match))
                return true;
        return false;
    }
}
