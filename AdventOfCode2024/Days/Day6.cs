using System.Collections;
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
        var steps = CountGuardSteps(guard, barriers);

        Console.WriteLine($"The guard took {steps} steps.");
    }

    public void Part2()
    {
        throw new NotImplementedException();
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
        // Console.WriteLine($"fieldWidth: {fieldWidth}; fieldHeight: {fieldHeight}; rawIndex: {rawIndex}");
        var x = rawIndex % FieldWidth;
        var y = rawIndex / FieldHeight;
        return new Vector2(x, y);
    }

    private long CountGuardSteps(Guard guard, List<Vector2> barriers)
    {
        var encountersBarriers = true;

        while (encountersBarriers)
        {
            Vector2 nextObstacle = _outOfBounds;
            switch (guard.Direction)
            {
                case Direction.North:
                    nextObstacle = barriers
                       .Where(v => v.Y < guard.Position.Y && v.X == guard.Position.X)
                       .OrderByDescending(v => v.Y)
                       .FirstOrDefault(_outOfBounds);
                    break;
                case Direction.East:
                    nextObstacle = barriers
                            .Where(v => v.X > guard.Position.X && v.Y == guard.Position.Y)
                            .OrderBy(v => v.X)
                            .FirstOrDefault(_outOfBounds);
                    break;
                case Direction.South:
                    nextObstacle = barriers
                       .Where(v => v.Y > guard.Position.Y && v.X == guard.Position.X)
                       .OrderBy(v => v.Y)
                       .FirstOrDefault(_outOfBounds);
                    break;
                case Direction.West:
                    nextObstacle = barriers
                            .Where(v => v.X < guard.Position.X && v.Y == guard.Position.Y)
                            .OrderByDescending(v => v.X)
                            .FirstOrDefault(_outOfBounds);
                    break;
            }
            encountersBarriers = nextObstacle != _outOfBounds;

            if (encountersBarriers)
                guard.Turn(nextObstacle);
            else
            {
                switch (guard.Direction)
                {
                    case Direction.North:
                        guard.Turn(new Vector2(guard.Position.X, -1));
                        break;
                    case Direction.East:
                        guard.Turn(new Vector2(FieldWidth, guard.Position.Y));
                        break;
                    case Direction.South:
                        guard.Turn(new Vector2(guard.Position.X, FieldHeight));
                        break;
                    case Direction.West:
                        guard.Turn(new Vector2(-1, guard.Position.Y));
                        break;
                }
            }
        }
        Console.WriteLine($"Max Width and Height {FieldWidth}, {FieldHeight}");
        return guard.Steps.Keys.ToList().Count;
    }

    private class Guard(Vector2 currentPosition)
    {
        private readonly Dictionary<string, int> _steps = new Dictionary<string, int>();
        public Vector2 Position { get; private set; } = currentPosition;
        public Direction Direction { get; private set; }

        public Dictionary<string, int> Steps => _steps;


        /// <summary>
        /// Changes guard direction and accounts steps.
        /// </summary>
        /// <param name="obstacle">The position of the barrier encountered.</param>
        public void Turn(Vector2 obstacle)
        {
            switch (Direction)
            {
                case Direction.North:
                    for (var y = Position.Y; y > obstacle.Y; y--)
                    {
                        var vector = $"{Position.X},{y}";
                        if (!Steps.ContainsKey(vector))
                            Console.WriteLine($"\t{vector}");
                        Steps[vector] = 1;

                    }
                    break;
                case Direction.East:
                    for (var x = Position.Y; x < obstacle.X; x++)
                    {
                        var vector = $"{x},{Position.Y}";
                        if (!Steps.ContainsKey(vector))
                            Console.WriteLine($"\t{vector}");
                        Steps[vector] = 1;
                    }
                    break;
                case Direction.South:
                    for (var y = Position.Y; y < obstacle.Y; y++)
                    {
                        var vector = $"{Position.X},{y}";
                        if (!Steps.ContainsKey(vector))
                            Console.WriteLine($"\t{vector}");
                        Steps[vector] = 1;
                    }
                    break;
                case Direction.West:
                    for (var x = Position.X; x > obstacle.X; x--)
                    {
                        var vector = $"{x},{Position.Y}";
                        if (!Steps.ContainsKey(vector))
                            Console.WriteLine($"\t{vector}");
                        Steps[vector] = 1;
                    }
                    break;
            }
            Position = Direction switch
            {
                Direction.North => new Vector2(obstacle.X, obstacle.Y + 1),
                Direction.East => new Vector2(obstacle.X - 1, obstacle.Y),
                Direction.South => new Vector2(obstacle.X, obstacle.Y - 1),
                Direction.West => new Vector2(obstacle.X + 1, obstacle.Y),
                _ => throw new ArgumentOutOfRangeException(nameof(Direction), "Whatever Direction is... it's wrong"),
            };
            Direction = (Direction)(((int)Direction + 1) % 4);
        }
    }
}
