using System.Text;
using AdventOfCode2024.Types;
using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Days;


public class Day6 : IDay
{
    public const string LoopMessage = "Starting loop";

    private string _example = @"....#.....
.........#
..........
..#.......
.......#..
..........
.#..^.....
........#.
#.........
......#...";

    private readonly Map _map;

    /*
        Obstacle locations (example):
        7,9
        3,8
        1,8
        7,7
        6,7
        3,6
    */
    private readonly Point _outOfBounds = new(-1, -1);

    public Day6(bool useExample = false)
    {
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            _map = new Map(_example.Split('\n'));
        }
        else
        {
            var inputFile = $"inputData/{GetType().Name}.txt";
            using var sr = new StreamReader(inputFile);
            _map = new Map(sr.ReadToEnd().Split('\n'));
        }
    }

    public void Part1()
    {
        while (!_map.OutOfBounds)
        {
            _map.Move();
            // Thread.Sleep(250);
            // Console.WriteLine(_map);
        }

        var totalGuardSTeps = _map.OriginalPath.Select(x => x.Location).Distinct().Count();
        Console.WriteLine($"The guard took {totalGuardSTeps} steps.");
    }

    //1740
    public void Part2()
    {

        // Console.WriteLine($"There are {loopingObstacleLocations} locations for obstacles causing guard loops.");
    }

    private record Map
    {
        public Map(string[] input)
        {
            Bounds = new Boundary(input.Count(), input[0].Length);
            for (var y = 0; y < Bounds.Height; y++)
                for (var x = 0; x < Bounds.Width; x++)
                {
                    if (input[y][x] == '#') Obstacles.Add(new Point(x, y));
                    else if (input[y][x] == '^') Guard = new Guard(new Point(x, y), Vector.North);
                }

            OriginalPath.Add(Guard!.Stance);
        }

        public Boundary Bounds { get; }
        public List<Point> Obstacles { get; private set; } = [];
        public HashSet<(Point Location, Vector Direction)> OriginalPath { get; } = [];

        public Guard Guard { get; set; }
        public Point ExtraBoundary { get; set; }

        public bool OutOfBounds => Bounds.OutOfBounds(Guard.Location);
        public bool Looping { get; set; }

        public void Move()
        {
            var checkPoint = Guard.Location + Guard.Direction;
            if (Obstacles.Any(x => x.Equals(checkPoint)))
            {
                Guard.Turn();
            }
            else
            {
                Guard.Location = checkPoint;
            }
            if (!OutOfBounds)
                Looping = !OriginalPath.Add(Guard.Stance);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var y = 0; y < Bounds.Height; y++)
            {
                for (var x = 0; x < Bounds.Width; x++)
                {
                    var printChar = '.';
                    var currentPoint = new Point(x, y);
                    if (Guard.Location == currentPoint)
                        printChar = Guard.Direction == Vector.North ? '^'
                            : Guard.Direction == Vector.East ? '>'
                            : Guard.Direction == Vector.South ? 'v'
                            : '<'; //West
                    else if (Obstacles.Any(x => x.Equals(currentPoint))) printChar = '#';
                    sb.Append(printChar);
                }
                sb.Append('\n');
            }
            return sb.ToString();
        }
    }

    private record Guard(Point Location, Vector Direction)
    {
        public Point Location { get; set; } = Location;
        public Vector Direction { get; set; } = Direction;

        public (Point Location, Vector Direction) Stance => (Location, Direction);

        public void Turn() => Direction = Direction.Clockwise();

    }
}
