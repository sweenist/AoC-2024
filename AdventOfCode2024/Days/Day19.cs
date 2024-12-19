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
            _input.AddRange(_example.Split(Environment.NewLine));
        }
        else
        {
            var inputFile = $"inputData/{GetType().Name}.txt";
            using var sr = new StreamReader(inputFile);
            _input.AddRange(sr.ReadToEnd().Split(Environment.NewLine));
        }
    }

    public void Part1()
    {
        var (available, desired) = ParseTowels();
        var result = Combinate(available, desired);
        // Console.WriteLine($"combos: {string.Join('\n', result.Keys)}");

        Console.WriteLine($"Only {result.Count} available combinations of towels available");
    }

    public void Part2()
    {
        throw new NotImplementedException();
    }

    private (List<string> available, List<string> desired) ParseTowels()
    {
        var available = _input[0].Split(", ").OrderBy(x => x.Length).ToList();
        var desired = _input[2..5].ToList();
        return (available, desired);
    }

    private static Dictionary<string, List<List<string>>> Combinate(List<string> available, List<string> desired)
    {
        var results = new Dictionary<string, List<List<string>>>();
        var memo = new Dictionary<string, List<string>>();

        foreach (var want in desired)
        {
            var chunks = available.Where(want.Contains).OrderByDescending(t => t.Length).ToList();
            var combinations = GetCombinations(want, chunks, memo);
            foreach (var combo in combinations)
            {
                if (want.Equals(string.Join("", combo)))
                {
                    if (results.ContainsKey(want))
                        results[want].Add(combo);
                    else
                        results[want] = [combo];

                    // Console.WriteLine($"combo: {string.Join(',', combo)}");
                }
            }
        }
        return results;
    }

    private static List<List<string>> GetCombinations(string word, List<string> towels, Dictionary<string, List<string>> memo)
    {
        var matches = CascadeTowels(word, towels, memo).ToList();
        var returnList = matches.Split(x => x.Equals(","));

        return returnList.ToList();
    }

    private static IEnumerable<string> CascadeTowels(string wordSegment, List<string> towels, Dictionary<string, List<string>> memo)
    {
        if (wordSegment == string.Empty)
        {
            yield return ",";
            yield break;
        }

        var towelSubset = towels.Where(wordSegment.Contains).OrderByDescending(t => t.Length).ToList();
        if (towelSubset.Count == 0)
            yield break;

        if (memo.TryGetValue(wordSegment, out var result))
            foreach (var t in result)
                yield return t;

        foreach (var towel in towelSubset)
        {

            if (wordSegment.StartsWith(towel))
            {
                memo[wordSegment] = [towel];
                yield return towel;

                var cascades = CascadeTowels(wordSegment[towel.Length..], towelSubset, memo).ToList();
                memo[wordSegment].AddRange(cascades);

                foreach (var segment in cascades)
                    yield return segment;
            }
        }
    }
}