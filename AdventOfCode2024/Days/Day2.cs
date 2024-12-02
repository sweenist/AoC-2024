using AdventOfCode2024.Utility;
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
            var (assessment, increasingPower, decreasingPower) = ParseReport(report.Split().Select(x => int.Parse(x)).ToList());

            var allIncreasing = increasingPower.All(x => x == true);
            var allDecreasing = decreasingPower.All(x => x == true);

            safeReports += allIncreasing || allDecreasing ? 1 : 0;
        }
        Console.WriteLine($"There are {safeReports} safe report(s)");
    }

    public void Part2()
    {
        var safeReports = 0;
        foreach (var report in _input)
        {
            var (assessment, increasingPower, decreasingPower) = ParseReport(report.Split().Select(x => int.Parse(x)).ToList());

            var allIncreasing = increasingPower.All(x => x == true);
            var allDecreasing = decreasingPower.All(x => x == true);

            if (allIncreasing || allDecreasing)
            {
                safeReports++;
                continue;
            }

            var increasingPowerFaults = increasingPower.Count(x => x == false);
            var decreasingPowerFaults = decreasingPower.Count(x => x == false);

            switch (increasingPowerFaults)
            {
                case 2:
                    if (increasingPower.HasAnyAdjacent(false))
                    {
                        assessment.RemoveAt(increasingPower.IndexOf(false) + 1);
                        var (_, inc, _) = ParseReport(assessment);
                        allIncreasing = inc.All(x => x == true);
                    }
                    break;
                case 1:
                    // take up to first index, skip, take rest;
                    //take up to second index, skip take rest;
                    break;
                default: break;
            }

            if (increasingPower.Count(x => x == false) <= 2)
            {
                if (increasingPower.Count(x => x == false) == 1 && increasingPower[0] == false)
                {
                    Console.WriteLine($"Increasing: First object was false in {string.Join(',', increasingPower)}");
                    safeReports++;
                    continue;
                }
                var index = increasingPower.IndexOf(false) + 1;
                if (index == increasingPower.Count || !increasingPower[index])
                {
                    Console.WriteLine($"Removing from {string.Join(',', assessment)} at index {index}");
                    Console.WriteLine($"\t{string.Join("\n\t", increasingPower)}");
                    assessment.RemoveAt(index);

                    var (_, increasingPowerDampened, _) = ParseReport(assessment);
                    Console.WriteLine($"\nWith Dampening{string.Join("\n\t", increasingPowerDampened)}");
                    allIncreasing = increasingPowerDampened.All(x => x == true);
                    safeReports += allIncreasing ? 1 : 0;
                    continue;
                }
            }

            if (decreasingPower.Count(x => x == false) <= 2)
            {
                if (decreasingPower.Count(x => x == false) == 1 && decreasingPower[0] == false)
                {
                    Console.WriteLine($"Decreasing: First object was false in {string.Join(',', decreasingPower)}");
                    safeReports++;
                    continue;
                }
                var index = decreasingPower.IndexOf(false) + 1;
                if (index == decreasingPower.Count || !decreasingPower[index])
                {
                    Console.WriteLine($"Removing from {string.Join(',', assessment)} at index {index}");
                    Console.WriteLine($"\t{string.Join("\n\t", decreasingPower)}");
                    assessment.RemoveAt(index);

                    var (_, _, decreasingPowerDampened) = ParseReport(assessment);
                    allDecreasing = decreasingPowerDampened.All(x => x == true);

                    safeReports += allDecreasing ? 1 : 0;
                }
            }

        }
        Console.WriteLine($"There are {safeReports} safe report(s) when dampening is applied");
    }

    private (List<int> assessment, List<bool> increasingPower, List<bool> decreasingPower) ParseReport(List<int> report)
    {
        var assessment = report;
        var left = assessment.Take(assessment.Count - 1);
        var right = assessment.Skip(1);
        var increasingPower = left.Zip(right, (a, b) => a < b && b - a >= 1 && b - a <= 3).ToList();
        var decreasingPower = left.Zip(right, (a, b) => a > b && a - b >= 1 && a - b <= 3).ToList();

        return (assessment!, increasingPower, decreasingPower);
    }
}