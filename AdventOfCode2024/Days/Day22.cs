namespace AdventOfCode2024.Days;

public class Day22 : IDay
{
    private string _example = @"1
2
3
2024";

    private readonly List<string> _input = [];

    public Day22(bool useExample = false)
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
    }

    public void Part1()
    {
        var result = _input.Select(x => NextSequence(long.Parse(x), 2000)).Sum();
        Console.WriteLine($"The result of 2000 secret number iterations is {result}");
    }

    public void Part2()
    {
        var results = _input.Select(long.Parse)
                            .Select(x => new KeyValuePair<long, List<short>>(x, Sequencify(x, 1999)))
                            .ToDictionary();

        var bananaMarket = new Dictionary<string, int>();
        foreach (var initial in results.Keys)
        {
            var bananaDeltas = Bananafy(results, initial);
            foreach (var key in bananaDeltas.Keys)
                bananaMarket[key] = bananaMarket.GetValueOrDefault(key) + bananaDeltas[key];
        }
        Console.WriteLine($"The most bananas one can get are {bananaMarket.Values.Max()} ");
    }

    private static Dictionary<string, int> Bananafy(Dictionary<long, List<short>> results, long initial)
    {
        var bananaDifferences = new Dictionary<string, int>();
        var ones = results[initial];
        var diffs = ones.Zip(ones.Skip(1), (f, s) => s - f).ToArray();
        for (var i = 4; i < diffs.Length; i++)
        {
            var key = string.Join(',', diffs[(i - 4)..i]);
            if (!bananaDifferences.ContainsKey(key))
                bananaDifferences.Add(key, ones[i]);
        }
        return bananaDifferences;
    }

    private static long NextSequence(long secret, int iterations)
    {
        const long prune = 16777216L;

        for (var i = 0; i < iterations; i++)
        {
            var first = ((secret * 64) ^ secret) % prune;
            var second = ((first / 32) ^ first) % prune;
            secret = ((second * 2048) ^ second) % prune;
        }
        return secret;
    }

    private static List<short> Sequencify(long secret, int iterations)
    {
        const long prune = 16777216L;
        var ones = new List<short> { (short)(secret % 10) };

        for (var i = 0; i < iterations; i++)
        {
            var first = ((secret * 64) ^ secret) % prune;
            var second = ((first / 32) ^ first) % prune;
            secret = ((second * 2048) ^ second) % prune;
            ones.Add((short)(secret % 10));
        }

        return ones;
    }
}