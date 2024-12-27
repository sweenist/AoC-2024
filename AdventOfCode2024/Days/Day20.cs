using System.Text;
using AdventOfCode2024.Types;
using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Days;

public class Day20 : IDay
{
    private readonly string _example = @"###############
#...#...#.....#
#.#.#.#.#.###.#
#S#...#.#.#...#
#######.#.#.###
#######.#.#...#
#######.#.###.#
###..E#...#...#
###.#######.###
#...###...#...#
#.#####.#.###.#
#.#...#.#.#...#
#.#.#.#.#.#.###
#...#...#...###
###############";

    private readonly string[] _input = [];

    public Day20(bool useExample = false)
    {
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            _input = _example.Split(Environment.NewLine);
        }
        else
        {
            var inputFile = $"inputData/{GetType().Name}.txt";
            using var sr = new StreamReader(inputFile);
            _input = sr.ReadToEnd().Split(Environment.NewLine);
        }
    }

    public void Part1()
    {
        var maze = new Map(_input);
        var paths = maze.Traverse();

        var cheats = maze.Cheat(paths).GroupBy(x => x).ToDictionary(g => g.Key, x => x.Count());
        var hundredPlus = cheats.Where(k => k.Key >= 100).Sum(k => k.Value);
        Console.WriteLine($"Time to complete maze with integrity: {hundredPlus}");
    }

    public void Part2()
    {
        var maze = new Map(_input);
        var paths = maze.Traverse();
        var cheatGroups = GetEligibleCheatPoints(paths);

        var cheats = maze.CheatPlus(paths, cheatGroups).GroupBy(x => x).ToDictionary(g => g.Key, x => x.Count());
        Console.WriteLine($"{string.Join("\n\t", cheats.Select(c => $"{c.Key}: {c.Value}"))}");
        // var hundredPlus = cheats.Where(k => k.Key >= 100).Sum(k => k.Value);
        // Console.WriteLine($"Time to complete maze with integrity: {hundredPlus}");

    }

    private static Dictionary<Point, List<(Point Point, int DIstance)>> GetEligibleCheatPoints(Dictionary<Point, int> paths)
    {
        var returnCache = new Dictionary<Point, List<(Point Point, int DIstance)>>();
        foreach (var key in paths.Keys)
        {
            var maxDistance = paths[key];
            var potentialCheats = paths.Where(kvp => kvp.Value < maxDistance && kvp.Key.ManhattanDistance(key) < 20)
                .Select(kvp => (Point: kvp.Key, Distance: kvp.Value)).ToList();
            returnCache.Add(key, potentialCheats);
        }

        return returnCache;
    }

    private record Map
    {
        public Map(string[] input)
        {
            Bounds = new Boundary(input.Length, input[0].Length);
            Walkable = new bool[Bounds.Width, Bounds.Height];

            for (var x = 0; x < Bounds.Width; x++)
                for (var y = 0; y < Bounds.Width; y++)
                {
                    var mapChar = input[y][x];
                    Walkable[x, y] = mapChar != '#';
                    if (mapChar == 'S') Start = new Point(x, y);
                    else if (mapChar == 'E') End = new Point(x, y);
                }
        }

        public Boundary Bounds { get; set; }
        public bool[,] Walkable { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }


        public Dictionary<Point, int> Traverse()
        {
            var distances = new Dictionary<Point, int>();
            var visited = new HashSet<Point>();

            var openList = new Queue<(Point Point, int Distance)>();
            openList.Enqueue((End, 0));

            while (openList.TryDequeue(out var pointDistance))
            {
                distances.Add(pointDistance.Point, pointDistance.Distance);
                visited.Add(pointDistance.Point);
                foreach (var direction in Vector.CardinalPoints)
                {
                    var nextPosition = pointDistance.Point + direction;
                    if (Bounds.OutOfBounds(nextPosition) || !Walkable[nextPosition.X, nextPosition.Y])
                        continue;

                    if (visited.Add(nextPosition) && Walkable[nextPosition.X, nextPosition.Y])
                        openList.Enqueue((nextPosition, pointDistance.Distance + 1));
                }
            }
            return distances;
        }

        public List<int> Cheat(Dictionary<Point, int> traversed)
        {
            var savedSeconds = new List<int>();

            for (var y = 1; y < Bounds.BoundY; y++)
                for (var x = 1; x < Bounds.BoundX; x++)
                {
                    if (Walkable[x, y]) continue;
                    var wall = new Point(x, y);
                    foreach (var direction in new[] { Vector.North, Vector.East })
                    {
                        var adjacent = wall + direction.Invert();
                        var otherAdjacent = wall + direction;
                        if (traversed.TryGetValue(adjacent, out var side1) && traversed.TryGetValue(otherAdjacent, out var side2))
                        {
                            savedSeconds.Add(Math.Abs(side1 - side2) - 2);
                        }
                    }
                }

            return savedSeconds;
        }

        public List<int> CheatPlus(Dictionary<Point, int> traversed, Dictionary<Point, List<(Point Point, int Distance)>> cheatGroups)
        {
            var savedSeconds = new List<int>();

            foreach (var key in cheatGroups.Keys)
            {
                var pathListPoints = cheatGroups[key].Select(p => p.Point).ToList();
                var q = new Queue<(int Steps, Point Node)>();
                var visited = new HashSet<Point>([key]);
                foreach (var p in Vector.CardinalPoints.Select(v => (Steps: 1, Node: key + v)).Where(x => !Walkable[x.Node.X, x.Node.Y]))
                    q.Enqueue(p);
                while (q.Count > 0)
                {
                    var _ = q.TryDequeue(out var n);
                    visited.Add(n.Node);
                    var nextPoints = Vector.CardinalPoints.Select(v => n.Node + v)
                        .Where(v => !visited.Contains(v)).ToList();

                    savedSeconds.AddRange(nextPoints.Intersect(pathListPoints)
                        .Select(p =>
                        {
                            pathListPoints.Remove(p);
                            return traversed[key] - traversed[p] - (n.Steps + 1);
                        })
                        .Where(x => x >= 50));

                    foreach (var nextNode in nextPoints.Where(x => !Bounds.OutOfBounds(x) && !Walkable[x.X, x.Y]))
                        q.Enqueue((n.Steps + 1, nextNode));
                }
                Console.WriteLine($"{key}:\n\t{string.Join("\n\t", cheatGroups[key])}");
            }
            return savedSeconds;
        }
    }
}