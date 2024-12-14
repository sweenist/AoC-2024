using System.Diagnostics;
using System.Text;
using AdventOfCode2024.Types;
using AdventOfCode2024.Utility.Math;

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

    public static string PrintMap(IEnumerable<ICoordinate> points, Boundary bounds)
    {
        var s = new StringBuilder();
        for (var y = 0; y < bounds.Height; y++)
        {
            for (var x = 0; x < bounds.Width; x++)
            {
                s.Append(points.Any(p => p.X == x && p.Y == y) ? '#' : '.');
            }
            s.Append('\n');
        }
        return s.ToString();
    }

    public static string PrintMapWithCounts(IEnumerable<ICoordinate> points, Boundary bounds)
    {
        var s = new StringBuilder();
        for (var y = 0; y < bounds.Height; y++)
        {
            for (var x = 0; x < bounds.Width; x++)
            {
                var pointCounts = points.Count(p => p.X == x && p.Y == y);
                s.Append(pointCounts > 0 ? pointCounts.ToString() : ".");
            }
            s.Append('\n');
        }
        return s.ToString();
    }
}
