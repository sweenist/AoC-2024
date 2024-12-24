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
            InitializeNumberSequence(sequence);
            // Robots recursively managing movements from vectors

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
        return visited.Zip(visited.Skip(1), (src, target) => Vector.Delta(src, target)).ToList();
    }
}