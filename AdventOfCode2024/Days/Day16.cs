using System.Text;
using AdventOfCode2024.Types;
using AdventOfCode2024.Utility;
using AdventOfCode2024.Utility.Math;
using static AdventOfCode2024.Utility.Math.VectorExtensions;

namespace AdventOfCode2024.Days;

public class Day16 : IDay
{
    private readonly string _example = @"###############
#.......#....E#
#.#.###.#.###.#
#.....#.#...#.#
#.###.#####.#.#
#.#.#.......#.#
#.#.#####.###.#
#...........#.#
###.#.#####.#.#
#...#.....#.#.#
#.#.#.###.#.#.#
#.....#...#.#.#
#.###.#.#.#.#.#
#S..#.....#...#
###############";

    //     private string _example2 = @"#################
    // #...#...#...#..E#
    // #.#.#.#.#.#.#.#.#
    // #.#.#.#...#...#.#
    // #.#.#.#.###.#.#.#
    // #...#.#.#.....#.#
    // #.#.#.#.#.#####.#
    // #.#...#.#.#.....#
    // #.#.#####.#.###.#
    // #.#.#.......#...#
    // #.#.###.#####.###
    // #.#.#...#.....#.#
    // #.#.#.#####.###.#
    // #.#.#.........#.#
    // #.#.#.#########.#
    // #S#.............#
    // #################";

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
        var result = _maze.Traverse().Select(t => t.Item1.Count + t.Turns * 1000).Min();
        Console.WriteLine($"Best reindeer track score is {result}");
    }

    public void Part2()
    {
        var paths = _maze.Traverse();
        var result = _maze.Traverse().Select(t => t.Item1.Count).Sum();

        Console.WriteLine($"The number of paths around the best seats are {result}");
    }

    private class Reindeer(Point Location, Vector Direction) : Actor(Location, Direction)
    {
        public Reindeer(Point Location, Vector Direction, HashSet<Point> visited) : this(Location, Direction)
        {
            Visited = visited;
        }
        public HashSet<Point> Visited { get; set; } = new();
    }

    private record TurnCell : Cell<Reindeer>
    {
        public TurnCell(Reindeer Parent, int? startValues = null) : base(Parent)
        {
            if (startValues.HasValue)
            {
                Accumulated = startValues.Value;
                Heuristic = startValues.Value;
                TotalScore = startValues.Value;
            }
        }

        public int Turns { get; set; } = 0;
    }

    private class Maze : TextMap
    {
        public Maze(string[] input) : base(input)
        {
            const char startChar = 'S';
            const char endChar = 'E';
            Cells = new TurnCell[Bounds.Width, Bounds.Height];

            for (int y = 0; y < Bounds.Height; y++)
                for (int x = 0; x < Bounds.Width; x++)
                {
                    var cell = input[y][x];
                    Walkable[x, y] = cell != '#';
                    Cells[x, y] = new TurnCell(new Reindeer(new Point(-1, -1), Vector.Zero));

                    if (cell == startChar)
                    {
                        Start = new Point(x, y);
                        Cells[x, y] = new TurnCell(new Reindeer(Start, Vector.East), 0);
                    }
                    else if (cell == endChar)
                        End = new Point(x, y);
                }
        }

        public TurnCell[,] Cells { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }

        public List<(List<Reindeer>, int Turns)> Traverse()
        {
            var startingPoints = new[] { (0, new Reindeer(Start, Vector.East)) }.ToList();
            var openList = new SortedSet<(int FScore, Reindeer Reindeer)>(startingPoints, new PriorityComparer<Reindeer>());
            var returnList = new List<(List<Reindeer>, int Turns)>();
            while (openList.Count > 0)
            {
                var point = openList.Min;
                openList.Remove(point);
                var (_, parent) = point;
                parent.Visited.Add(parent.Location);

                foreach (var direction in Vector.CardinalPoints.Where(x => x != parent.Direction.Invert()))
                {
                    var isTurning = parent.Direction != direction;
                    var nextPosition = parent.Location + direction;
                    if (!Walkable[nextPosition.X, nextPosition.Y])
                        continue;

                    if (nextPosition == End)
                    {
                        var parentCell = Cells[parent.Location.X, parent.Location.Y];
                        var cell = Cells[nextPosition.X, nextPosition.Y];
                        cell.Parent = parent;
                        cell.Turns = parentCell.Turns;

                        returnList.Add(FollowPath());
                    }

                    if (!parent.Visited.Contains(nextPosition)
                        && Walkable[nextPosition.X, nextPosition.Y])
                    {
                        var parentCell = Cells[parent.Location.X, parent.Location.Y];
                        var turns = parentCell.Turns + (isTurning ? 1 : 0);
                        var g = parentCell.Accumulated + 1;
                        var h = nextPosition.ManhattanDistance(End);
                        var f = g + h + turns * 1000;

                        var nextCell = Cells[nextPosition.X, nextPosition.Y];

                        if (nextCell.TotalScore == int.MaxValue || nextCell.TotalScore > f)
                        {
                            var nextReindeer = new Reindeer(nextPosition, direction, [.. parent.Visited]);
                            openList.Add((f, nextReindeer));
                            nextCell.TotalScore = f;
                            nextCell.Accumulated = g;
                            nextCell.Heuristic = h;
                            nextCell.Parent = parent;
                            nextCell.Turns = turns;
                        }
                    }
                }
            }
            return returnList;
        }

        private (List<Reindeer>, int Turns) FollowPath()
        {
            var path = new List<Reindeer>();
            var cells = new HashSet<TurnCell>();
            var y = End.Y;
            var x = End.X;


            while (!(Cells[x, y].Parent.Location.X == x && Cells[x, y].Parent.Location.Y == y))
            {
                var point = new Point(x, y);
                var nextParent = Cells[x, y].Parent;
                var orientation = Vector.Delta(point, nextParent.Location);
                var pathSegment = new Reindeer(point, orientation);

                path.Add(pathSegment);
                cells.Add(Cells[x, y]);
                x = nextParent.Location.X;
                y = nextParent.Location.Y;
            }

            Print([.. path]);
            return (path, Cells[End.X, End.Y].Turns);
        }

        private IEnumerable<TurnCell> FindCandidates(TurnCell cell, HashSet<TurnCell> pathCells, List<TurnCell> visited)
        {
            var connected = false;
            while (!connected)
            {
                visited.Add(cell);
                var cellParent = cell.Parent.Location;
                var nextCell = Cells[cellParent.X, cellParent.Y];
                if (nextCell is null)
                    break;
                Console.WriteLine($"Valid cell {cell}");
                cell = nextCell;
                if (visited.Contains(cell)) yield break;
                connected = pathCells.Contains(cell);
            }
            if (!connected)
            {
                Console.WriteLine($"Cell with parent {cell.Parent} short circuited");
                yield break;
            }
            foreach (var newStep in visited)
            {
                if (pathCells.Contains(newStep)) continue;
                yield return newStep;
            }

        }

        private void Print(bool[,] closedList, Reindeer parent, Point next)
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
                    else if (compPoint.Equals(parent.Location)) sb.Append('O');
                    else if (closedList[x, y]) sb.Append('X');
                    else if (compPoint.Equals(next)) sb.Append('?');
                    else sb.Append(' ');
                }
                sb.Append('\n');
            }
            Console.Write(sb);
            var cell = Cells[next.X, next.Y];
            Console.WriteLine($"Next Cell: {cell.Parent.Location}:{cell.Parent.Direction} f:{cell.TotalScore} g:{cell.Accumulated} h:{cell.Heuristic} turns: {cell.Turns}\n");
            // Console.ReadKey();
        }

        private void Print(List<Reindeer> path)
        {
            var sb = new StringBuilder();

            for (var y = 0; y < Bounds.Height; y++)
            {
                for (var x = 0; x < Bounds.Width; x++)
                {
                    var compPoint = new Point(x, y);
                    var step = path.Find(x => x.Location == compPoint);
                    if (!Walkable[x, y]) sb.Append('#');
                    else if (compPoint.Equals(Start)) sb.Append('S');
                    else if (compPoint.Equals(End)) sb.Append('E');
                    else if (compPoint.Equals(step?.Location)) sb.Append(MapTokens[step.Direction]);
                    else sb.Append(' ');
                }
                sb.Append('\n');
            }
            Console.Write(sb);
            // Console.ReadKey();
        }

        private void PrintCells()
        {
            var sb = new StringBuilder();

            for (var y = 0; y < Bounds.Height; y++)
            {
                for (var x = 0; x < Bounds.Width; x++)
                {
                    if (Cells[x, y].TotalScore != int.MaxValue)
                    {
                        sb.Append(Cells[x, y]);
                        sb.Append('\n');
                    }
                }
                sb.Append('\n');
            }
            Console.Write(sb);
            // Console.ReadKey();
        }
    }
}