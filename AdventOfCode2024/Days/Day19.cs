using System.Globalization;

namespace AdventOfCode2024.Days;

public class Day19 : IDay
{
    private string _example = @"r, wr, b, g, bwu, rb, gb, br

brwrr
bggr
gbbr
rrbgbr
ubwu
bwurrg
brgr
bbrgwb";

    private readonly List<string> _input = [];

    public Day19(bool useExample = false)
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
        var (available, desired) = ParseTowels();
        var result = Combinate(available, desired);
        Console.WriteLine($"combos: {string.Join('\n', result.Keys)}");

        Console.WriteLine($"Only {result.Count} available combinations of towels available");
    }

    public void Part2()
    {
        throw new NotImplementedException();
    }

    private (List<string> available, List<string> desired) ParseTowels()
    {
        var available = _input[0].Split(", ").OrderBy(x => x.Length).ToList();
        var desired = _input[2..].ToList();
        return (available, desired);
    }

    private Dictionary<string, List<string>> Combinate(List<string> available, List<string> desired)
    {
        var results = new Dictionary<string, List<string>>();
        // var maxChunk = desired.Max(x => x.Length);

        // var desiredSegments = SegmentTowel(desired[0]).ToList();
        // Console.WriteLine(string.Join(':', desiredSegments));

        foreach (var want in desired)
        {
            var chunks = available.Where(want.Contains).OrderByDescending(t => t.Length).ToList();
            Console.WriteLine($"Possible segments: {string.Join(',', chunks)}");
            var canSpell = TrySpell(want, chunks);
            if (canSpell)
                results[want] = chunks;
        }
        return results;
    }

    private static bool TrySpell(string want, List<string> chunks)
    {
        if (want == string.Empty) return true;

        var wordBreaks = new bool[want.Length + 1];
        for (var i = 0; i < want.Length; i++)
        {
            if (!wordBreaks[i])
                wordBreaks[i] = TowelContains(want[..i], chunks);

            if (wordBreaks[i])
            {
                if (i == want.Length) return true;

                for (var w = i + 1; w < want.Length; w++)
                {
                    if (!wordBreaks[w])
                        wordBreaks[w] = TowelContains(want.Substring(i, w - i), chunks);

                    if (w == want.Length && wordBreaks[w]) ;
                    return true;
                }
            }

        }
        return false;
    }

    private static bool TowelContains(string towel, List<string> towels)
    {
        foreach (var segment in towels)
        {
            if (string.CompareOrdinal(segment, towel) == 0)
                return true;
        }
        return false;
    }

    private IEnumerable<string> SegmentTowel(string towel)
    {
        if (towel.Length <= 1)
        {
            Console.WriteLine($"1: {towel}");
            yield return towel;
            yield break;
        }
        var c = towel[0];
        foreach (var rest in SegmentTowel(towel.Remove(0, 1)))
        {
            Console.WriteLine($"2: {c + rest}; {c} {rest}");
            yield return c + rest;
            yield return c + " " + rest;
        }
    }

}