using System.Numerics;
using AdventOfCode2024.Enums;
using AdventOfCode2024.Utility;

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

    private readonly List<string> _input = [];
    private readonly Vector2 _outOfBounds = new(-1, -1);

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

    public void Part2()
    {
        var barriers = _input.SelectMany(SetupFieldRow).ToList();
        var guardStartPosition = GetGuardStartPosition();
        var guard = new Guard(GetGuardStartPosition());
        RunGuardTrack(guard, barriers);

        // Console.WriteLine($"Distinct Guard trace:\n\t{string.Join("\n\t", guard.Path.Select(v => v.DisplayDirection()))}");

        var loopingObstacleLocations = GetLoopCausingObstacles(guard, barriers);

        Console.WriteLine($"There are {loopingObstacleLocations} locations for obstacles causing guard loops.");
    }

    private int FieldWidth => _input[0].Length;
    private int FieldHeight => _input.Count;

    private IEnumerable<Vector2> SetupFieldRow(string row, int rowIndex)
    {
        return row.Select((x, i) => x == '#'
            ? new Vector2(i, rowIndex)
            : _outOfBounds).Where(x => !x.Equals(_outOfBounds));
    }

    private Vector2 GetGuardStartPosition()
    {
        var rawIndex = string.Join("", _input).ToList().FindIndex(c => c == '^');
        var x = rawIndex % FieldWidth;
        var y = rawIndex / FieldHeight;
        return new Vector2(x, y);
    }

    private void RunGuardTrack(Guard guard, List<Vector2> barriers)
    {
        var encountersBarriers = true;

        while (encountersBarriers)
        {
            Vector2 nextObstacle = _outOfBounds;
            nextObstacle = guard.Direction switch
            {
                Direction.North => barriers.Where(v => v.Y < guard.Position.Y && v.X == guard.Position.X)
                                           .OrderByDescending(v => v.Y).FirstOrDefault(_outOfBounds),
                Direction.East => barriers.Where(v => v.X > guard.Position.X && v.Y == guard.Position.Y)
                                           .OrderBy(v => v.X).FirstOrDefault(_outOfBounds),
                Direction.South => barriers.Where(v => v.Y > guard.Position.Y && v.X == guard.Position.X)
                                           .OrderBy(v => v.Y).FirstOrDefault(_outOfBounds),
                Direction.West => barriers.Where(v => v.X < guard.Position.X && v.Y == guard.Position.Y)
                                          .OrderByDescending(v => v.X).FirstOrDefault(_outOfBounds),
                _ => throw new InvalidOperationException("Direction was not cardinal")
            };

            encountersBarriers = nextObstacle != _outOfBounds;

            if (encountersBarriers)
                guard.Turn(nextObstacle);
            else
            {
                var outOfField = guard.Direction switch
                {
                    Direction.North => new Vector2(guard.Position.X, -1),
                    Direction.East => new Vector2(FieldWidth, guard.Position.Y),
                    Direction.South => new Vector2(guard.Position.X, FieldHeight),
                    Direction.West => new Vector2(-1, guard.Position.Y),
                    _ => throw new InvalidOperationException("Direction was not cardinal")
                };
                guard.Turn(outOfField);
                Console.WriteLine($"\tGuard: looping {guard.IsLooping}");
            }
        }
    }

    private int GetLoopCausingObstacles(Guard guard, List<Vector2> barriers)
    {
        var pathCopy = new Stack<Vector3>(guard.Path);
        var obstacleCount = 0;

        var obstacles = new List<Vector2>();

        while (pathCopy.Count > 1)
        {
            var obstacle = pathCopy.Pop();
            var guardStart = pathCopy.First();
            var barriersCopy = barriers.ToList();
            barriersCopy.Add(obstacle.ToVector2());
            // Console.WriteLine($"Popped {obstacle.DisplayDirection()}");
            // Console.WriteLine($"\tGuard at {guardStart.DisplayDirection()}");

            guard.Reposition(guardStart);
            try
            {
                RunGuardTrack(guard, barriersCopy);
            }
            catch (Exception e)
            {
                if (e.Message.Equals(LoopMessage))
                {
                    obstacleCount++;
                    obstacles.Add(obstacle.ToVector2());
                }
                else
                    throw;
            }
        }
        Console.WriteLine($"Obstacles:\n\t{string.Join("\n\t", obstacles)}");

        return obstacleCount;
    }

    private class Guard(Vector2 currentPosition)
    {
        private readonly HashSet<Vector2> _steps = [];
        private readonly HashSet<Vector3> _pathLocations = [];

        public Vector2 Position { get; private set; } = currentPosition;
        public Direction Direction { get; private set; }
        public bool IsLooping { get; private set; }

        /// <summary>A unique set of points the guard visits.</summary>
        public HashSet<Vector2> Steps => _steps;

        /// <summary>The locations(x, y) and direction(z as <seealso cref="Direction" />)</summary>
        public HashSet<Vector3> Path => _pathLocations;


        /// <summary>Changes guard direction and accounts steps.</summary>
        /// <param name="obstacle">The position of the barrier encountered.</param>
        public void Turn(Vector2 obstacle)
        {
            switch (Direction)
            {
                case Direction.North:
                    for (var y = Position.Y; y > obstacle.Y; y--)
                        Traverse(new Vector3(Position.X, y, (int)Direction));
                    break;
                case Direction.East:
                    for (var x = Position.X; x < obstacle.X; x++)
                        Traverse(new Vector3(x, Position.Y, (int)Direction));
                    break;
                case Direction.South:
                    for (var y = Position.Y; y < obstacle.Y; y++)
                        Traverse(new Vector3(Position.X, y, (int)Direction));
                    break;
                case Direction.West:
                    for (var x = Position.X; x > obstacle.X; x--)
                        Traverse(new Vector3(x, Position.Y, (int)Direction));
                    break;
            }

            Position = Direction switch
            {
                Direction.North => new Vector2(obstacle.X, obstacle.Y + 1),
                Direction.East => new Vector2(obstacle.X - 1, obstacle.Y),
                Direction.South => new Vector2(obstacle.X, obstacle.Y - 1),
                Direction.West => new Vector2(obstacle.X + 1, obstacle.Y),
                _ => throw new InvalidOperationException("Whatever Direction is... it's wrong"),
            };
            Direction = (Direction)(((int)Direction + 1) % 4);
        }

        public void Reposition(Vector3 coordinates)
        {
            Position = coordinates.ToVector2();
            Direction = (Direction)(int)coordinates.Z;

            // Reset the path
            var revisedPath = Path.ToList();
            var locationIndex = revisedPath.IndexOf(coordinates);
            _pathLocations.Clear();
            _pathLocations.UnionWith(revisedPath.Take(locationIndex));

            IsLooping = false;

            // Console.WriteLine($"Update: Last path {_pathLocations.Last().DisplayDirection()}");
        }

        public override string ToString()
        {
            return $"Guard: P:{Position}; D:{Direction}";
        }


        private void Traverse(Vector3 location)
        {
            if (_pathLocations.Contains(location))
            {
                IsLooping = true;
                throw new Exception(LoopMessage); //Dumb. figure out different execution breaking mechanism
            }

            _pathLocations.Add(location);
            _steps.Add(location.ToVector2());
        }
    }
}
