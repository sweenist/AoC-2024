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
        var trueDistance = paths[maze.Start];

        var cheats = maze.Cheat(paths).GroupBy(x => x).ToDictionary(g => g.Key, x => x.Count());
        var hundredPlus = cheats.Where(k => k.Key >= 100).Sum(k => k.Value);
        // foreach (var kvp in cheats)
        //     Console.WriteLine($"\t{kvp.Value} save {kvp.Key} seconds");
        Console.WriteLine($"Time to complete maze with integrity: {hundredPlus}");

    }

    public void Part2()
    {
        throw new NotImplementedException();
    }

    private record Map
    {
        public Map(string[] input)
        {
            Bounds = new Boundary(input.Length, input[0].Length);
            Paths = new Cell<Point>[Bounds.Width, Bounds.Height];
            Walkable = new bool[Bounds.Width, Bounds.Height];

            for (var x = 0; x < Bounds.Width; x++)
                for (var y = 0; y < Bounds.Width; y++)
                {
                    Paths[x, y] = new Cell<Point>(new Point(x, y));
                    var mapChar = input[y][x];
                    Walkable[x, y] = mapChar != '#';
                    if (mapChar == 'S')
                    {
                        Start = new Point(x, y);
                        Paths[x, y] = new Cell<Point>(Start) { TotalScore = 0, Accumulated = 0, Heuristic = 0 };
                    }
                    else if (mapChar == 'E') End = new Point(x, y);
                }
        }

        public Boundary Bounds { get; set; }
        public bool[,] Walkable { get; set; }
        public Cell<Point>[,] Paths { get; set; }
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

        private void Print(bool[,] closedList, Point parent, Point next)
        {
            var sb = new StringBuilder();
            for (var y = 0; y < Bounds.Height; y++)
            {
                for (var x = 0; x < Bounds.Width; x++)
                {
                    var compPoint = new Point(x, y);

                    if (!Walkable[x, y]) sb.Append('#');
                    else if (compPoint.Equals(Start)) sb.Append('S');
                    else if (compPoint.Equals(End)) sb.Append('E');
                    else if (compPoint.Equals(parent)) sb.Append('O');
                    else if (closedList[x, y]) sb.Append('X');
                    else if (compPoint.Equals(next)) sb.Append('?');
                    else sb.Append(' ');
                }
                sb.Append('\n');
            }
            Console.Write(sb);
            var cell = Paths[next.X, next.Y];
            Console.WriteLine($"Next Cell: {cell.Parent}: f:{cell.TotalScore} g:{cell.Accumulated} h:{cell.Heuristic}\n");
        }
    }

}