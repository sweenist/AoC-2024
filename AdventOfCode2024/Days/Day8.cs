using System.Text;
using AdventOfCode2024.Utility;
using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Days;

public class Day8 : IDay
{
    private string _example = @"............
........0...
.....0......
.......0....
....0.......
......A.....
............
............
........A...
.........A..
............
............";

    private readonly Dictionary<char, IEnumerable<PointPair>> _frequencies = [];
    private readonly int _mapWidth;
    private readonly int _mapHeight;

    public Day8(bool useExample = false)
    {
        List<string> input = [];
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            input.AddRange(_example.Split('\n'));
        }
        else
        {
            var inputFile = $"inputData/{GetType().Name}.txt";
            using var sr = new StreamReader(inputFile);
            input.AddRange(sr.ReadToEnd().Split('\n'));
        }
        _frequencies = ParseInputs(input);
        _mapHeight = input.Count;
        _mapWidth = input[0].Length;
    }

    public void Part1()
    {
        var antiNodes = IdentifyAntiNodes(shouldResonate: false);

        Console.WriteLine($"Found {antiNodes.Count} distinct anti-nodes in map.");
    }

    public void Part2()
    {
        var antiNodes = IdentifyAntiNodes(shouldResonate: true);

        Console.WriteLine($"Found {antiNodes.Count} resonating anti-nodes in map.");
    }

    private static Dictionary<char, IEnumerable<PointPair>> ParseInputs(List<string> inputs)
    {
        return inputs.SelectMany((row, i) => row.Select((col, j) => (Key: col, Point: new Point(j, i))))
            .Where(x => x.Key != '.')
            .GroupBy(x => x.Key)
            .ToDictionary(k => k.Key, v => v.Select(x => x.Point).Pairwise());
    }

    private HashSet<Point> IdentifyAntiNodes(bool shouldResonate)
    {
        var nodes = new HashSet<Point>();
        foreach (var key in _frequencies.Keys.ToList())
        {
            var antiNodes = _frequencies[key].SelectMany(p => Resonate(p, shouldResonate))
                .ToList();
            nodes.UnionWith(antiNodes);
        }
        return nodes;
    }

    private IEnumerable<Point> Resonate(PointPair pair, bool resonate = true)
    {
        bool inBounds(Point p) => p.X >= 0 && p.X < _mapWidth && p.Y >= 0 && p.Y < _mapHeight;

        var slope = pair.GetSlopeDistance();
        var (Back, Forward, A, B) = (true, true, pair.A, pair.B);
        if (resonate)
        {
            yield return A;
            yield return B;
        }

        while (Back || Forward)
        {
            A = slope - A;
            B += slope;
            Back = inBounds(A);
            Forward = inBounds(B);

            if (Back) yield return A;
            if (Forward) yield return B;
            if (!resonate) break;
        }
    }

    /// <summary>Prints the map of nodes for debugging.</summary>
    /// <param name="antiNodes">The nodes to draw on map.</param>
    private void Print(IEnumerable<Point> antiNodes)
    {
        var s = new StringBuilder();
        for (var y = 0; y < _mapHeight; y++)
        {
            for (var x = 0; x < _mapWidth; x++)
            {
                s.Append(antiNodes.Any(p => p.X == x && p.Y == y) ? '#' : '.');
            }
            s.Append('\n');
        }
        var map = s.ToString();
        Console.WriteLine($"Antinodes map:\n{map}");

        Console.WriteLine();
    }
}
