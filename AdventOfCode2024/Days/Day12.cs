using AdventOfCode2024.Types;
using AdventOfCode2024.Utility;
using AdventOfCode2024.Utility.Math;

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

        Console.WriteLine($"Total fence costs are {totalCost}");
    }

    public void Part2()
    {
        var plotPoints = _input.SelectMany((y, i) => y.Select((x, j) => new Plot(x, new Point(j, i))));
        var result = Map(plotPoints);

        var totalCost = result.Sum(x => x.Value.BulkPrice);

        Console.WriteLine($"Total fence costs with bulk savings are {totalCost}");
    }

    private Dictionary<int, Region> Map(IEnumerable<Plot> sections)
    {
        var regions = new Dictionary<int, Region>();
        var visited = new bool[_bounds.Width, _bounds.Height];

        void Traverse(Plot plot, int searchId, bool isNew = false)
        {
            visited[plot.Location.X, plot.Location.Y] = true;
            if (isNew)
            {
                var region = new Region();
                region.Plots.Add(plot);
                regions[searchId] = region;
            }

            var edges = new EdgeDetector();
            List<Point> neighborLocationsOfSameId = [];

            foreach (var cardinal in Vector.CardinalPoints)
            {
                var adjacent = plot.Location + cardinal;
                if (_bounds.OutOfBounds(adjacent))
                {
                    edges[cardinal] = true;
                    continue;
                }

                var adjacentPlot = sections.Single(x => x.Location == adjacent);
                if (adjacentPlot.Id == plot.Id)
                {
                    neighborLocationsOfSameId.Add(adjacent);
                    if (!visited[adjacentPlot.Location.X, adjacentPlot.Location.Y])
                    {
                        regions[searchId].Plots.Add(adjacentPlot);
                        Traverse(adjacentPlot, searchId);
                    }
                }
                else
                {
                    edges[cardinal] = true;
                }
            }
            plot.Edges = EdgeMap.ParseEdges(edges);
            plot.OutsideCorners = CheckForOutsideCorners(plot, edges, neighborLocationsOfSameId, sections.ToList());
        }

        for (var y = 0; y < _bounds.Height; y++)
            for (var x = 0; x < _bounds.Width; x++)
            {
                var point = new Point(x, y);
                if (visited[point.X, point.Y]) continue;

                var searchId = y * _bounds.Width + x;
                var plot = sections.Single(x => x.Location == point);
                Traverse(plot, searchId, isNew: true);
            }
        return regions;
    }

    private int CheckForOutsideCorners(Plot plot, EdgeDetector edges, List<Point> neighborLocationsOfSameId, List<Plot> allPlots)
    {
        if (neighborLocationsOfSameId.Count < 2 || edges.ParallelWalls)
            return 0;

        var differentCorners = Vector.OrdinalPoints.Select(v => plot.Location + v)
                                          .Where(p => !_bounds.OutOfBounds(p))
                                          .Select(p => allPlots.Single(x => x.Location == p))
                                          .Where(p => p.Id != plot.Id)
                                          .ToList();

        if (neighborLocationsOfSameId.Count == 4)
            return differentCorners.Count;
        else if (neighborLocationsOfSameId.Count == 3)
        {
            return edges switch
            {
                var x when x.North => differentCorners.Count(c => c.Location.Y > plot.Location.Y),
                var x when x.East => differentCorners.Count(c => c.Location.X < plot.Location.X),
                var x when x.South => differentCorners.Count(c => c.Location.Y < plot.Location.Y),
                var x when x.West => differentCorners.Count(c => c.Location.X > plot.Location.X),
                _ => throw new Exception("Trouble finding outside corners for 3 same neighbours")
            };
        }
        else if (edges.IsCorner)
        {
            return edges switch
            {
                var x when x.North && x.East => differentCorners.Count(c => plot.Location + Vector.SouthWest == c.Location),
                var x when x.North && x.West => differentCorners.Count(c => plot.Location + Vector.SouthEast == c.Location),
                var x when x.South && x.East => differentCorners.Count(c => plot.Location + Vector.NorthWest == c.Location),
                var x when x.South && x.West => differentCorners.Count(c => plot.Location + Vector.NorthEast == c.Location),
                _ => throw new Exception($"Corner edge had unexpected result when determining outside corner")
            };
        }
        else
        {
            Console.WriteLine($"Shouldn't be here {plot}, neighbors: {string.Join(',', neighborLocationsOfSameId)}; corners: {string.Join(',', differentCorners)}");
            return 0;
        }
    }

    private record Plot(char Id, Point Location)
    {
        public char Id { get; } = Id;
        public Point Location { get; } = Location;
        public Edge Edges { get; set; }
        public int OutsideCorners { get; set; }

        public override string ToString()
        {
            return $"Plot: Location: {Location}; Edges:{Edges.Id}, {Edges.Side}; Outside corners: {OutsideCorners}";
        }
    }

    private record Region
    {
        public List<Plot> Plots { get; set; } = [];
        int Perimeter => Plots.Where(p => p.OutsideCorners == 0)
                              .Select(p => p.Edges).Where(e => e.Perimeter > 0)
                              .Select(e => e.Perimeter).Sum();
        public int Vertices => Plots.Where(p => p.Edges.Side > 0 || p.OutsideCorners > 0)
                                    .Select(p => p.Edges.Side + p.OutsideCorners).Sum();
        public int Area => Plots.Count;
        public int Price => Area * Perimeter;
        public int BulkPrice => Vertices * Area;

        public override string ToString()
        {
            return $"Region: {Plots[0].Id}: Area {Area}, Perimeter: {Perimeter}; Corners:{Vertices} Regular ${Price}; Bulk: ${BulkPrice}";
        }
    }
}