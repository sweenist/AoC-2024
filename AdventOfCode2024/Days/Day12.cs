using AdventOfCode2024.Types;
using AdventOfCode2024.Utility;

namespace AdventOfCode2024.Days;

public class Day12 : IDay
{
    private string _example = @"RRRRIICCFF
RRRRIICCCF
VVRRRCCFFF
VVRCCCJFFF
VVVVCJJCFE
VVIVCCJJEE
VVIIICJJEE
MIIIIIJJEE
MIIISIJEEE
MMMISSJEEE";

    private readonly List<string> _input = [];
    private readonly Boundary _bounds;

    public Day12(bool useExample = false)
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
        _bounds = new Boundary(_input.Count, _input[0].Length);
    }

    public void Part1()
    {
        var plotPoints = _input.SelectMany((y, i) => y.TrimEnd().Select((x, j) => new Plot(x, new Point(j, i))));
        var result = Map(plotPoints);
        var totalCost = result.Sum(x => x.Value.Price);
        // Console.WriteLine($"Regions are: {string.Join("\n\n", result.Values)}");

        Console.WriteLine($"Total fence costs are {totalCost}");
    }

    public void Part2()
    {
        throw new NotImplementedException();
    }

    private Dictionary<int, Region> Map(IEnumerable<Plot> sections)
    {
        var regions = new Dictionary<int, Region>();
        var visited = sections.Select(p => new KeyValuePair<Point, bool>(p.Location, false)).ToDictionary();

        void Traverse(Plot plot, int searchId, Vector previous)
        {
            visited[plot.Location] = true;
            if (!regions.TryGetValue(searchId, out var _))
            {
                var region = new Region();
                region.Plots.Add(plot);
                regions[searchId] = region;
            }

            foreach (var cardinal in Vector.CardinalPoints.Where(v => v != previous))
            {
                var adjacent = plot.Location + cardinal;
                if (adjacent.X < 0 || adjacent.X > _bounds.BoundX || adjacent.Y < 0 || adjacent.Y > _bounds.BoundY)
                    continue;

                var adjacentPlot = sections.Single(x => x.Location == adjacent);
                if (adjacentPlot.Id == plot.Id)
                {
                    adjacentPlot.Neighbours.Add(plot);
                    plot.Neighbours.Add(adjacentPlot);

                    if (!visited[adjacentPlot.Location])
                    {
                        regions[searchId].Plots.Add(adjacentPlot);
                        Traverse(adjacentPlot, searchId, cardinal.Invert());
                    }
                }
            }
        }

        for (var y = 0; y < _bounds.BoundY; y++)
            for (var x = 0; x < _bounds.BoundX; x++)
            {
                var point = new Point(x, y);
                if (visited[point]) continue;

                var searchId = y * _bounds.Width + x;
                var plot = sections.Single(x => x.Location == point);
                Traverse(plot, searchId, Vector.Zero);
            }

        return regions;
    }

    private record Plot(char Id, Point Location)
    {
        public char Id { get; } = Id;
        public Point Location { get; } = Location;
        public HashSet<Plot> Neighbours { get; set; } = [];

        public int Perimeter => 4 - Neighbours.Count;
    }

    private record Region
    {
        public List<Plot> Plots { get; set; } = [];
        int Perimeter => Plots.Select(p => p.Perimeter).Sum();
        int Area => Plots.Count;
        public int Price => Area * Perimeter;

        public override string ToString()
        {
            return $"Region: {Plots[0].Id}: Area{Area}, P:{Perimeter}: \\$${Price}:\n\t{string.Join("\n\t", Plots.Select(p => $"{p.Location} => {p.Perimeter}"))}";
        }
    }
}