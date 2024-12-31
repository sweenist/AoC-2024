using AdventOfCode2024.Utility.Math;
using static AdventOfCode2024.Utility.Math.VectorExtensions;

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
        {'X', new Point(0,3)},
        {'0', new Point(1,3)},
        {'A', new Point(2,3)},
    };

    private readonly Dictionary<char, Point> _dirPad = new()
    {
        {'X', new Point(0,0)},  //Berserker "button"
        {'^', new Point(1,0)},
        {'A', new Point(2,0)},  //Activate Button
        {'<', new Point(0,1)},
        {'v', new Point(1,1)},
        {'>', new Point(2,1)},
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
    }

    public void Part1()
    {
        var result = Solve(3);
        Console.WriteLine($"The complexity code of two keypad robots was {result}");
    }

    public void Part2()
    {
        var result = Solve(25);
        Console.WriteLine($"The complexity code of twenty five keypad robots was {result}");
    }

    private long Solve(int maxDepth)
    {
        var result = 0L;

        var pathCache = new Dictionary<(char Start, char Next, int Depth), long>();

        foreach (var seq in _input)
        {
            var multiplier = int.Parse(seq.TrimEnd('A'));
            var sequenceResult = ProcessKeyAction(seq, maxDepth, pathCache, true);
            result += sequenceResult * multiplier;
        }

        return result;
    }

    private long ProcessKeyAction(string keySequence, int depth, Dictionary<(char Start, char Next, int Depth), long> cache, bool firstRun = false)
    {
        if (depth == 0)
            return keySequence.Length;

        var currentKey = 'A';
        var keyLength = 0L;
        foreach (var key in keySequence)
        {
            keyLength += CacheShortest(currentKey, key, depth, cache, firstRun);
            currentKey = key;
        }

        return keyLength;
    }

    private long CacheShortest(char startKey, char nextKey, int depth, Dictionary<(char Start, char Next, int Depth), long> cache, bool firstRun = false)
    {
        if (cache.TryGetValue((startKey, nextKey, depth), out var existingLength)) return existingLength;

        var length = long.MaxValue;
        var startPos = firstRun ? _numPad[startKey] : _dirPad[startKey];
        var endPos = firstRun ? _numPad[nextKey] : _dirPad[nextKey];

        var deltaY = endPos.Y - startPos.Y;
        var deltaX = endPos.X - startPos.X;

        var vertical = new string(deltaY < 0 ? '^' : 'v', Math.Abs(deltaY));
        var horizontal = new string(deltaX < 0 ? '<' : '>', Math.Abs(deltaX));

        var XRef = new Point(startPos.X, endPos.Y);
        var berserker = firstRun ? _numPad['X'] : _dirPad['X'];

        if (XRef != berserker)
            length = Math.Min(length, ProcessKeyAction($"{vertical}{horizontal}A", depth - 1, cache));

        XRef = new Point(endPos.X, startPos.Y);
        if (XRef != berserker)
            length = Math.Min(length, ProcessKeyAction($"{horizontal}{vertical}A", depth - 1, cache));

        return length;
    }
}