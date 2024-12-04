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
        const string WORD = "XMAS";
        var offsets = new Dictionary<string, int[]>{
            {"LEFT", [1,0] },
            {"RIGHT", [-1,0] },
            {"DOWN", [0,1] },
            {"UP", [0,-1] },
            {"LEFTDOWN", [1,1] },
            {"LEFTUP", [1,-1] },
            {"RIGHTDOWN", [-1,1] },
            {"RIGHTUP", [-1,-1] },
        };

        var totalFound = 0;
        var anchors = new List<(int x, int y)>();
        var maxX = _input[0].Length;
        var maxY = _input.Count;
        var boundX = maxX - 1;
        var boundY = maxY - 1;

        for (var y = 0; y < maxY; y++)
            for (var x = 0; x < maxX; x++)
            {
                var keys = GetKeys(offsets.Keys.ToList(), x, y);
                
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

    private List<string> GetKeys(List<string> keys, int x, int y)
    {
        if(x==0) keys = keys.Where(k => !k.StartsWith("RIGHT")).ToList();
        if(y==0) keys = keys.Where(k => !k.EndsWith("UP")).ToList();
    }
}
