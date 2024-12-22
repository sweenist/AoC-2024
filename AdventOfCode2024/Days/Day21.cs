using AdventOfCode2024.Utility.Math;
using static AdventOfCode2024.Utility.Math.VectorExtensions;

namespace AdventOfCode2024.Days;

public class Day21 : IDay
{
    private string _example = @"029A
980A
179A
456A
379A";

    private readonly Dictionary<char, Point> _numPad = new()
    {
            {'7', new Point(0,0)},
            {'8', new Point(1,0)},
            {'9', new Point(2,0)},
            {'4', new Point(0,1)},
            {'5', new Point(1,1)},
            {'6', new Point(2,1)},
            {'1', new Point(0,2)},
            {'2', new Point(1,2)},
            {'3', new Point(2,2)},
            {'0', new Point(1,3)},
            {'A', new Point(2,3)},
        };
    private readonly Dictionary<Vector, Point> _dirPad = new(){
            {Vector.North, new Point(1,0)},
            {Vector.Zero, new Point(2,0)},
            {Vector.West, new Point(0,1)},
            {Vector.South, new Point(1,1)},
            {Vector.East, new Point(2,1)},
        };

    private readonly List<string> _input = [];

    const int ACTIVATE = 1;

    public Day21(bool useExample = false)
    {
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            _input.AddRange(_example.Split('\n'));
        }
        else
        {
            var inputFile = $"inputData/{GetType().Name}.txt";
            using var sr = new StreamReader(inputFile);
            _input.AddRange(sr.ReadToEnd().Split('\n'));
        }
    }

    public void Part1()
    {
        var totalComplexity = 0;
        foreach (var sequence in _input)
        {
            var robot1 = (Moves: 0, Visited: new List<Point>() { _numPad['A'] });
            var robot2 = (Moves: 0, Visited: new List<Point>() { _dirPad[Vector.Zero] });
            var robot3 = (Moves: 0, Visited: new List<Point>() { _dirPad[Vector.Zero] });

            foreach (var seq in sequence)
            {
                robot1.Moves += robot1.Visited[^1].ManhattanDistance(_numPad[seq]) + ACTIVATE;
                robot1.Visited.Add(_numPad[seq]);
            }
            var (_, robot2Visits) = MoveDirectionalRobot(robot2, robot1.Visited, true);
            var (robot3Moves, _) = MoveDirectionalRobot(robot3, robot2Visits, true);
            totalComplexity += int.Parse(sequence.Trim('A')) * robot3Moves;
            Console.WriteLine($"{robot3Moves}, {int.Parse(sequence.Trim('A'))}");
        }

        Console.WriteLine($"Total complexity keypad movements is {totalComplexity}");
    }

    public void Part2()
    {
        throw new NotImplementedException();
    }

    private (int Moves, List<Point> Visited) MoveDirectionalRobot((int Moves, List<Point> Visited) robot, List<Point> visited, bool print = false)
    {
        var printString = "";
        foreach (var (target, source) in Pair(visited))
        {
            var deltaVector = Vector.Delta(target, source);
            var directions = deltaVector.Cardinalize().PreferFirstCardinal(Vector.South);
            var currentDir = Vector.Zero;

            while (directions.Count > 0)
            {
                var dir = directions[0];
                robot.Moves += _dirPad[currentDir].ManhattanDistance(_dirPad[dir]);
                currentDir = dir;

                var horizontal = dir.X != 0;
                var moves = horizontal ? Math.Abs(deltaVector.X) : Math.Abs(deltaVector.Y);
                robot.Moves += moves;
                deltaVector += dir.Invert() * moves;
                robot.Visited.AddRange(Enumerable.Repeat(_dirPad[currentDir], moves));
                printString += MapTokens[currentDir];

                directions.RemoveAt(0);
            }
            var acceptMoves = _dirPad[currentDir].ManhattanDistance(_dirPad[Vector.Zero]) + ACTIVATE;
            deltaVector = Vector.Delta(_dirPad[currentDir], _dirPad[Vector.Zero]);
            Console.WriteLine($"deltaVector to A button: {deltaVector}");
            robot.Moves += acceptMoves;
            printString += string.Join("", Enumerable.Repeat(MapTokens[currentDir], acceptMoves));

            robot.Visited.Add(_dirPad[Vector.Zero]);
            printString += 'A';
        }

        if (print) Console.WriteLine($"{robot.Moves}: {printString}");
        return (robot.Moves, robot.Visited);
    }

    private List<(Point Target, Point Source)> Pair(List<Point> points)
    {
        return points.Skip(1).Zip(points.Take(points.Count - 1), (t, s) => (Target: t, Source: s)).ToList();
    }
}