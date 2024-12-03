using System.Text.RegularExpressions;

namespace AdventOfCode2024.Days;

public class Day3 : IDay
{
    private readonly string _example = @"xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))";

    private readonly List<string> _input = [];

    public Day3(bool useExample = false)
    {
        if (useExample)
        {
            _input.Add(_example);
            return;
        }
        var inputFile = $"inputData/{GetType().Name}.txt";
        using var sr = new StreamReader(inputFile);

        _input.AddRange(sr.ReadToEnd().Split('\n'));
    }

    public void Part1()
    {
        var regexPattern = @"mul\((\d{1,3}),(\d{1,3})\)";
        var sumOfValues = 0L;

        foreach (var line in _input)
        {
            var matches = Regex.Matches(line, regexPattern);
            sumOfValues += matches.Select(x => int.Parse(x.Groups[1].Value) * int.Parse(x.Groups[2].Value)).Sum();
        }
        Console.WriteLine($"The sum of all values is {sumOfValues}");
    }

    public void Part2()
    {
        var mulPattern = @"mul\((\d{1,3}),(\d{1,3})\)";
        var sumOfValues = 0L;
        var processedInstructions = string.Join("", _input)
                                          .Split("don't()")
                                          .SelectMany((x, i) => i == 0
                                            ? [x]
                                            : x.Split("do()", 2).Skip(1));

        foreach (var line in processedInstructions)
        {
            var matches = Regex.Matches(line, mulPattern);
            sumOfValues += matches.Select(x => int.Parse(x.Groups[1].Value) * int.Parse(x.Groups[2].Value)).Sum();
        }
        Console.WriteLine($"The sum of all do() values is {sumOfValues}");
    }
}
