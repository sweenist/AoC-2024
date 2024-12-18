using AdventOfCode2024.Types;
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

    public Day18(bool useExample = false)
    {
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            _input.AddRange(_example.Split('\n'));
            _exit = new Point(6, 6);
        }
        else
        {
            var inputFile = $"inputData/{GetType().Name}.txt";
            using var sr = new StreamReader(inputFile);
            _input.AddRange(sr.ReadToEnd().Split('\n'));
            _exit = new Point(70, 70);
        }
    }

    public void Part1()
    {
        var corruptedBytes = _input.Take(1024)
                                    .Select(x => x.Split(',').Select(int.Parse).ToList())
                                    .Select(p => new Point(p[0], p[1]))
                                    .ToList();

        var Map = new Map(corruptedBytes, _exit);
    }

    public void Part2()
    {
        throw new NotImplementedException();
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

            Paths[0, 0] = new Cell(new Point(0, 0)) { TotalScore = 0, Accumulated = 0, Heuristic = 0 };
        }

        public Boundary Bounds { get; set; }
        public bool[,] Corrupted { get; set; }
        public Cell[,] Paths { get; set; }
        public Point Start { get; set; } = new Point(0, 0);
        public Point End { get; set; }

        public (List<Point>, int Turns) Traverse()
        {
            var closedList = new bool[Bounds.Width, Bounds.Height];
            var startingPoints = new[] { (0, Start) };

            var openList = new SortedSet<(int FScore, Point step)>(startingPoints, new PriorityComparer());

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
        private (List<Actor>, int Turns) FollowPath()
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

            return (path, Turns: Cells[End.X, End.Y].Turns);
        }

    }
}