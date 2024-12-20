using System.Text;
using AdventOfCode2024.Types;
using AdventOfCode2024.Utility;
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
        var integralPath = maze.Traverse();
        var cheats = maze.Cheat(integralPath).GroupBy(x => x).ToDictionary(g => g.Key, x => x.Count());
        var hundredPlus = cheats.Where(k => k.Key >= 100).Sum(k => k.Value);
        Console.WriteLine($"Time to complete maze with integrity: {hundredPlus}");
        foreach (var kvp in cheats)
            Console.WriteLine($"\t{kvp.Value} save {kvp.Key} seconds");


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


        public List<Point> Traverse(Point? start = null)
        {
            var closedList = new bool[Bounds.Width, Bounds.Height];
            var startingPoints = new[] { (0, start ?? Start) };

            var openList = new SortedSet<(int FScore, Point step)>(startingPoints, new AStarComparer());

            while (openList.Count > 0)
            {
                var point = openList.Min;
                openList.Remove(point);
                var (_, parent) = point;
                closedList[parent.X, parent.Y] = true;

                foreach (var direction in Vector.CardinalPoints)
                {
                    var nextPosition = parent + direction;
                    if (Bounds.OutOfBounds(nextPosition) || !Walkable[nextPosition.X, nextPosition.Y])
                        continue;

                    if (nextPosition == End)
                    {
                        var cell = Paths[nextPosition.X, nextPosition.Y];
                        cell.Parent = parent;
                        return FollowPath();
                    }

                    if (!closedList[nextPosition.X, nextPosition.Y]
                        && Walkable[nextPosition.X, nextPosition.Y])
                    {
                        var parentCell = Paths[parent.X, parent.Y];
                        var g = parentCell.Accumulated + 1;
                        var h = nextPosition.ManhattanDistance(End);
                        var f = g + h;

                        var nextCell = Paths[nextPosition.X, nextPosition.Y];

                        if (nextCell.TotalScore == int.MaxValue || nextCell.TotalScore > f)
                        {
                            openList.Add((f, nextPosition));
                            nextCell.TotalScore = f;
                            nextCell.Accumulated = g;
                            nextCell.Heuristic = h;
                            nextCell.Parent = parent;
                        }
                        // Print(closedList, parent, nextPosition);
                    }
                }
            }
            return [];
        }

        private List<Point> FollowPath()
        {
            var path = new List<Point>();
            var y = End.Y;
            var x = End.X;

            while (!(Paths[x, y].Parent.X == x && Paths[x, y].Parent.Y == y))
            {
                path.Add(new Point(x, y));
                var nextParent = Paths[x, y].Parent;

                x = nextParent.X;
                y = nextParent.Y;
            }

            return path;
        }

        public List<int> Cheat(List<Point> truePath)
        {
            truePath.Add(Start);
            var steps = new Stack<Point>(truePath);
            var triedWallPositions = new HashSet<Point>();
            var savedSeconds = new List<int>();
            Point? previousPoint = null;

            while (steps.Count > 0)
            {
                var stepToTry = steps.Pop();
                var previousDirection = Vector.Delta(stepToTry, previousPoint ?? stepToTry);
                foreach (var direction in Vector.CardinalPoints.Except([previousDirection]))
                {
                    var searchCell = stepToTry + direction;
                    var searchCell2 = stepToTry + direction * 2;
                    if (!Walkable[searchCell.X, searchCell.Y]
                        && truePath.Contains(searchCell2)
                        && !triedWallPositions.Contains(searchCell))
                    {
                        triedWallPositions.Add(searchCell);
                        var pathIndex = truePath.IndexOf(stepToTry);
                        var cheatIndex = truePath.IndexOf(searchCell2);
                        savedSeconds.Add(Math.Abs(pathIndex - cheatIndex) - 2);
                    }
                }
                previousPoint = stepToTry;
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