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

        var cheats = SimpleCheat(paths, 2).GroupBy(x => x).ToDictionary(g => g.Key, x => x.Count());
        var hundredPlus = cheats.Where(k => k.Key >= 100).Sum(k => k.Value);
        Console.WriteLine($"Time to complete maze with integrity: {hundredPlus}");
    }

    public void Part2()
    {
        var maze = new Map(_input);
        var paths = maze.Traverse();

        var cheats = SimpleCheat(paths, 20).GroupBy(x => x).ToDictionary(g => g.Key, x => x.Count());
        var hundredPlus = cheats.Where(k => k.Key >= 100).Sum(k => k.Value);
        Console.WriteLine($"Time to complete maze with 20 ps cheats: {hundredPlus}");

    }

    private static Dictionary<Point, List<Point>> GetEligibleCheatPoints(Dictionary<Point, int> paths, int cheat)
    {
        var returnCache = new Dictionary<Point, List<Point>>();
        foreach (var key in paths.Keys)
        {
            var maxDistance = paths[key];
            var potentialCheats = paths.Where(kvp => kvp.Value < maxDistance && kvp.Key.ManhattanDistance(key) <= cheat)
                .Select(kvp => kvp.Key).ToList();
            returnCache.Add(key, potentialCheats);
        }

        return returnCache;
    }

    public static List<int> SimpleCheat(Dictionary<Point, int> traversed, int maxCheat)
    {
        var results = new List<int>();
        var eligiblePaths = GetEligibleCheatPoints(traversed, maxCheat);
        foreach (var path in traversed.Keys)
            results.AddRange(eligiblePaths[path].Select(x => traversed[path] - traversed[x] - path.ManhattanDistance(x)));

        return results;
    }

    private record Map
    {
        public Map(string[] input)
        {
            Bounds = new Boundary(input.Length, input[0].Length);
            Walkable = new bool[Bounds.Width, Bounds.Height];

            for (var x = 0; x < Bounds.Width; x++)
                for (var y = 0; y < Bounds.Height; y++)
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
    }
}