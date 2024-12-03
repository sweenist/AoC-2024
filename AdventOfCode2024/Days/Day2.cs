namespace AdventOfCode2024.Days;

public class Day2 : IDay
{
    private string _example = @"7 6 4 2 1
1 2 7 8 9
9 7 6 2 1
1 3 2 4 5
8 6 4 4 1
1 3 6 7 9";

    private readonly List<string> _input = [];

    public Day2(bool useExample = false)
    {
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
            var (increasingPower, decreasingPower) = ParseReport(assessment);

            var allIncreasing = increasingPower.All(x => x);
            var allDecreasing = decreasingPower.All(x => x);

            safeReports += allIncreasing || allDecreasing ? 1 : 0;
        }
        Console.WriteLine($"There are {safeReports} safe report(s)");
    }

    public void Part2()
    {
        var safeReports = 0;
        foreach (var report in _input)
        {
            var assessment = report.Split().Select(x => int.Parse(x)).ToList();
            var (increasingPower, decreasingPower) = ParseReport(assessment);

            var allIncreasing = increasingPower.All(x => x == true);
            var allDecreasing = decreasingPower.All(x => x == true);

            if (allIncreasing || allDecreasing)
            {
                safeReports++;
                continue;
            }

            //Handle Dampening
            var increasingIndex = increasingPower.IndexOf(false);
            var decreasingIndex = decreasingPower.IndexOf(false);
            var increasingFixedByDampening = IsFixedByDampening(assessment, increasingIndex, useOffset: false, shouldDecrease: false)
                || IsFixedByDampening(assessment, increasingIndex, useOffset: true, shouldDecrease: false);
            var decreasingFixedByDampening = IsFixedByDampening(assessment, decreasingIndex, useOffset: false, shouldDecrease: true)
                || IsFixedByDampening(assessment, decreasingIndex, useOffset: true, shouldDecrease: true);

            if (increasingFixedByDampening || decreasingFixedByDampening)
            {
                safeReports++;
                continue;
            }
        }
        Console.WriteLine($"There are {safeReports} safe report(s) when dampening is applied");
    }

    private static (List<bool> increasingPower, List<bool> decreasingPower) ParseReport(List<int> report)
    {
        var left = report.Take(report.Count - 1);
        var right = report.Skip(1);
        var increasingPower = left.Zip(right, (a, b) => a < b && b - a >= 1 && b - a <= 3).ToList();
        var decreasingPower = left.Zip(right, (a, b) => a > b && a - b >= 1 && a - b <= 3).ToList();

        return (increasingPower, decreasingPower);
    }

    private static bool IsFixedByDampening(IList<int> source, int index, bool useOffset, bool shouldDecrease)
    {
        var maxIndex = source.Count - 1;
        var offset = useOffset ? 1 : 0;

        var dampenedValues = new List<int>(source);
        dampenedValues.RemoveAt(Math.Min(index + offset, maxIndex));

        var (increase, decrease) = ParseReport(dampenedValues);

        return shouldDecrease
            ? decrease.All(x => x)
            : increase.All(x => x);
    }
}