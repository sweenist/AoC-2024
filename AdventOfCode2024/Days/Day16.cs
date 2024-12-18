using System.Text;
using AdventOfCode2024.Types;
using AdventOfCode2024.Utility;
using AdventOfCode2024.Utility.Math;
using static AdventOfCode2024.Utility.Math.VectorExtensions;

namespace AdventOfCode2024.Days;

public class Day16 : IDay
{
    private string _example = @"###############
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

    private string _example2 = @"#################
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

    private readonly Maze _maze;

    public Day16(bool useExample = false)
    {
        string[] input;
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            input = [.. _example2.Split('\n').Select(x => x.TrimEnd())];
        }
        else
        {
            var inputFile = $"inputData/{GetType().Name}.txt";
            using var sr = new StreamReader(inputFile);
            input = [.. sr.ReadToEnd().Split('\n').Select(x => x.TrimEnd())];
        }
        _maze = new Maze(input);
    }

    public void Part1()
    {
        var result = _maze.Traverse();
        Console.WriteLine($"Best reindeer track score is {result}");
    }

    public void Part2()
    {
        throw new NotImplementedException();
    }

    private struct Cell(Actor parent)
    {
        public Actor Parent { get; set; } = parent;
        public int f = int.MaxValue;
        public int g = int.MaxValue;
        public int Heuristic { get; set; } = int.MaxValue;
        public int Turns { get; set; }
        public Vector Orientation { get; set; } = Vector.Zero;
    }

    private record Maze
    {
        public Maze(string[] input)
        {
            var height = input.Length;
            var width = input[0].Length;

            Bounds = new Boundary(height, width);
            IsWalkable = new bool[width, height];
            Cells = new Cell[width, height];

            for (int y = 0; y < height; y++)
                for (int x = 0; x < height; x++)
                {
                    var cell = input[y][x];
                    IsWalkable[x, y] = cell != '#';
                    Cells[x, y] = new Cell(new Actor(new Point(-1, -1), Vector.Zero));

                    if (cell == 'S')
                    {
                        Start = new Point(x, y);
                        Reindeer = new Actor(Start, Vector.East);
                        Cells[x, y].Parent = Reindeer;
                        Cells[x, y].f = 0;
                        Cells[x, y].g = 0;
                        Cells[x, y].Heuristic = 0;
                        Cells[x, y].Turns = 0;
                    }
                    if (cell == 'E')
                        End = new Point(x, y);
                }
        }

        public bool[,] IsWalkable { get; set; }
        public Cell[,] Cells { get; set; }
        public Boundary Bounds { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }
        public Actor Reindeer { get; set; }

        public int Distance(Point current)
        {
            return Math.Abs(current.X - End.X) + Math.Abs(current.Y - End.Y);
        }

        public int Traverse()
        {
            var closedList = new bool[Bounds.Width, Bounds.Height];
            var openList = new SortedSet<(int FScore, Actor Position)>(new PriorityComparer())
            {
                (0, Reindeer)
            };

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
                    if (!IsWalkable[nextPosition.X, nextPosition.Y])
                        continue;

                    if (nextPosition == End)
                    {
                        var parentCell = Cells[parent.Location.X, parent.Location.Y];
                        ref var cell = ref Cells[nextPosition.X, nextPosition.Y];
                        cell.Parent = parent;
                        cell.Turns = parentCell.Turns;
                        return FollowPath();
                    }

                    if (!closedList[nextPosition.X, nextPosition.Y]
                        && IsWalkable[nextPosition.X, nextPosition.Y])
                    {
                        ref var parentCell = ref Cells[parent.Location.X, parent.Location.Y];
                        var turns = parentCell.Turns + (isTurning ? 1 : 0);
                        var g = parentCell.g + 1;
                        var h = Distance(nextPosition);
                        var f = g + h + turns * 1000;

                        ref var nextCell = ref Cells[nextPosition.X, nextPosition.Y];

                        if (nextCell.f == int.MaxValue || nextCell.f > f)
                        {
                            var updatedReindeer = new Actor(nextPosition, direction); //possibly need to invert direction vector
                            openList.Add((f, updatedReindeer));
                            nextCell.f = f;
                            nextCell.g = g;
                            nextCell.Heuristic = h;
                            nextCell.Parent = parent;
                            nextCell.Turns = turns;
                            nextCell.Orientation = direction;
                        }
                        // Print(closedList, parent, nextPosition);
                    }
                }
            }
            throw new Exception("Could not find a reasonable path");
        }

        private int FollowPath()
        {
            var path = new List<Actor>();
            var y = End.Y;
            var x = End.X;

            while (!(Cells[x, y].Parent.Location.X == x && Cells[x, y].Parent.Location.Y == y))
            {
                var point = new Point(x, y);
                var nextParent = Cells[x, y].Parent;
                var orientation = Vector.Delta(point, nextParent.Location);
                var pathSegment = new Actor(point, orientation);

                path.Add(pathSegment);
                x = nextParent.Location.X;
                y = nextParent.Location.Y;
            }

            // path.Add(new Actor(new Point(x, y), Vector.East)); //last point is start position

            Print([.. path]);
            return path.Count + Cells[End.X, End.Y].Turns * 1000;
        }

        private void Print(bool[,] closedList, Actor parent, Point next)
        {
            var sb = new StringBuilder();
            for (var y = 0; y < Bounds.Height; y++)
            {
                for (var x = 0; x < Bounds.Width; x++)
                {
                    var compPoint = new Point(x, y);

                    if (!IsWalkable[x, y]) sb.Append('#');
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
            Console.WriteLine($"Next Cell: {cell.Parent.Location}:{cell.Parent.Direction} f:{cell.f} g:{cell.g} h:{cell.Heuristic} turns: {cell.Turns}\n");
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
                    if (!IsWalkable[x, y]) sb.Append('#');
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
    }
}