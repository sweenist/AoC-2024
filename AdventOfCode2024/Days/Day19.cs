using System.Globalization;
using System.Runtime.InteropServices;
using AdventOfCode2024.Utility;

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

    private Dictionary<string, List<List<string>>> Combinate(List<string> available, List<string> desired)
    {
        var results = new Dictionary<string, List<List<string>>>();

        foreach (var want in desired)
        {
            var chunks = available.Where(want.Contains).OrderByDescending(t => t.Length).ToList();
            var combinations = GetCombinations(want, chunks);
            foreach (var combo in combinations)
            {
                if (want.Equals(string.Join("", combo)))
                {
                    if (results.ContainsKey(want))
                        results[want].Add(combo);
                    else
                        results[want] = [combo];

                    Console.WriteLine($"combo: {string.Join(',', combo)}");
                }
            }
        }
        return results;
    }

    private List<List<string>> GetCombinations(string word, List<string> towels)
    {
        var matches = CascadeTowels(word, towels).ToList();
        var returnList = matches.Split(x => x.Equals(","));

        return returnList.ToList();
    }

    private static IEnumerable<string> CascadeTowels(string wordSegment, List<string> towels)
    {
        if (wordSegment == string.Empty)
        {
            yield return ",";
            yield break;
        }

        var towelSubset = towels.Where(wordSegment.Contains).OrderByDescending(t => t.Length).ToList();
        if (towelSubset.Count == 0)
            yield break;
        // Console.WriteLine($"{wordSegment}: {string.Join(',', towelSubset)}");

        foreach (var towel in towelSubset)
        {
            if (wordSegment.StartsWith(towel))
            {
                yield return towel;
                foreach (var segment in CascadeTowels(wordSegment[towel.Length..], towelSubset).ToList())
                    yield return segment;
            }
        }
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

                for (var w = i + 1; w <= want.Length; w++)
                {
                    if (!wordBreaks[w])
                        wordBreaks[w] = TowelContains(want.Substring(i, w - i), chunks);

                    if (w == want.Length && wordBreaks[w])
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