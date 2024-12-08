using AdventOfCode2024.Utility;

namespace AdventOfCode2024.Days;

public class Day7 : IDay
{
    private string _example = @"190: 10 19
3267: 81 40 27
83: 17 5
156: 15 6
7290: 6 8 6 15
161011: 16 10 13
192: 17 8 14
21037: 9 7 18 13
292: 11 6 16 20";

    private readonly List<string> _input = [];
    private readonly Dictionary<long, IEnumerable<long>> _calibrations;

    public Day7(bool useExample = false)
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
        _calibrations = ParseCalibrations();
    }

    public void Part1()
    {
        var sum = 0L;
        foreach (var kvp in _calibrations)
        {
            var sumProducts = kvp.Value.SumProductTree();
            if (sumProducts.Contains(kvp.Key)) sum += kvp.Key;
        }

        Console.WriteLine($"Sum of all calibration tests are {sum}");
    }

    public void Part2()
    {
        var sum = 0L;
        foreach (var kvp in _calibrations)
        {
            var combinations = kvp.Value.SumProductConcatenate().ToList();
            if (!combinations.Contains(kvp.Key))
                continue;
            sum += kvp.Key;
        }

        Console.WriteLine($"Sum of all calibration tests with concatenation are {sum}");
    }

    private Dictionary<long, IEnumerable<long>> ParseCalibrations()
    {
        return _input.Select(x =>
        {
            var kvp = x.Split(':');
            return new KeyValuePair<long, IEnumerable<long>>(
                key: long.Parse(kvp[0]),
                value: kvp[1].Trim().Split(' ').Select(long.Parse));
        }).ToDictionary();
    }
}
