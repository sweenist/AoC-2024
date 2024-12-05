using System.Numerics;

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
        var offsets = new Dictionary<string, Vector2>{
            {"LEFT", new Vector2(1,0) },
            {"RIGHT", new Vector2(-1,0) },
            {"DOWN", new Vector2(0,1) },
            {"UP", new Vector2(0,-1) },
            {"LEFTDOWN", new Vector2(1,1) },
            {"LEFTUP", new Vector2(1,-1) },
            {"RIGHTDOWN", new Vector2(-1,1) },
            {"RIGHTUP", new Vector2(-1,-1) },
        };

        var totalFound = 0;
        var grid = new Grid
        {
            Height = _input.Count,
            Width = _input[0].Length,
        };


        void Traverse(Vector2 current, int wordIndex, string trajectory = "")
        {
            var keys = GetKeys([.. offsets.Keys], current, grid);
            var foundKeys = keys.Where(k => (k == trajectory || trajectory == string.Empty)
                                        && GetLetter(offsets[k] + current) == WORD[wordIndex])
                                .ToList();

            if (foundKeys.Count == 0)
                return;

            foreach (var key in foundKeys)
            {
                if (wordIndex == WORD.Length - 1)
                {
                    totalFound++;
                    continue;
                }

                var nextPosition = offsets[key] + current;
                Traverse(nextPosition, wordIndex + 1, key);
            }
        }

        for (var y = 0; y < grid.Width; y++)
            for (var x = 0; x < grid.Height; x++)
            {
                var currentPosition = new Vector2(x, y);
                if (GetLetter(currentPosition) != WORD[0])
                    continue;
                Traverse(currentPosition, 1);
            }

        Console.WriteLine($"Found {totalFound} words in puzzle");
    }

    public void Part2()
    {
        var totalXmas = 0;
        var offsets = new List<Vector2>{
            new(-1,-1), //top-left
            new(1,-1),  //top-right
            new(-1,1),  //bottom-left
            new(1,1),   //bottom-right
        };

        var grid = new Grid
        {
            Height = _input.Count,
            Width = _input[0].Length,
        };

        void Xtreme(Vector2 current)
        {
            var letters = offsets.Select(x => GetLetter(x + current)).ToList();
            if (letters.Count(x => x == 'M') != 2
                || letters.Count(x => x == 'S') != 2
                || letters.First().Equals(letters.Last()))
                return;

            totalXmas++;
        }

        for (var y = 1; y < grid.BoundY; y++)
            for (var x = 1; x < grid.BoundX; x++)
            {
                var currentPosition = new Vector2(x, y);
                if (GetLetter(currentPosition) == 'A')
                    Xtreme(currentPosition);
            }
        Console.WriteLine($"Found a total of {totalXmas} X-MASes");
    }

    private static List<string> GetKeys(List<string> keys, Vector2 pos, Grid grid)
    {
        if (pos.X == 0) keys = keys.Where(k => !k.StartsWith("RIGHT")).ToList();
        if (pos.Y == 0) keys = keys.Where(k => !k.EndsWith("UP")).ToList();
        if (pos.X == grid.BoundX) keys = keys.Where(k => !k.StartsWith("LEFT")).ToList();
        if (pos.Y == grid.BoundY) keys = keys.Where(k => !k.EndsWith("DOWN")).ToList();
        return keys;
    }

    private char GetLetter(Vector2 pos)
    {
        return _input[(int)pos.Y][(int)pos.X];
    }

    private record Grid
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int BoundX => Width - 1;
        public int BoundY => Height - 1;
    }
}
