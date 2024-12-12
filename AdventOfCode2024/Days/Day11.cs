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
        var result = _input.Select(x => Blink(long.Parse(x), blinks)).Sum();
        Console.WriteLine($"The stone line is {result} long after {blinks} blinks!");
    }

    private long Blink(long stone, int blinks)
    {
        bool EvenDigits(long inVar, out long outVar)
        {
            outVar = (long)Math.Floor(Math.Log10(inVar)) + 1;
            return outVar % 2 == 0;
        }
        if (blinks == 0)
            return 1L;
        var nextBlink = blinks - 1;
        if (_cache.TryGetValue(new StonedBlinks(stone, nextBlink), out var existing)) return existing;
        long result;
        switch (stone)
        {
            case 0:
                result = Blink(1L, nextBlink);
                break;
            case var x when EvenDigits(x, out var digits):
                result = Blink(stone / (long)Math.Pow(10, digits), nextBlink)
                + Blink(stone % (long)Math.Pow(10, digits), nextBlink);
                break;
            default:
                result = Blink(stone * 2024, nextBlink);
                break;
        }
        _cache[new StonedBlinks(stone, nextBlink)] = result;
        return result;
    }
}