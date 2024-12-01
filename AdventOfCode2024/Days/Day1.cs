namespace AdventOfCode2024.Days;

public class Day1
{
    private List<int> _first = new();
    private List<int> _second = new();

    public Day1()
    {
        var inputFile = $"inputData/{this.GetType().Name}.txt";
        using (var sr = new StreamReader(inputFile))
        {
            while (!sr.EndOfStream)
            {
                var pair = sr.ReadLine().Split("   ");
                _first.Add(Convert.ToInt32(pair[0]));
                _second.Add(Convert.ToInt32(pair[1]));
            }
        }
    }

    public void Part1()
    {
        _first.Sort();
        _second.Sort();
        Console.WriteLine($"First: {_first[0]}| Second: {_second[0]}");

        var totalDiffs = _first.Zip(_second, (a, b) => Math.Abs(b - a)).Sum();
        Console.WriteLine($"Total Diffs are: {totalDiffs}");
    }

    public void Part2()
    {
        throw new NotImplementedException("Not ready for Part 2 yet");
    }
}
