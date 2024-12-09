using System.Diagnostics;

namespace AdventOfCode2024.Utility;

public static class Diagnostics
{
    public static void Profile(Action action)
    {
        var stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        var oldColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Completed solution in {TimeFormat(stopwatch.Elapsed)}.");
        Console.ForegroundColor = oldColor;
    }

    private static string TimeFormat(TimeSpan t)
    {
        return t switch
        {
            var x when x < TimeSpan.FromMilliseconds(900) => $"{x:fff} milliseconds".TrimStart('0'),
            var x when x >= TimeSpan.FromMilliseconds(900) && x < TimeSpan.FromSeconds(60) => $"{x:s\\.fff} seconds",
            var x when x >= TimeSpan.FromSeconds(60) && x < TimeSpan.FromMinutes(90) => $"{x:m\\:ss} minutes",
            var x => $"{x:g}"
        };
    }
}
