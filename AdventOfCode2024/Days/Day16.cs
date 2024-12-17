using AdventOfCode2024.Types;
using AdventOfCode2024.Utility.Math;

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

    private readonly Maze _maze;

    public Day16(bool useExample = false)
    {
        string[] input;
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            input = [.. _example.Split('\n')];
        }
        else
        {
            var inputFile = $"inputData/{GetType().Name}.txt";
            using var sr = new StreamReader(inputFile);
            input = [.. sr.ReadToEnd().Split('\n')];
        }
        _maze = new Maze(input);
    }

    public void Part1()
    {
        _maze.Traverse();
    }

    public void Part2()
    {
        throw new NotImplementedException();
    }

    private struct Cell(Point location)
    {
        public Point Parent { get; set; } = location;
        public int f = int.MaxValue;
        public int g = int.MaxValue;
        public int Heuristic { get; set; } = int.MaxValue;
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
                    Cells[x, y] = new Cell(new Point(-1, -1));

                    if (cell == 'S')
                    {
                        Start = new Point(x, y);
                        Cells[x, y].Parent = Start;
                        Cells[x, y].f = 0;
                        Cells[x, y].g = 0;
                        Cells[x, y].Heuristic = 0;
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

        public int Distance(Point current)
        {
            return Math.Abs(current.X - End.X) + Math.Abs(current.Y - End.Y);
        }

        public void Traverse()
        {
            var closedList = new bool[Bounds.Width, Bounds.Height];
            var openList = new SortedSet<(int FScore, Point Position)>(Comparer<(int FScore, Point Position)>.Create((a, b) => a.FScore.CompareTo(b.FScore)))
            {
                (0, Start)
            };

            while (openList.Count > 0)
            {
                var point = openList.Min;
                openList.Remove(point);
                var (_, parent) = point;
                closedList[parent.X, parent.Y] = true;

                foreach (var direction in Vector.CardinalPoints)
                {
                    var nextPosition = parent + direction;
                    if (!IsWalkable[nextPosition.X, nextPosition.Y])
                        continue;

                    if (nextPosition == End)
                    {
                        ref var cell = ref Cells[nextPosition.X, nextPosition.Y];
                        cell.Parent = nextPosition;
                        FollowPath();
                        return;
                    }

                    if (!closedList[nextPosition.X, nextPosition.Y]
                        && IsWalkable[nextPosition.X, nextPosition.Y])
                    {
                        ref var cell = ref Cells[parent.X, parent.Y];
                        var g = cell.g + 1;
                        var h = Distance(nextPosition);
                        var f = g + h;

                        Console.WriteLine($"F Values {cell.f} {f}");

                        if (cell.f == int.MaxValue || cell.f > f)
                        {
                            Console.WriteLine($"adding score {f} for {nextPosition} from {parent}");
                            openList.Add((f, nextPosition));
                            cell.f = f;
                            cell.g = g;
                            cell.Heuristic = h;
                            cell.Parent = parent;
                        }
                    }
                }
            }

        }
        private void FollowPath()
        {
            var path = new Stack<Point>();
            var row = End.Y;
            var column = End.X;

            while (!(Cells[column, row].Parent.X == column && Cells[column, row].Parent.Y == row))
            {
                path.Push(new Point(column, row));
                var nextParent = Cells[column, row].Parent;
                row = nextParent.Y;
                column = nextParent.X;
            }

            path.Push(new Point(column, row));
            while (path.Count > 0)
            {
                var point = path.Pop();
                Console.WriteLine($" -> {point}");
            }
        }
    }
}