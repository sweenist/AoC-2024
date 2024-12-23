namespace AdventOfCode2024.Days;

public class Day23 : IDay
{
    private string _example = @"kh-tc
qp-kh
de-cg
ka-co
yn-aq
qp-ub
cg-tb
vc-aq
tb-ka
wh-tc
yn-cg
kh-ub
ta-co
de-co
tc-td
tb-wq
wh-td
ta-ka
td-qp
aq-cg
wq-ub
ub-vc
de-ta
wq-aq
wq-vc
wh-yn
ka-de
kh-ta
co-tc
wh-qp
tb-vc
td-yn";

    private readonly List<string> _input = [];

    public Day23(bool useExample = false)
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
        var combos = new HashSet<string>();
        var lanNodes = _input.SelectMany(x => x.Split('-')).Distinct().ToDictionary(x => x, v => new HashSet<string>());

        foreach (var key in lanNodes.Keys)
        {
            lanNodes[key].UnionWith(_input.Where(s => s.Contains(key)).SelectMany(x => x.Split('-').Where(p => p != key)));
        }
        var keys = lanNodes.Keys.OrderBy(x => x).ToList();
        for (var i = 0; i < lanNodes.Keys.Count - 1; i++)
        {
            var masterKey = keys[i];
            for (var j = i + 1; j < lanNodes.Keys.Count; j++)
            {
                var key = keys[j];
                if (lanNodes[masterKey].Contains(key) && lanNodes[key].Contains(masterKey))
                {
                    combos.UnionWith(lanNodes[masterKey]
                            .Intersect(lanNodes[key])
                            .Where(x => x[0] == 't')
                            .Select(x => string.Join(",", new[] { masterKey, key, x }
                            .OrderBy(x => x))));
                }
            }
        }

        Console.WriteLine($"There are {combos.Count} computers that start with 't");
    }

    public void Part2()
    {
        var combos = new HashSet<string>();
        var lanNodes = _input.SelectMany(x => x.Split('-')).Distinct().ToDictionary(x => x, v => new HashSet<string>());

        foreach (var key in lanNodes.Keys)
        {
            lanNodes[key].UnionWith(_input.Where(s => s.Contains(key))
                                          .SelectMany(x => x.Split('-').Where(p => p != key))
                                          .Append(key)
                                          .OrderBy(x => x));
        }
        var passwords = new SortedSet<(int Count, string Password)>(Comparer<(int Count, string Password)>.Create((a, b) => a.Count.CompareTo(b.Count)));
        foreach (var (key, value) in lanNodes)
        {
            var allKeys = lanNodes[key];
            var intersections = allKeys.Select(k => lanNodes[k].Intersect(value).ToList())
                                       .GroupBy(x => string.Join(',', x), v => v.Count)
                                       .Where(kvp => kvp.Key.Count(s => s == ',') == kvp.Count())
                                       .ToDictionary(x => x.Key, v => v.Max());

            if (intersections.Count == 0) continue;
            var maxPassword = intersections.MaxBy(kvp => kvp.Value);
            passwords.Add((maxPassword.Value, maxPassword.Key));

        }

        Console.WriteLine($"The LAN party password is {passwords.Last().Password}");
    }
}