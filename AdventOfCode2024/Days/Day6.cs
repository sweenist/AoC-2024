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

    /*
        Obstacle locations (example):
        7,9
        3,8
        1,8
        7,7
        6,7
        3,6
    */

    private readonly List<string> _input = [];
    private readonly Point _outOfBounds = new(-1, -1);

    public Day6(bool useExample = false)
    {
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            _input.AddRange(_example.Split('\n'));
            return;
        }
        var inputFile = $"inputData/{GetType().Name}.txt";
        using var sr = new StreamReader(inputFile);
        _input.AddRange(sr.ReadToEnd().Split('\n'));
    }

    public void Part1()
    {
        var barriers = _input.SelectMany(SetupFieldRow).ToList();
        var guardStartPosition = GetGuardStartPosition();
        var guard = new Guard(GetGuardStartPosition());

        RunGuardTrack(guard, barriers);

        Console.WriteLine($"The guard took {guard.Steps.Count} steps.");
    }

    //1740
    public void Part2()
    {
        var barriers = _input.SelectMany(SetupFieldRow).ToList();
        var guardStartPosition = GetGuardStartPosition();
        var guard = new Guard(GetGuardStartPosition());
        RunGuardTrack(guard, barriers);

        var loopingObstacleLocations = GetLoopCausingObstacles(guard, barriers);

        Console.WriteLine($"There are {loopingObstacleLocations} locations for obstacles causing guard loops.");
    }

    private int FieldWidth => _input[0].Length;
    private int FieldHeight => _input.Count;

    private IEnumerable<Point> SetupFieldRow(string row, int rowIndex)
    {
        return row.Select((x, i) => x == '#'
            ? new Point(i, rowIndex)
            : _outOfBounds).Where(x => !x.Equals(_outOfBounds));
    }

    private Point GetGuardStartPosition()
    {
        var rawIndex = string.Join("", _input).ToList().FindIndex(c => c == '^');
        var x = rawIndex % FieldWidth;
        var y = rawIndex / FieldHeight;
        return new Point(x, y);
    }

    private void RunGuardTrack(Guard guard, List<Point> barriers)
    {
        var encountersBarriers = true;

        while (encountersBarriers)
        {
            Point nextObstacle = _outOfBounds;

            if (guard.Direction.Equals(Vector.North))
                nextObstacle = barriers.Where(v => v.Y < guard.Position.Y && v.X == guard.Position.X)
                                       .OrderByDescending(v => v.Y).FirstOrDefault(_outOfBounds);
            else if (guard.Direction.Equals(Vector.East))
                nextObstacle = barriers.Where(v => v.X > guard.Position.X && v.Y == guard.Position.Y)
                                       .OrderBy(v => v.X).FirstOrDefault(_outOfBounds);
            else if (guard.Direction.Equals(Vector.South))
                nextObstacle = barriers.Where(v => v.Y > guard.Position.Y && v.X == guard.Position.X)
                                       .OrderBy(v => v.Y).FirstOrDefault(_outOfBounds);
            else if (guard.Direction.Equals(Vector.West))
                nextObstacle = barriers.Where(v => v.X < guard.Position.X && v.Y == guard.Position.Y)
                                       .OrderByDescending(v => v.X).FirstOrDefault(_outOfBounds);

            encountersBarriers = nextObstacle != _outOfBounds;

            if (encountersBarriers)
            {
                guard.Turn(nextObstacle);
                if (guard.IsLooping) return;
            }
            else
            {
                if (guard.Direction == Vector.North)
                    guard.Turn(new Point(guard.Position.X, -1));
                else if (guard.Direction == Vector.East)
                    guard.Turn(new Point(FieldWidth, guard.Position.Y));
                else if (guard.Direction == Vector.South)
                    guard.Turn(new Point(guard.Position.X, FieldHeight));
                else if (guard.Direction == Vector.West)
                    guard.Turn(new Point(-1, guard.Position.Y));
            }
        }
    }

    private int GetLoopCausingObstacles(Guard guard, List<Point> barriers)
    {
        var pathCopy = new Stack<(Point Location, Vector Direction)>(guard.Path);
        var obstacleCount = 0;

        var obstacles = new HashSet<Point>();

        while (pathCopy.Count > 1)
        {
            var obstacle = pathCopy.Pop().Location;
            var guardStart = pathCopy.First();
            if (obstacle == guardStart.Location)
            {
                continue;
            }
            var barriersCopy = barriers.ToList();
            barriersCopy.Add(obstacle);

            guard.Reposition();
            RunGuardTrack(guard, barriersCopy);

            if (guard.IsLooping && obstacles.Add(obstacle))
            {
                obstacleCount++;
            }
        };

        return obstacleCount;
    }

    private class Guard
    {
        private readonly HashSet<Point> _steps = [];
        private readonly HashSet<(Point Location, Vector Direction)> _pathLocations = [];
        private readonly Point _startPosition;

        public Guard(Point startPosition)
        {
            Position = startPosition;
            _startPosition = startPosition;
            Direction = Vector.North;
        }

        public Point Position { get; private set; }
        public Vector Direction { get; private set; }
        public bool IsLooping { get; private set; }

        /// <summary>A unique set of points the guard visits.</summary>
        public HashSet<Point> Steps => _steps;

        /// <summary>The locations(x, y) and direction as a vector.</summary>
        public HashSet<(Point Location, Vector Direction)> Path => _pathLocations;


        /// <summary>Changes guard direction and accounts steps.</summary>
        /// <param name="obstacle">The position of the barrier encountered.</param>
        public void Turn(Point obstacle)
        {
            if (Direction.Equals(Vector.North))
            {
                for (var y = Position.Y; y > obstacle.Y; y--)
                {
                    Traverse((Location: new Point(Position.X, y), Direction));
                    if (IsLooping)
                        break;
                }
                Position = obstacle + Vector.South;
            }
            if (Direction.Equals(Vector.East))
            {
                for (var x = Position.X; x < obstacle.X; x++)
                {
                    Traverse((Location: new Point(x, Position.Y), Direction));
                    if (IsLooping)
                        break;
                }
                Position = obstacle + Vector.West;
            }
            if (Direction.Equals(Vector.South))
            {
                for (var y = Position.Y; y < obstacle.Y; y++)
                {
                    Traverse((Location: new Point(Position.X, y), Direction));
                    if (IsLooping)
                        break;
                }
                Position = obstacle + Vector.North;
            }
            if (Direction.Equals(Vector.West))
            {
                for (var x = Position.X; x > obstacle.X; x--)
                {
                    Traverse((Location: new Point(x, Position.Y), Direction));
                    if (IsLooping)
                        break;
                }
                Position = obstacle + Vector.East;
            }

            Direction = Vector.Clockwise(Direction);
        }

        public void Reposition()
        {
            Position = _startPosition;
            Direction = Vector.North;

            _pathLocations.Clear();

            IsLooping = false;
        }

        public override string ToString()
        {
            return $"Guard: P:{Position}; D:{Direction}; Looping {IsLooping}";
        }


        private void Traverse((Point Location, Vector Direction) position)
        {
            if (!_pathLocations.Add(position))
                IsLooping = true;

            _steps.Add(position.Location);
        }
    }
}
