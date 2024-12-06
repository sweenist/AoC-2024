using System.Numerics;
using AdventOfCode2024.Enums;

namespace AdventOfCode2024.Days;

public class Day6 : IDay
{
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
            }
        }
    }

    private class Guard(Vector2 currentPosition)
    {
        private readonly HashSet<string> _steps = [];
        public Vector2 Position { get; private set; } = currentPosition;
        public Direction Direction { get; private set; }

        public HashSet<string> Steps => _steps;


        /// <summary>Changes guard direction and accounts steps.</summary>
        /// <param name="obstacle">The position of the barrier encountered.</param>
        public void Turn(Vector2 obstacle)
        {
            switch (Direction)
            {
                case Direction.North:
                    for (var y = Position.Y; y > obstacle.Y; y--)
                    {
                        var vector = $"{Position.X},{y}";
                        Steps.Add(vector);

                    }
                    break;
                case Direction.East:
                    for (var x = Position.X; x < obstacle.X; x++)
                    {
                        var vector = $"{x},{Position.Y}";
                        Steps.Add(vector);
                    }
                    break;
                case Direction.South:
                    for (var y = Position.Y; y < obstacle.Y; y++)
                    {
                        var vector = $"{Position.X},{y}";
                        Steps.Add(vector);
                    }
                    break;
                case Direction.West:
                    for (var x = Position.X; x > obstacle.X; x--)
                    {
                        var vector = $"{x},{Position.Y}";
                        Steps.Add(vector);
                    }
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
    }
}
