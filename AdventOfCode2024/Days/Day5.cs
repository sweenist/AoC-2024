namespace AdventOfCode2024.Days;

public class Day5 : IDay
{
    private string _example = @"47|53
97|13
97|61
97|47
75|29
61|13
75|53
29|13
97|29
53|29
61|53
97|53
61|29
47|13
75|47
97|75
47|61
75|61
47|29
75|13
53|13

75,47,61,53,29
97,61,53,29,13
75,29,13
75,97,47,61,53
61,13,29
97,13,75,29,47";

    private readonly List<string> _input = [];
    private readonly Dictionary<int, List<int>> _orderingRules = [];
    private readonly List<List<int>> _pagesToPrint = [];

    public Day5(bool useExample = false)
    {
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            _input.AddRange(_example.Split('\n'));
            return;
        }
        var inputFile = $"inputData/{GetType().Name}.txt";
        using var sr = new StreamReader(inputFile);
        _input.AddRange(sr.ReadToEnd().Split('\n'));

        _orderingRules = GetOrderingRules();
        _pagesToPrint = GetPageUpdates();
    }

    public void Part1()
    {
        var validPageTotal = 0;

        foreach (var print in _pagesToPrint)
        {
            var middleIndex = (print.Count - 1) / 2;
            var isValidOrdering = true;

            for (var i = 1; i < print.Count; i++)
            {
                var key = print[i];
                if (!_orderingRules.ContainsKey(key))
                    continue;
                var rules = _orderingRules[key];
                var left = print.Take(i);
                if (rules.Intersect(left).Any())
                {
                    isValidOrdering = false;
                    break;
                }
            }
            if (isValidOrdering)
                validPageTotal += print[middleIndex];
        }

        Console.WriteLine($"Total of middle page numbers of correct rules: {validPageTotal}");
    }

    public void Part2()
    {
        throw new NotImplementedException();
    }

    private Dictionary<int, List<int>> GetOrderingRules()
    {
        return _input.Where(x => x.Contains('|'))
                     .Select(x => x.Split('|').Select(int.Parse).ToList())
                     .GroupBy(x => x[0], g => g[1])
                     .ToDictionary(x => x.Key, x => x.ToList());
    }

    private List<List<int>> GetPageUpdates()
    {
        return _input.Where(x => x.Contains(','))
                     .Select(x => x.Split(',').
                        Select(int.Parse).ToList())
                     .ToList();
    }
}
