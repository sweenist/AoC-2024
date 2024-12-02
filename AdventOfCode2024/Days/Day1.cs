namespace AdventOfCode2024.Days;

public class Day1 : IDay
{
    private readonly List<int> _first = [];
    private readonly List<int> _second = [];

    public Day1()
    {
        var inputFile = $"inputData/{GetType().Name}.txt";
        using var sr = new StreamReader(inputFile);
        while (!sr.EndOfStream)
        {
            var pair = (sr.ReadLine()?.Split("   "))
                ?? throw new InvalidDataException("unexpected data parsing issue");
            _first.Add(Convert.ToInt32(pair[0]));
            _second.Add(Convert.ToInt32(pair[1]));
        }
    }

    public void Part1()
    {
        _first.Sort();
        _second.Sort();

        var totalDiffs = _first.Zip(_second, (a, b) => Math.Abs(b - a)).Sum();
        Console.WriteLine($"Total span between ordered ids are: {totalDiffs}");
    }

    public void Part2()
    {
        var similarityMultipliers = _second.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
        var sum = 0L;

        foreach (var id in _first)
        {
            sum += id * similarityMultipliers.GetValueOrDefault(id, 0);
        }

        Console.WriteLine($"Similarity Score of the Historian's list is: {sum}");
    }
}
