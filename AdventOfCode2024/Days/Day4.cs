using System.Text.RegularExpressions;

namespace AdventOfCode2024.Days;

public class Day4 : IDay
{
    private string _example = @"MMMSXXMASM
MSAMXMSMSA
AMXSXMAAMM
MSAMASMSMX
XMASAMXAMM
XXAMMXXAMA
SMSMSASXSS
SAXAMASAAA
MAMMMXMMMM
MXMXAXMASX";

    private readonly List<string> _input = [];

    public Day4(bool useExample = false)
    {
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            _input.AddRange(_example.Split('\n'));
            return;
        }
        var inputFile = $"inputData/{GetType().Name}.txt";
        using var sr = new StreamReader(inputFile);
        _input.AddRange(sr.ReadToEnd().Split('\n'));
    }

    public void Part1()
    {
        const string searchPattern = "XMAS";
        const string reversePattern = "SAMX";

        var totalFound = 0;

        foreach (var line in _input)
        {
            var matches = Regex.Matches(line, searchPattern);
            var sehctam = Regex.Matches(line, reversePattern);
            totalFound += matches.Count + sehctam.Count;
        }
        var pivot = PivotInput(true);
        foreach (var line in pivot)
        {
            var matches = Regex.Matches(line, searchPattern);
            var sehctam = Regex.Matches(line, reversePattern);
            totalFound += matches.Count + sehctam.Count;
        }

        Console.WriteLine($"Found {totalFound} words in puzzle");
    }

    public void Part2()
    {
        throw new NotImplementedException();
    }

    public List<string> PivotInput(bool debug = false)
    {
        var orthagonal = new List<string>();
        for (var i = 0; i < _input[0].Length; i++)
        {
            orthagonal.Add(string.Join("", _input.Select(x => x[i])));
        }
        if (debug)
        {
            Console.WriteLine("---Original---");
            foreach (var line in _input)
            {
                Console.WriteLine(string.Join(' ', line.ToCharArray()));
            }

            Console.WriteLine("---Pivot---");
            foreach (var line in orthagonal)
            {
                Console.WriteLine(string.Join(' ', line.ToCharArray()));
            }
        }
        return orthagonal;
    }
}
