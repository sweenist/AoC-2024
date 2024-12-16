using System.Text;
using AdventOfCode2024.Types;
using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Days;


public class Day6 : IDay
{
    public const string LoopMessage = "Starting loop";

    //     private string _example = @".##..
    // ....#
    // .....
    // .^.#.
    // .....";

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
        }

        var totalGuardSTeps = _map.OriginalPath.Select(x => x.Location).Distinct().Count();
        Console.WriteLine($"The guard took {totalGuardSTeps} steps.");
    }

    //1740
    public void Part2()
    {
        while (!_map.OutOfBounds)
        {
            _map.Move();
        }

        var loopCount = 0;
        var originalPath = _map.OriginalPath.Select(x => x.Location).Distinct().ToList();

        while (originalPath.Count > 1)
        {
            if (_map.OutOfBounds || _map.Looping)
            {
                loopCount += _map.Looping ? 1 : 0;
                _map.SetNewObstacle(originalPath);
            }
            _map.Move();
        }
        Console.WriteLine($"There are {loopCount} locations for obstacles causing guard loops.");
    }

    private record Map
    {
        private Point _guardStartLocation;
        private Vector _guardStartDirection;

        public Map(string[] input)
        {

            Bounds = new Boundary(input.Length, input[0].Length);
            for (var y = 0; y < Bounds.Height; y++)
                for (var x = 0; x < Bounds.Width; x++)
                {
                    if (input[y][x] == '#') Obstacles.Add(new Point(x, y));
                    else if (input[y][x] == '^')
                    {
                        Guard = new Guard(new Point(x, y), Vector.North);
                        _guardStartLocation = new Point(x, y);
                        _guardStartDirection = Vector.North;
                    }
                }
            OriginalPath.Add(Guard!.Stance);
        }

        public Boundary Bounds { get; }
        public List<Point> Obstacles { get; private set; } = [];
        public HashSet<(Point Location, Vector Direction)> OriginalPath { get; private set; } = [];

        public Guard Guard { get; set; }
        public Point ExtraBoundary { get; set; }

        public bool OutOfBounds => Bounds.OutOfBounds(Guard.Location);
        public bool Looping { get; set; }

        public void Move()
        {
            var checkPoint = Guard.Location + Guard.Direction;
            if (ExtraBoundary.Equals(checkPoint) || Obstacles.Any(x => x.Equals(checkPoint)))
                Guard.Turn();
            else
                Guard.Location = checkPoint;

            if (!OutOfBounds)
                Looping = !OriginalPath.Add(Guard.Stance);
        }

        public void SetNewObstacle(List<Point> originalPath)
        {
            while (true)
            {
                var obstacleLocation = originalPath.Last();
                originalPath.Remove(obstacleLocation);

                var location = originalPath.Last();

                if (location.Equals(obstacleLocation))
                    continue;

                Guard.Location = _guardStartLocation;
                Guard.Direction = _guardStartDirection;
                OriginalPath = [];
                OriginalPath.Add(Guard.Stance);

                ExtraBoundary = obstacleLocation;
                Looping = false;
                break;
            }
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
                    else if (ExtraBoundary.Equals(currentPoint)) printChar = '@';
                    else if (OriginalPath.Any(x => x.Location.Equals(currentPoint))) printChar = 'x';
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
