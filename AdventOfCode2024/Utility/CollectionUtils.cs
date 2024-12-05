namespace AdventOfCode2024.Utility;

public static class CollectionUtils
{
    public static T Middle<T>(this IEnumerable<T> target)
    {
        var tList = target.ToList();
        var middleIndex = tList.Count % 2 == 1 ? (tList.Count - 1) / 2 : tList.Count / 2;
        return tList[middleIndex];
    }
}
