using AdventOfCode2024.Utility;

namespace AdventOfCode2024.Days;

public class Day19 : IDay
{
    private string _example = @"r, wr, b, g, bwu, rb, gb, br

brwrr
bggr
gbbr
rrbgbr
ubwu
bwurrg
brgr
bbrgwb";

    private Dictionary<string, int> _matchedPatterns = [];

    private readonly List<string> _input = [];

    public Day19(bool useExample = false)
    {
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            _input.AddRange(_example.Split(Environment.NewLine));
        }
        else
        {
            var inputFile = $"inputData/{GetType().Name}.txt";
            using var sr = new StreamReader(inputFile);
            _input.AddRange(sr.ReadToEnd().Split(Environment.NewLine));
        }
    }

    public void Part1()
    {
        var (available, desired) = ParseTowels();

        var result = desired.Count(t => Combinate(t, available) != 0);

        Console.WriteLine($"Only {result} available combinations of towels available");
    }

    public void Part2()
    {
        var (available, desired) = ParseTowels();

        var result = desired.Sum(t => Combinate(t, available));

        Console.WriteLine($"{result} total available combinations of towels available");
    }

    private (List<string> available, List<string> desired) ParseTowels()
    {
        var available = _input[0].Split(", ").ToList();
        var desired = _input[2..].ToList();
        return (available, desired);
    }

    private int Combinate(string wanted, List<string> towels)
    {
        if (_matchedPatterns.TryGetValue(wanted, out var patternCount)) return patternCount;
        foreach (var towel in towels)
        {
            if (wanted == string.Empty) return 1;
            if (wanted.StartsWith(towel))
                patternCount += Combinate(wanted[towel.Length..], towels);
        }
        _matchedPatterns[wanted] = patternCount;
        return patternCount;
    }
}