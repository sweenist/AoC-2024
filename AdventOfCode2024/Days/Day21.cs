using AdventOfCode2024.Types.Day21;
using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Days;

public partial class Day21 : IDay
{
    private string _example = @"029A
980A
179A
456A
379A";

    private readonly Dictionary<string, string> _assertions = new()
    {
        {"029A", "<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A"},
        {"980A", "<v<A>>^AAAvA^A<vA<AA>>^AvAA<^A>A<v<A>A>^AAAvA<^A>A<vA>^A<A>A"},
        {"179A", "<v<A>>^A<vA<A>>^AAvAA<^A>A<v<A>>^AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A"},
        {"456A", "<v<A>>^AA<vA<A>>^AAvAA<^A>A<vA>^A<A>A<vA>^A<A>A<v<A>A>^AAvA<^A>A"},
        {"379A", "<v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A"},
    };

    private readonly Dictionary<(Point A, Point B), List<List<Vector>>> _possibleNumPaths;

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


    private readonly Dictionary<Point, Vector> _dirPad = new()
    {
        { new Point(1,0), Vector.North},
        { new Point(2,0), Vector.Zero},  //Activate Button
        { new Point(0,1), Vector.West},
        { new Point(1,1), Vector.South},
        { new Point(2,1), Vector.East},
    };
    private readonly Dictionary<Vector, Point> _dirPadButtons = new()
    {
        {Vector.North, new Point(1,0)},
        {Vector.Zero, new Point(2,0)},  //Activate Button
        {Vector.West, new Point(0,1)},
        {Vector.South, new Point(1,1)},
        {Vector.East, new Point(2,1)},
    };

    private readonly List<string> _input = [];

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
        _possibleNumPaths = PopulateVectorDictionary().ToDictionary();
    }

    public void Part1()
    {
        var result = Solve(3);
        Console.WriteLine($"The result was {result}");
        return;
        var totalComplexity = 0L;
        var manager = new SpecManager();
        var radiationRobot = new Robot(manager, "Robbie");
        var freezingTobot = new Robot(manager, "Freyda");

        radiationRobot.Controller = freezingTobot;

        foreach (var sequence in _input)
        {
            var blah = InitializeNumberSequence(sequence);
            foreach (var keySequence in blah)
                radiationRobot.Move(keySequence, true);
            totalComplexity += int.Parse(sequence.Trim('A')) * freezingTobot.ActionsPerformed;


            Console.WriteLine($"{sequence} had {freezingTobot.ActionsPerformed} moves");
            Console.WriteLine($"{sequence}:\n\t{freezingTobot.Actions}\n\t{_assertions[sequence]}");
            // Console.WriteLine($"{radiationRobot.Name}:\n\t{radiationRobot.Actions}");
            // Console.WriteLine($"{freezingTobot.Name}:\n\t{freezingTobot.Actions}");
            radiationRobot.Reset();
            // break;
        }

        Console.WriteLine($"Total complexity keypad movements is {totalComplexity}");
    }

    public void Part2()
    {
        throw new NotImplementedException();
    }

    private long Solve(int maxDepth)
    {
        var result = 0L;
        var numSequence = InitializeNumberSequence(_input[0]);
        var pathCache = new Dictionary<(Point Start, Point End, int Depth), long>();

        foreach (var seq in numSequence)
        {
            result += ProcessKeyAction(seq, _dirPadButtons[Vector.Zero], maxDepth, pathCache);
        }

        return result;
    }

    private long ProcessKeyAction(Vector direction, Point initial, int depth, Dictionary<(Point Start, Point End, int Depth), long> cache)
    {
        var score = long.MaxValue;
        var endPoint = _dirPadButtons[direction];
        if (cache.TryGetValue((initial, endPoint, depth), out var previous))
            return previous;

        foreach (var paths in _possibleNumPaths[(initial, endPoint)])
        {
            if (depth == 0)
            {
                score = 1;
                break;
            }
            var startPoint = initial;
            Console.WriteLine($"Score for {string.Join(',', paths)}  at {initial} {depth} in direction {direction}");
            var pathLength = paths.Select(v => ProcessKeyAction(v, startPoint += v, depth - 1, cache)).Sum();

            score = Math.Min(score, pathLength);
        }

        cache.Add((initial, endPoint, depth), score);

        return score;
    }

    private List<Vector> InitializeNumberSequence(string sequence)
    {
        var visited = sequence.Select(s => _numPad[s]).Prepend(_numPad['A']);
        var vectorDeltas = visited.Zip(visited.Skip(1), (target, src) => Vector.Delta(src, target))
                                  .SelectMany(x => new[] { x, Vector.Zero })
                                  .Stepify().ToList();
        Console.WriteLine($"{sequence}-> {string.Join(',', vectorDeltas)}");
        return vectorDeltas;
    }

    private static IEnumerable<KeyValuePair<(Point A, Point B), List<List<Vector>>>> PopulateVectorDictionary()
    {
        var up = new Point(1, 0);
        var activate = new Point(2, 0);
        var left = new Point(0, 1);
        var down = new Point(1, 1);
        var right = new Point(2, 1);

        //from activate button paths
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((activate, activate), [[Vector.Zero]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((activate, up), [[Vector.West]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((activate, down),
            [[Vector.West, Vector.South], [Vector.South, Vector.West]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((activate, right), [[Vector.South]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((activate, left),
            [[Vector.West, Vector.South, Vector.West], [Vector.South, Vector.West, Vector.West]]);

        //from up button paths
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((up, activate), [[Vector.East]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((up, up), [[Vector.Zero]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((up, down), [[Vector.South]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((up, right),
            [[Vector.South, Vector.East], [Vector.East, Vector.South]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((up, left),
            [[Vector.South, Vector.West], [Vector.South, Vector.West]]);

        //from down button paths
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((down, activate),
            [[Vector.North, Vector.East], [Vector.East, Vector.North]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((down, up), [[Vector.North]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((down, down), [[Vector.Zero]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((down, right), [[Vector.East]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((down, left), [[Vector.West]]);

        //from left button paths
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((left, activate),
            [[Vector.East, Vector.North, Vector.East], [Vector.East, Vector.East, Vector.North]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((left, up),
            [[Vector.East, Vector.North]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((left, down), [[Vector.West]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((left, right),
            [[Vector.East, Vector.East]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((left, left), [[Vector.Zero]]);

        //from right button paths
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((right, activate), [[Vector.North]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((right, up),
            [[Vector.West, Vector.North], [Vector.North, Vector.West]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((right, down), [[Vector.West]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((right, right), [[Vector.Zero]]);
        yield return new KeyValuePair<(Point A, Point B), List<List<Vector>>>((right, left),
            [[Vector.West, Vector.West]]);
    }
}