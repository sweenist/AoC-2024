using AdventOfCode2024.Utility;

namespace AdventOfCode2024.Days;

public class Day10 : IDay
{
    private string _example = @"89010123
78121874
87430965
96549874
45678903
32019012
01329801
10456732";

    private readonly List<string> _input = [];

    private List<List<KeyValuePair<int, Point>>> _map;

    public Day10(bool useExample = false)
    {
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            _input.AddRange(_example.Split('\n'));
        }
        else
        {
            var inputFile = $"inputData/{GetType().Name}.txt";
            using var sr = new StreamReader(inputFile);
            _input.AddRange(sr.ReadToEnd().Split('\n'));
        }
        _map = _input.Select((x, i) =>
                        x.Select((c, j) => new KeyValuePair<int, Point>(int.Parse(c.ToString()), new Point(j, i)))
                        .ToList())
                    .ToList();
    }

    public void Part1()
    {
        var trailHeads = _map.SelectMany(x => x.Where(k => k.Key == 0)).ToList();

        var result = Traverse(trailHeads);

        Console.WriteLine($"There are {result} trailheads");
    }

    public void Part2()
    {
        throw new NotImplementedException();
    }

    private long Traverse(List<KeyValuePair<int, Point>> trailMarkers)
    {
        var nextMarkers = new List<HashSet<KeyValuePair<int, Point>>>();
        foreach (var point in trailMarkers.Select(x => x.Value))
            nextMarkers.Add(new HashSet<KeyValuePair<int, Point>>(AdvanceTrail(point, 1).ToList()));

        // foreach (var item in nextMarkers)
        //     Console.WriteLine($"trails:\n\t{string.Join("\n\t", item.Select(x => $"{x.Key}, {x.Value}"))}");
        return nextMarkers.SelectMany(x => x.Where(k => k.Key == 9)).Count();
    }

    private IEnumerable<KeyValuePair<int, Point>> AdvanceTrail(Point trailSegment, int nextSegment)
    {
        foreach (var direction in Point.CardinalPoints)
        {
            var searchPoint = trailSegment + direction;
            if (searchPoint.X < 0 || searchPoint.X == _map[0].Count || searchPoint.Y < 0 || searchPoint.Y == _map.Count)
                continue;

            var mapSegment = _map[searchPoint.Y][searchPoint.X];
            if (mapSegment.Key == nextSegment)
                if (nextSegment == 9) yield return new KeyValuePair<int, Point>(nextSegment, searchPoint);
                else
                {
                    foreach (var item in AdvanceTrail(searchPoint, nextSegment + 1))
                        yield return item;
                }
        }
    }
}