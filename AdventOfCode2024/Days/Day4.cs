using AdventOfCode2024.Types;
using AdventOfCode2024.Utility.Math;

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
    private readonly Boundary _bounds;
    private List<Vector> _paraVectors = [];

    public Day4(bool useExample = false)
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
        _bounds = new Boundary(_input.Count, _input[0].Length);
    }

    public void Part1()
    {
        _paraVectors = Vector.OmniDirections;
        const string WORD = "XMAS";
        var totalFound = 0;

        void Traverse(Point current, int wordIndex, Vector? trajectory = null)
        {
            var adjacentDirections = GetSurrounding(current);
            var eligibleVectors = adjacentDirections.Where(k => (trajectory is null || k.Equals(trajectory.Value))
                                        && GetLetter(current + k) == WORD[wordIndex])
                                .ToList();

            if (eligibleVectors.Count == 0)
                return;

            foreach (var vector in eligibleVectors)
            {
                if (wordIndex == WORD.Length - 1)
                {
                    totalFound++;
                    continue;
                }

                var nextPosition = current + vector;
                Traverse(nextPosition, wordIndex + 1, vector);
            }
        }

        for (var y = 0; y < _bounds.Width; y++)
            for (var x = 0; x < _bounds.Height; x++)
            {
                var currentPosition = new Point(x, y);
                if (GetLetter(currentPosition) != WORD[0])
                    continue;
                Traverse(currentPosition, 1);
            }

        Console.WriteLine($"Found {totalFound} words in puzzle");
    }

    public void Part2()
    {
        var totalXmas = 0;
        _paraVectors = Vector.OrdinalPoints;

        for (var y = 1; y < _bounds.BoundY; y++)
            for (var x = 1; x < _bounds.BoundX; x++)
            {
                var currentPosition = new Point(x, y);
                if (GetLetter(currentPosition) == 'A')
                    totalXmas += Xtreme(currentPosition);
            }
        Console.WriteLine($"Found a total of {totalXmas} X-MASes");
    }

    private List<Vector> GetSurrounding(Point currentPosition)
    {
        var validVectors = _paraVectors.ToList();
        if (currentPosition.X == 0) validVectors = validVectors.Where(v => v.X != -1).ToList();
        if (currentPosition.Y == 0) validVectors = validVectors.Where(v => v.Y != -1).ToList();
        if (currentPosition.X == _bounds.BoundX) validVectors = validVectors.Where(v => v.X != 1).ToList();
        if (currentPosition.Y == _bounds.BoundY) validVectors = validVectors.Where(v => v.Y != 1).ToList();

        return validVectors;
    }

    private int Xtreme(Point current)
    {
        var letters = _paraVectors.Select(x => GetLetter(current + x)).ToList();
        return (letters.Count(x => x == 'M') != 2
                || letters.Count(x => x == 'S') != 2
                || letters.First().Equals(letters.Last()))
            ? 0
            : 1;
    }

    private char GetLetter(Point pos)
    {
        return _input[pos.Y][pos.X];
    }
}
