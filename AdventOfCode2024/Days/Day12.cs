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
            _input.AddRange(_example.Split('\n').Select(s => s.TrimEnd()));
        }
        else
        {
            var inputFile = $"inputData/{GetType().Name}.txt";
            using var sr = new StreamReader(inputFile);
            _input.AddRange(sr.ReadToEnd().Split('\n').Select(s => s.TrimEnd()));
        }
        _bounds = new Boundary(_input.Count, _input[0].Length);
    }

    public void Part1()
    {
        var plotPoints = _input.SelectMany((y, i) => y.Select((x, j) => new Plot(x, new Point(j, i))));
        var result = Map(plotPoints);
        var totalCost = result.Sum(x => x.Value.Price);
        var totalArea = result.Sum(x => x.Value.Area);

        // Console.WriteLine($"Regions are: {string.Join("\n\n", result.Values)}");
        Console.WriteLine($"Total area are {totalArea}. SHould equal {_bounds.Width * _bounds.Height}");

        Console.WriteLine($"Total fence costs are {totalCost}");
    }

    public void Part2()
    {
        var plotPoints = _input.SelectMany((y, i) => y.Select((x, j) => new Plot(x, new Point(j, i))));
        var result = Map(plotPoints);
        foreach (var region in result.Select(x => x.Value))
        {
            region.CalculateVertices();
            // Console.WriteLine($"Region {region} has {region.BulkPrice} ({region.Vertices} x Area)");
        }


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

        for (var y = 0; y < _bounds.Height; y++)
            for (var x = 0; x < _bounds.Width; x++)
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
        public int Vertices { get; private set; }
        public int Area => Plots.Count;
        public int Price => Area * Perimeter;
        public int BulkPrice => Vertices * Area;

        public void CalculateVertices()
        {
            if (Area == 1) Vertices += 4;
            else
            {
                var edgePlots = Plots.Where(p => p.Perimeter > 0)
                                       .OrderBy(x => x.Location.X)
                                       .ThenBy(x => x.Location.Y)
                                       .ToList();
                var visitedEdges = edgePlots.Select(x => new KeyValuePair<Point, bool>(x.Location, false)).ToDictionary();
                var startPlot = edgePlots[0];
                var startDirection = Vector.CardinalPoints.Intersect(startPlot.Neighbours
                        .Select(x => startPlot.Location - x.Location)
                        .Select(x => new Vector(x.X, x.Y)))
                    .First();

                var currentPlot = startPlot;
                var currentDirection = startDirection;
                Console.WriteLine($"Starting edge detection on Region {currentPlot.Id} {currentPlot.Location}, Direction {currentDirection}");

                do
                {
                    Console.WriteLine($"Heading Direction {currentDirection} from {currentPlot.Location}");

                    visitedEdges[currentPlot.Location] = true;
                    //ClockWise
                    var nextPlot = currentPlot.Neighbours.FirstOrDefault(x => x.Location == currentPlot.Location + currentDirection.Clockwise());
                    if (nextPlot is not null)
                    {
                        currentDirection = currentDirection.Clockwise();
                        Console.WriteLine($"\tTurning CW and moving {currentDirection} to {nextPlot.Location}");
                        Vertices++;
                        currentPlot = nextPlot;
                        continue;
                    }
                    //Straight
                    nextPlot = currentPlot.Neighbours.FirstOrDefault(x => x.Location == currentPlot.Location + currentDirection);
                    if (nextPlot is not null)
                    {
                        Console.WriteLine($"\tMoving straight to {nextPlot.Location}");
                        currentPlot = nextPlot;
                        continue;
                    }
                    //CounterClockwise
                    nextPlot = currentPlot.Neighbours.FirstOrDefault(x => x.Location == currentPlot.Location + currentDirection.AntiClockwise());
                    if (nextPlot is not null)
                    {
                        currentDirection = currentDirection.AntiClockwise();
                        Console.WriteLine($"\tTurning CCW and moving {currentDirection} to {nextPlot.Location}");
                        Vertices++;
                        currentPlot = nextPlot;
                        continue;
                    }

                    currentPlot = currentPlot.Neighbours.Single();
                    currentDirection = currentDirection.Invert();
                    Console.WriteLine($"\tTurning around and moving {currentDirection} to {currentPlot.Location}");
                    Vertices += 2;
                } while (currentDirection != startDirection && currentPlot != startPlot);
                //handle missing edges
            }
        }

        public override string ToString()
        {
            return $"Region: {Plots[0].Id}: Area {Area}, P:{Perimeter}: ${Price}";
        }
    }
}