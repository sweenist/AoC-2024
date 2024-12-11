using AdventOfCode2024.Utility;

namespace AdventOfCode2024.Days;

public class Day11 : IDay
{
    private string _example = @"125 17";

    private readonly List<string> _input = [];

    public Day11(bool useExample = false)
    {
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            _input.AddRange(_example.Split(' '));
        }
        else
        {
            var inputFile = $"inputData/{GetType().Name}.txt";
            using var sr = new StreamReader(inputFile);
            _input.AddRange(sr.ReadToEnd().Split(' '));
        }
    }

    public void Part1()
    {
        var stones = _input.Select(long.Parse).ToList();
        var result = Blink(stones, 25);

        Console.WriteLine($"The stone line is {result.Count} long after 25 blinks!");
    }

    public void Part2()
    {
        var accum = 0L;
        var stones = _input.Select(long.Parse).ToList();
        // var result = Chunk(stones, 75).Select(c => c.Count).Sum();
        var result = Blink(stones, 40);
        foreach (var stone in result)
        {
            try
            {
                accum += Chunk(stone, 35);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"stone: {stone}; encountered {ex.Message}");
            }
        }
        Console.WriteLine($"The stone line is {accum} long after 75 blinks!");
    }

    private static List<long> Blink(List<long> stones, int blinks)
    {
        while (blinks > 0)
        {
            --blinks;
            stones = stones.Blink().ToList();

            // Console.WriteLine($"Stones: {string.Join(' ', stones)} Length: {stones.Count}; Zeroes: {stones.Where(x => x == 0).Count()}");
        }
        return stones;
    }

    private static long Chunk(long stone, int blinks)
    {
        var chunks = new[] { new[] { stone }.ToList() }.ToList();
        while (blinks > 0)
        {
            --blinks;
            for (var i = 0; i < chunks.Count; i++)
            {
                chunks[i] = chunks[i].Blink().ToList();
            }
            if (blinks % 10 == 0)
            {
                chunks = chunks.SelectMany(x => new[] { x.Take(x.Count / 2).ToList(), x.Skip(x.Count / 2).ToList() }.ToList()).ToList();
            }

            // Console.WriteLine($"Stones: {string.Join(' ', stones)}");
        }
        return chunks.Select(c => c.Count).Sum();
    }
}