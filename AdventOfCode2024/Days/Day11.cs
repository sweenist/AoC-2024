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
        var result = Blink(stones, 10);

        Console.WriteLine($"The stone line is {result.Count} long after 25 blinks!");
    }

    public void Part2()
    {
        var stoneValues = new Dictionary<long, long>();
        var stones = _input.Select(long.Parse).ToList();
        var result = Blink2(stones, 10);
        Console.WriteLine($"The stone line is {result} long after 75 blinks!");
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

    private static long Blink2(List<long> stones, int blinks)
    {
        var stoneValues = new Dictionary<long, long>();
        while (blinks > 0)
        {
            --blinks;
            foreach (var stone in stones.Blink().ToList())
            {
                switch (stone)
                {
                    case 0:
                        if (!stoneValues.TryAdd(1L, 1L)) stoneValues[1L]++;
                        break;
                    case var x when x.ToString().Length % 2 == 0:
                        var word = x.ToString();
                        var key1 = long.Parse(word[..(word.Length / 2)]);
                        var key2 = long.Parse(word[(word.Length / 2)..]);
                        if (!stoneValues.TryAdd(key1, 1L)) stoneValues[key1]++;
                        if (!stoneValues.TryAdd(key2, 1L)) stoneValues[key2]++;
                        break;
                    default:
                        var key = stone * 2024;
                        if (!stoneValues.TryAdd(key, 1L)) stoneValues[key]++;
                        break;
                }
            }
            // Console.WriteLine($"Stones: {string.Join(' ', stones)} Length: {stones.Count}; Zeroes: {stones.Where(x => x == 0).Count()}");
        }
        // Console.WriteLine($"Hashing: {string.Join(',', stoneValues.Select(d => $"{d.Key}: {d.Value}"))}");
        return stoneValues.Values.Sum();
    }
}