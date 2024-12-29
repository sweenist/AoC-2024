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
        var result = _maze.Traverse().Min(t => t.PathScore);
        Console.WriteLine($"Best reindeer track score is {result}");
    }

    public void Part2()
    {
        var paths = _maze.Traverse();
        var (path, bestScore) = paths.OrderBy(t => t.PathScore).First();

        var sb = new StringBuilder();
        for (var y = 0; y < _maze.Bounds.Height; y++)
        {
            for (var x = 0; x < _maze.Bounds.Width; x++)
            {
                if (!_maze.Walkable[x, y]) sb.Append('#');
                else if (path.Visited.Contains(new Point(x, y))) sb.Append('O');
                else sb.Append('.');
            }
            sb.Append('\n');
        }
        Console.WriteLine(sb);
        Console.WriteLine($"The number of paths around the best seats are {path.Visited.Count}");
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
                    if (cell != '#') CumulativeScore.Add(new Point(x, y), int.MaxValue);

                    if (cell == startChar)
                    {
                        Start = new Point(x, y);
                        CumulativeScore[Start] = 0;
                    }
                    else if (cell == endChar)
                        End = new Point(x, y);
                }
        }

        public Point Start { get; set; }
        public Point End { get; set; }
        public Dictionary<Point, int> CumulativeScore { get; set; } = [];

        public List<(Reindeer, int PathScore)> Traverse()
        {
            var returnList = new List<(Reindeer, int PathScore)>();
            var openList = new SortedSet<(int PathScore, Reindeer Reindeer)>(new PriorityComparer<Reindeer>())
                {(0, new Reindeer(Start, Vector.East))};

            while (openList.Count > 0)
            {
                var point = openList.Min;
                openList.Remove(point);
                var (_, current) = point;
                current.Visited.Add(current.Location);

                var nextDirections = Vector.CardinalPoints.Where(x => x != current.Direction.Invert())
                    .Select(v => (Direction: v, NextLocation: current.Location + v))
                    .Where(vt => Walkable[vt.NextLocation.X, vt.NextLocation.Y]);
                var hasBranches = nextDirections.Count() > 1;

                foreach (var (direction, nextPosition) in nextDirections)
                {
                    var weight = current.Direction != direction ? 1001 : 1;
                    var visited = hasBranches ? [.. current.Visited] : current.Visited;

                    if (nextPosition == End)
                    {
                        returnList.Add((new Reindeer(nextPosition, direction, [.. current.Visited]), point.PathScore + weight));
                    }

                    if (!current.Visited.Contains(nextPosition)
                        && Walkable[nextPosition.X, nextPosition.Y])
                    {
                        var nextReindeer = new Reindeer(nextPosition, direction, visited);
                        var nodeScore = point.PathScore + weight;

                        CumulativeScore.TryGetValue(nextPosition, out var score);
                        CumulativeScore[nextPosition] = Math.Min(score, nodeScore);
                        openList.Add((nodeScore, nextReindeer));
                    }
                }
            }
            return returnList;
        }
    }
}