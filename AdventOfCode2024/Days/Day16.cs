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
        var (paths, turns) = _maze.Traverse();
        var result = paths.Count + turns * 1000;
        Console.WriteLine($"Best reindeer track score is {result}");
    }

    public void Part2()
    {
        var (round1, _) = _maze.Traverse(findGoodSeats: true);

        Console.WriteLine($"The number of paths around the best seats are {round1.Count}");
    }

    private record TurnCell : Cell<Actor>
    {
        public TurnCell(Actor Parent, int? startValues = null) : base(Parent)
        {
            if (startValues.HasValue)
            {
                Accumulated = startValues.Value;
                Heuristic = startValues.Value;
                TotalScore = startValues.Value;
            }
        }

        public int Turns { get; set; } = 0;
        public List<TurnCell> Neighbors { get; } = [];
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
                    Cells[x, y] = new TurnCell(new Actor(new Point(-1, -1), Vector.Zero));

                    if (cell == startChar)
                    {
                        Start = new Point(x, y);
                        Cells[x, y] = new TurnCell(new Actor(Start, Vector.East), 0);
                    }
                    else if (cell == endChar)
                        End = new Point(x, y);
                }
        }

        public TurnCell[,] Cells { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }

        public (List<Actor>, int Turns) Traverse(bool findGoodSeats = false)
        {
            var closedList = new bool[Bounds.Width, Bounds.Height];
            var startingPoints = new[] { (0, new Actor(Start, Vector.East)) }.ToList();

            var openList = new SortedSet<(int FScore, Actor Reindeer)>(startingPoints, new PriorityComparer());

            while (openList.Count > 0)
            {
                var point = openList.Min;
                openList.Remove(point);
                var (_, parent) = point;
                closedList[parent.Location.X, parent.Location.Y] = true;

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
                        cell.Neighbors.Add(parentCell);

                        parentCell.Neighbors.Add(cell);
                        return FollowPath(findGoodSeats);
                    }

                    if (!closedList[nextPosition.X, nextPosition.Y]
                        && Walkable[nextPosition.X, nextPosition.Y])
                    {
                        var parentCell = Cells[parent.Location.X, parent.Location.Y];
                        var turns = parentCell.Turns + (isTurning ? 1 : 0);
                        var g = parentCell.Accumulated + 1;
                        var h = nextPosition.ManhattanDistance(End);
                        var f = g + h + turns * 1000;

                        var nextCell = Cells[nextPosition.X, nextPosition.Y];
                        parentCell.Neighbors.Add(nextCell);

                        if (nextCell.TotalScore == int.MaxValue || nextCell.TotalScore > f)
                        {
                            var updatedReindeer = new Actor(nextPosition, direction);
                            openList.Add((f, updatedReindeer));
                            nextCell.TotalScore = f;
                            nextCell.Accumulated = g;
                            nextCell.Heuristic = h;
                            nextCell.Parent = parent;
                            nextCell.Turns = turns;
                        }
                        // Print(closedList, parent, nextPosition);
                    }
                }
            }
            throw new Exception("Could not find a reasonable path");
        }

        private (List<Actor>, int Turns) FollowPath(bool findOtherPaths)
        {
            var path = new List<Actor>();
            var cells = new HashSet<TurnCell>();
            var y = End.Y;
            var x = End.X;


            while (!(Cells[x, y].Parent.Location.X == x && Cells[x, y].Parent.Location.Y == y))
            {
                var point = new Point(x, y);
                var nextParent = Cells[x, y].Parent;
                var orientation = Vector.Delta(point, nextParent.Location);
                var pathSegment = new Actor(point, orientation);

                path.Add(pathSegment);
                cells.Add(Cells[x, y]);
                x = nextParent.Location.X;
                y = nextParent.Location.Y;
            }

            if (findOtherPaths)
            {
                // PrintCells();
                var newPath = BuildAlternatePaths(cells);
                return (newPath.Select(x => x.Parent).ToList(), 0);
            }
            Print([.. path]);
            return (path, Cells[End.X, End.Y].Turns);
        }

        private HashSet<TurnCell> BuildAlternatePaths(HashSet<TurnCell> happyPath)
        {
            var newSet = new HashSet<TurnCell>(happyPath);
            foreach (var path in happyPath)
            {
                if (path.Neighbors.Count > 1)
                {
                    var possibilities = path.Neighbors.Where(x => !happyPath.Contains(x)).ToList();
                    // var newCells = possibilities.SelectMany(x => FindCandidates(x, happyPath, visited).ToList()).ToList();
                    foreach (var candidate in possibilities)
                    {
                        var visited = new List<TurnCell>([path]);
                        var chain = FindCandidates(candidate, happyPath, visited).ToList();
                        newSet.UnionWith(chain);
                    }
                }
            }
            return newSet;
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

        private void Print(bool[,] closedList, Actor parent, Point next)
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

        private void Print(List<Actor> path)
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