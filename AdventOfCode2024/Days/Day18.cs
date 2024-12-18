using System.Text;
using System.Threading.Channels;
using AdventOfCode2024.Types;
using AdventOfCode2024.Utility;
using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Days;

public class Day18 : IDay
{
    private string _example = @"5,4
4,2
4,5
3,0
2,1
6,3
2,4
1,5
0,6
3,3
2,6
5,1
1,2
5,5
2,5
6,5
1,4
0,4
6,4
1,1
6,1
1,0
0,5
1,6
2,0";

    private readonly List<string> _input = [];
    private readonly Point _exit;
    private readonly int _fallingBytes;

    public Day18(bool useExample = false)
    {
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            _input.AddRange(_example.Split(Environment.NewLine));
            _exit = new Point(6, 6);
            _fallingBytes = 12;
        }
        else
        {
            var inputFile = $"inputData/{GetType().Name}.txt";
            using var sr = new StreamReader(inputFile);
            _input.AddRange(sr.ReadToEnd().Split(Environment.NewLine));
            _exit = new Point(70, 70);
            _fallingBytes = 1024;
        }
    }

    public void Part1()
    {
        var corruptedBytes = _input.Take(_fallingBytes)
                                    .Select(x => x.Split(',').Select(int.Parse).ToList())
                                    .Select(p => new Point(p[0], p[1]))
                                    .ToList();

        var Map = new Map(corruptedBytes, _exit);
        var shortestPath = Map.Traverse();

        Console.WriteLine($"The shortest path through falling bytes is {shortestPath.Count}");
    }

    public void Part2()
    {
        var corruptedBytes = _input.Select(x => x.Split(',').Select(int.Parse).ToList())
                                   .Select(p => new Point(p[0], p[1]))
                                   .ToList();

        var fallingCount = _fallingBytes + 1;
        var initialFallingBytes = corruptedBytes.Take(fallingCount).ToList();
        var map = new Map(initialFallingBytes, _exit);
        var shortestPath = map.Traverse();

        var firstBlockagePoint = new Point(-1, -1);
        var highBytes = corruptedBytes.Count - fallingCount;
        var lowBytes = 0;

        while (true)
        {
            var midRemaining = lowBytes + (highBytes - lowBytes) / 2;
            if (highBytes < lowBytes)
            {
                firstBlockagePoint = corruptedBytes[fallingCount + midRemaining - 1];
                break;
            }
            var newBytes = corruptedBytes.Take(fallingCount + midRemaining).ToList();

            map.ReInitialize(newBytes);
            shortestPath = map.Traverse();

            if (shortestPath.Count > 0) lowBytes = midRemaining + 1;
            else highBytes = midRemaining - 1;
            round++;
        }
        Console.WriteLine($"The first blocking byte is {firstBlockagePoint}");
    }

    private record Map
    {
        public Map(List<Point> corruptBytes, Point end)
        {
            Bounds = new Boundary(end);
            Paths = new Cell[Bounds.Width, Bounds.Height];
            Corrupted = new bool[Bounds.Width, Bounds.Height];
            End = end;
            foreach (var fallingByte in corruptBytes)
                Corrupted[fallingByte.X, fallingByte.Y] = true;
            for (var x = 0; x < Bounds.Width; x++)
                for (var y = 0; y < Bounds.Width; y++)
                    Paths[x, y] = new Cell(new Point(x, y));

            Paths[0, 0] = new Cell(new Point(0, 0)) { TotalScore = 0, Accumulated = 0, Heuristic = 0 };
        }

        public Boundary Bounds { get; set; }
        public bool[,] Corrupted { get; set; }
        public Cell[,] Paths { get; set; }
        public Point Start { get; set; } = new Point(0, 0);
        public Point End { get; set; }

        public void ReInitialize(List<Point> corruptedBytes)
        {
            Corrupted = new bool[Bounds.Width, Bounds.Height];
            foreach (var fallingByte in corruptedBytes)
                Corrupted[fallingByte.X, fallingByte.Y] = true;
            for (var x = 0; x < Bounds.Width; x++)
                for (var y = 0; y < Bounds.Width; y++)
                    Paths[x, y] = new Cell(new Point(x, y));

            Paths[0, 0] = new Cell(new Point(0, 0)) { TotalScore = 0, Accumulated = 0, Heuristic = 0 };
        }

        public List<Point> Traverse()
        {
            var closedList = new bool[Bounds.Width, Bounds.Height];
            var startingPoints = new[] { (0, Start) };

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
                    if (Bounds.OutOfBounds(nextPosition) || Corrupted[nextPosition.X, nextPosition.Y])
                        continue;

                    if (nextPosition == End)
                    {
                        var cell = Paths[nextPosition.X, nextPosition.Y];
                        cell.Parent = parent;
                        return FollowPath();
                    }

                    if (!closedList[nextPosition.X, nextPosition.Y]
                        && !Corrupted[nextPosition.X, nextPosition.Y])
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
        private void Print(bool[,] closedList, Point parent, Point next)
        {
            var sb = new StringBuilder();
            for (var y = 0; y < Bounds.Height; y++)
            {
                for (var x = 0; x < Bounds.Width; x++)
                {
                    var compPoint = new Point(x, y);

                    if (Corrupted[x, y]) sb.Append('#');
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