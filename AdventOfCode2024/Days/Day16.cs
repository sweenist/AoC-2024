using System.Text;
using AdventOfCode2024.Types;
using AdventOfCode2024.Utility;
using AdventOfCode2024.Utility.Math;
using static AdventOfCode2024.Utility.Math.VectorExtensions;

namespace AdventOfCode2024.Days;

public class Day16 : IDay
{
    //     private readonly string _example = @"###############
    // #.......#....E#
    // #.#.###.#.###.#
    // #.....#.#...#.#
    // #.###.#####.#.#
    // #.#.#.......#.#
    // #.#.#####.###.#
    // #...........#.#
    // ###.#.#####.#.#
    // #...#.....#.#.#
    // #.#.#.###.#.#.#
    // #.....#...#.#.#
    // #.###.#.#.#.#.#
    // #S..#.....#...#
    // ###############";

    private string _example = @"#################
#...#...#...#..E#
#.#.#.#.#.#.#.#.#
#.#.#.#...#...#.#
#.#.#.#.###.#.#.#
#...#.#.#.....#.#
#.#.#.#.#.#####.#
#.#...#.#.#.....#
#.#.#####.#.###.#
#.#.#.......#...#
#.#.###.#####.###
#.#.#...#.....#.#
#.#.#.#####.###.#
#.#.#.........#.#
#.#.#.#########.#
#S#.............#
#################";

    private readonly string[] _input;
    private readonly Maze _maze;

    public Day16(bool useExample = false)
    {
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            _input = [.. _example.Split('\n').Select(x => x.TrimEnd())];
        }
        else
        {
            var inputFile = $"inputData/{GetType().Name}.txt";
            using var sr = new StreamReader(inputFile);
            _input = [.. sr.ReadToEnd().Split('\n').Select(x => x.TrimEnd())];
        }
        _maze = new Maze(_input);
    }

    public void Part1()
    {
        var (result, _) = _maze.Traverse();
        Console.WriteLine($"Best reindeer track score is {result}");
    }

    public void Part2()
    {
        var (_, paths) = _maze.Traverse();
        var result = paths.SelectMany(p => p).Distinct().ToList();

        var sb = new StringBuilder();
        for (var y = 0; y < _maze.Bounds.Height; y++)
        {
            for (var x = 0; x < _maze.Bounds.Width; x++)
            {
                if (!_maze.Walkable[x, y]) sb.Append('#');
                else if (result.Contains(new Point(x, y))) sb.Append('O');
                else sb.Append('.');
            }
            sb.Append('\n');
        }
        Console.WriteLine(sb);
        Console.WriteLine($"The number of paths around the best seats are {result.Count}");
    }

    private class Reindeer(Point Location, Vector Direction) : Actor(Location, Direction)
    {
        public Reindeer(Point Location, Vector Direction, List<Point> visited) : this(Location, Direction)
        {
            Visited = visited;
        }
        public List<Point> Visited { get; set; } = [];
    }

    private class Maze : TextMap
    {
        public Maze(string[] input) : base(input)
        {
            const char startChar = 'S';
            const char endChar = 'E';

            for (int y = 0; y < Bounds.Height; y++)
                for (int x = 0; x < Bounds.Width; x++)
                {
                    var cell = input[y][x];
                    Walkable[x, y] = cell != '#';
                    if (cell != '#')
                        foreach (var facing in Vector.CardinalPoints)
                            CumulativeScore.Add((new Point(x, y), facing), int.MaxValue);

                    if (cell == startChar)
                    {
                        Start = new Point(x, y);
                        CumulativeScore[(Start, Vector.East)] = 0;
                    }
                    else if (cell == endChar)
                        End = new Point(x, y);
                }
        }

        public Point Start { get; set; }
        public Point End { get; set; }
        public Dictionary<(Point, Vector), int> CumulativeScore { get; set; } = [];

        public (int BestScore, List<List<Point>> BestPaths) Traverse()
        {
            var optimalPaths = new List<List<Point>>();
            var openList = new Queue<(int PathScore, Reindeer Reindeer)>();
            openList.Enqueue((0, new Reindeer(Start, Vector.East, [Start])));
            var bestScore = int.MaxValue;

            while (openList.TryDequeue(out var composite))
            {
                var (currentScore, current) = composite;
                if (currentScore > bestScore) continue;
                current.Visited.Add(current.Location);

                var nextDirections = Vector.CardinalPoints.Where(x => x != current.Direction.Invert())
                    .Select(v => (Direction: v, NextLocation: current.Location + v))
                    .Where(vt => Walkable[vt.NextLocation.X, vt.NextLocation.Y]);

                foreach (var (direction, nextPosition) in nextDirections)
                {
                    var weight = current.Direction != direction ? 1001 : 1;

                    if (nextPosition == End)
                    {
                        composite.PathScore += weight;
                        current.Visited.Add(nextPosition);

                        if (composite.PathScore < bestScore)
                        {
                            bestScore = composite.PathScore;
                            optimalPaths = [[.. current.Visited]];
                        }
                        else if (composite.PathScore == bestScore)
                            optimalPaths.Add(current.Visited);

                        continue;
                    }

                    var nextReindeer = new Reindeer(nextPosition, direction, [.. current.Visited]);
                    var nextScore = composite.PathScore + weight;
                    CumulativeScore.TryGetValue((nextPosition, direction), out var score);

                    if (nextScore <= score)
                    {
                        CumulativeScore[(nextPosition, direction)] = nextScore;
                        openList.Enqueue((nextScore, nextReindeer));
                    }
                }
            }
            return (bestScore, optimalPaths);
        }
    }
}