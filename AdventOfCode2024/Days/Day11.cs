using System.Runtime.InteropServices;

namespace AdventOfCode2024.Days;

public class Day11 : IDay
{
    private struct StonedBlinks(long stone, int blink)
    {
        public long StoneValue { get; set; } = stone;
        public int BlinkCount { get; set; } = blink;
    }

    private string _example = @"125 17";

    private readonly List<string> _input = [];
    private readonly Dictionary<StonedBlinks, long> _cache = [];

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
        var result = _input.Select(x => Blink(long.Parse(x), 25)).Sum();
        Console.WriteLine($"The stone line is {result} long after 25 blinks!");
    }

    public void Part2()
    {
        var blinks = 75;
        var result = Blink182(_input.Select(long.Parse).ToArray(), blinks);
        Console.WriteLine($"The stone line is {result} long after {blinks} blinks!");
    }

    private long Blink(long stone, int blinks)
    {
        static bool EvenDigits(long inVar, out long outVar)
        {
            var value = EvenNumberLengths(inVar);
            outVar = value / 2;
            return value != 1;
        }

        if (blinks == 0)
            return 1L;
        var nextBlink = blinks - 1;

        if (_cache.TryGetValue(new StonedBlinks(stone, nextBlink), out var existing)) return existing;
        var result = stone switch
        {
            0 => Blink(1L, nextBlink),
            var x when EvenDigits(x, out var digits) => Blink(stone / TenPowers(digits), nextBlink)
                                   + Blink(stone % TenPowers(digits), nextBlink),
            _ => Blink(stone * 2024, nextBlink),
        };

        CollectionsMarshal.GetValueRefOrAddDefault(_cache, new StonedBlinks(stone, nextBlink), out bool _) = result;
        return result;
    }

    private static long Blink182(long[] stones, int blinks)
    {
        var seededStones = stones.ToDictionary(x => x, x => 1L);

        for (var i = 0; i < blinks; i++)
        {
            AllTheSmallThings(seededStones);
        }

        return seededStones.Sum(x => x.Value);
    }

    private static void AllTheSmallThings(Dictionary<long, long> stones)
    {
        static bool EvenDigits(long inVar, out long outVar)
        {
            var value = EvenNumberLengths(inVar);
            outVar = value / 2;
            return value != 1;
        }

        long SplitEven(long value, long digits, long count)
        {
            var left = value / TenPowers(digits);
            var right = value % TenPowers(digits);
            CollectionsMarshal.GetValueRefOrAddDefault(stones, left, out var _) += count;
            CollectionsMarshal.GetValueRefOrAddDefault(stones, right, out var _) += count;
            return count;
        }

        var stoneArray = stones.ToArray();
        stones.Clear();

        foreach (var (value, count) in stoneArray)
        {
            var _ = value switch
            {
                0 => CollectionsMarshal.GetValueRefOrAddDefault(stones, 1L, out var _) += count,
                var x when EvenDigits(x, out var digits) => SplitEven(x, digits, count),
                _ => CollectionsMarshal.GetValueRefOrAddDefault(stones, value * 2024, out var _) += count,
            };
        }
    }

    private static long TenPowers(long power) => power switch
    {
        0 => 1,
        1 => 10,
        2 => 100,
        3 => 1000,
        4 => 10000,
        5 => 100000,
        6 => 1000000,
        _ => (long)Math.Pow(10, power)
    };

    private static long EvenNumberLengths(long number) => number switch
    {
        >= 10 and <= 99 => 2,
        >= 1000 and <= 9999 => 4,
        >= 100000 and <= 999999 => 6,
        >= 10000000 and <= 99999999 => 8,
        >= 1000000000 and <= 9999999999 => 10,
        >= 100000000000 and <= 999999999999 => 12,
        >= 10000000000000 and <= 99999999999999 => 14,
        _ => 1
    };
}