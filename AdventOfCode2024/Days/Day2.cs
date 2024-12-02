using Microsoft.Win32.SafeHandles;

namespace AdventOfCode2024.Days;

public class Day2 : IDay
{
    private readonly bool _useExample;
    private string _example = @"7 6 4 2 1
1 2 7 8 9
9 7 6 2 1
1 3 2 4 5
8 6 4 4 1
1 3 6 7 9";

    private readonly List<string> _input = [];

    public Day2(bool useExample = false)
    {
        _useExample = useExample;
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            _input.AddRange(_example.Split('\n'));
            return;
        }

        var inputFile = $"inputData/{GetType().Name}.txt";
        using var sr = new StreamReader(inputFile);
        while (!sr.EndOfStream)
        {
            _input.Add(sr.ReadLine() ?? throw new ArgumentNullException("item", message: "Read null value from stream"));
        }
    }

    public void Part1()
    {
        var safeReports = 0;
        foreach (var report in _input)
        {
            var assessment = report.Split().Select(x => int.Parse(x)).ToList();
            var left = assessment.Take(assessment.Count - 1);
            var right = assessment.Skip(1);
            var allIncreasing = left.Zip(right, (a, b) => a < b && b - a >= 1 && b - a <= 3).All(x => x == true);
            var allDecreasing = left.Zip(right, (a, b) => a > b && a - b >= 1 && a - b <= 3).All(x => x == true);

            safeReports += allIncreasing || allDecreasing ? 1 : 0;
        }
        Console.WriteLine($"There are {safeReports} safe report(s)");
    }

    public void Part2()
    {
        throw new NotImplementedException();
    }
}