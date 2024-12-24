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
        var totalComplexity = 0L;
        var manager = new SpecManager();
        var radiationRobot = new Robot(manager);
        var freezingTobot = new Robot(manager);

        radiationRobot.Controller = freezingTobot;

        foreach (var sequence in _input)
        {
            var keySequence = InitializeNumberSequence(sequence);
            radiationRobot.Move(keySequence[0]);
            totalComplexity += int.Parse(sequence.Trim('A')) * freezingTobot.ActionsPerformed;

            Console.WriteLine($"{sequence} had {freezingTobot.ActionsPerformed} moves");
        }

        Console.WriteLine($"Total complexity keypad movements is {totalComplexity}");
    }

    public void Part2()
    {
        throw new NotImplementedException();
    }

    private List<Vector> InitializeNumberSequence(string sequence)
    {
        var visited = sequence.Select(s => _numPad[s]).Prepend(_numPad['A']);
        var vectorDeltas = visited.Zip(visited.Skip(1), (target, src) => Vector.Delta(src, target))
                                  .SelectMany(x => new[] { x, Vector.Zero });
        return vectorDeltas.ToList();
    }
}