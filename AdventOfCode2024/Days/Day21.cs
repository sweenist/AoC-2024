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
    /*
         0   1   2
           +---+---+
    0      | ^ | A |
       +---+---+---+
    1  | < + v | > |
       +---+---+---+
    */
    private readonly Dictionary<Vector, Point> _dirPad = new(){
        {Vector.North, new Point(1,0)},
        {Vector.Zero, new Point(2,0)},  //Activate Button
        {Vector.West, new Point(0,1)},
        {Vector.South, new Point(1,1)},
        {Vector.East, new Point(2,1)},
    };

    private readonly Dictionary<(Point Target, Point Source), List<Vector>> memoizedDirPad = [];

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
            var aggregate = new List<Point>();
            // foreach (var pair in Pair(robot1.Visited))
            aggregate.AddRange(MoveRobot((_numPad['0'], _numPad['A']), 1, 2));
            Console.WriteLine(string.Join("\n=>", aggregate));

            // var (_, robot2Visits) = MoveDirectionalRobot(robot2, robot1.Visited, true);
            // var (robot3Moves, _) = MoveDirectionalRobot(robot3, robot2Visits, true);
            // totalComplexity += int.Parse(sequence.Trim('A')) * robot3Moves;
            // Console.WriteLine($"{robot3Moves}, {int.Parse(sequence.Trim('A'))}");
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
        var activationKey = Vector.Zero;
        var currentKey = activationKey;

        foreach (var (target, source) in Pair(visited))
        {
            var deltaVector = Vector.Delta(target, source);
            var directions = deltaVector.Cardinalize().PreferFirstCardinal(Vector.South).PreferFirstCardinal(Vector.East);

            while (directions.Count > 0)
            {
                var dir = directions[0];
                robot.Moves += _dirPad[currentKey].ManhattanDistance(_dirPad[dir]);
                printString += PrintNodes(Vector.Delta(_dirPad[dir], _dirPad[currentKey]));
                currentKey = dir;

                var horizontal = dir.X != 0;
                var moves = horizontal ? Math.Abs(deltaVector.X) : Math.Abs(deltaVector.Y);
                printString += string.Join("", Enumerable.Repeat('A', moves));

                deltaVector += dir.Invert() * moves;
                robot.Visited.AddRange(Enumerable.Repeat(_dirPad[dir], moves));
                robot.Moves += moves;

                directions.RemoveAt(0);
                if (directions.Count == 0) printString += PrintNodes(Vector.Delta(_dirPad[activationKey], _dirPad[dir])) + 'A';

            }
            robot.Moves += _dirPad[currentKey].ManhattanDistance(_dirPad[activationKey]) + ACTIVATE;
            robot.Visited.Add(_dirPad[activationKey]);

            currentKey = activationKey;

        }

        if (print) Console.WriteLine($"{robot.Moves}: {printString}");
        // Console.WriteLine(PrintPoints(robot.Visited));
        return (robot.Moves, robot.Visited);
    }

    private IEnumerable<Point> MoveRobot((Point Target, Point Source) visited, int currentDepth, int maxDepth)
    {
        if (currentDepth > maxDepth)
            yield break;
        var deltaVector = Vector.Delta(visited.Target, visited.Source);
        var returnVector = deltaVector.Invert();
        var directions = deltaVector.Cardinalize().PreferFirstCardinal(Vector.South).PreferFirstCardinal(Vector.East);
        var nextSource = Vector.Zero;
        Vector nextTarget;
        var currentKey = Vector.Zero;
        Console.WriteLine($"delta: {deltaVector}");

        while (directions.Count > 0)
        {
            var dir = directions[0];
            currentKey = dir;

            Console.WriteLine($"dir: {dir}");
            var horizontal = dir.X != 0;
            var moves = horizontal ? Math.Abs(deltaVector.X) : Math.Abs(deltaVector.Y);
            deltaVector += dir.Invert() * moves;

            if (currentDepth == maxDepth)
                foreach (var keyPress in Enumerable.Repeat(_dirPad[dir], moves))
                {
                    yield return keyPress;
                    yield break;
                }


            directions.RemoveAt(0);
            if (directions.Count == 0)
            {
                nextTarget = dir;
                foreach (var keyPress in MoveRobot((_dirPad[nextTarget], _dirPad[nextSource]), currentDepth + 1, maxDepth))
                    yield return keyPress;
            }

        }

        if (currentDepth == maxDepth)
            yield break;
        // robot.Visited.Add(_dirPad[activationKey]);

    }

    private static List<(Point Target, Point Source)> Pair(List<Point> points)
    {
        return points.Skip(1).Zip(points.Take(points.Count - 1), (t, s) => (Target: t, Source: s)).ToList();
    }

    private static string PrintNodes(Vector delta)
    {
        var returnString = "";
        if (delta.Y > 0) returnString += string.Join("", Enumerable.Repeat('v', delta.Y));
        if (delta.X > 0) returnString += string.Join("", Enumerable.Repeat('>', delta.X));
        if (delta.Y < 0) returnString += string.Join("", Enumerable.Repeat('^', -delta.Y));
        if (delta.X < 0) returnString += string.Join("", Enumerable.Repeat('<', -delta.X));

        return returnString;
    }

    private string PrintPoints(List<Point> points)
    {
        var lookup = _dirPad.Select(kvp => new KeyValuePair<Point, char>(kvp.Value, MapTokens[kvp.Key])).ToDictionary();
        return string.Join("", points.Select(p => lookup[p]));
    }
}