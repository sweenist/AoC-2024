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
        Console.WriteLine($"Completed solution in {stopwatch.Elapsed:s\\.ffffff} seconds.");
        Console.ForegroundColor = oldColor;
    }
}
