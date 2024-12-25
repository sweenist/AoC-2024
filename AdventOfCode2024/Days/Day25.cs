namespace AdventOfCode2024.Days;

public class Day25 : IDay
{
    private string _example = @"#####
.####
.####
.####
.#.#.
.#...
.....

#####
##.##
.#.##
...##
...#.
...#.
.....

.....
#....
#....
#...#
#.#.#
#.###
#####

.....
.....
#.#..
###..
###.#
###.#
#####

.....
.....
.....
#....
#.#..
#.#.#
#####";

    private readonly string _input;

    public Day25(bool useExample = false)
    {
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            _input = _example;
        }
        else
        {
            var inputFile = $"inputData/{GetType().Name}.txt";
            using var sr = new StreamReader(inputFile);
            _input = sr.ReadToEnd();
        }
    }

    public void Part1()
    {
        var fits = 0;
        var (locks, keys) = ParseKeyLocks();

        foreach (var aLock in locks)
            foreach (var key in keys)
                fits += aLock.Zip(key, (l, k) => l + k).All(x => x < 6) ? 1 : 0;

        Console.WriteLine($"There are {fits} lock key combo matches");
    }

    public void Part2()
    {
        throw new NotImplementedException();
    }

    private (List<List<int>> Locks, List<List<int>> Keys) ParseKeyLocks()
    {
        var keysAndLocks = _input.Split($"{Environment.NewLine}{Environment.NewLine}");
        var keys = new List<List<int>>();
        var locks = new List<List<int>>();

        foreach (var schematic in keysAndLocks)
        {
            var rows = schematic.Split(Environment.NewLine);
            var newThing = Enumerable.Repeat(0, 5).ToList();
            if (rows[0] == "#####")
            {
                for (var y = 1; y < rows.Length; y++)
                    for (var x = 0; x < rows[0].Length; x++)
                    {
                        if (rows[y][x] == '#') newThing[x]++;
                    }
                locks.Add(newThing);
            }
            else
            {
                for (var y = 0; y < rows.Length - 1; y++)
                    for (var x = 0; x < rows[0].Length; x++)
                    {
                        if (rows[y][x] == '#') newThing[x]++;
                    }

                keys.Add(newThing);
            }
        }
        return (locks, keys);
    }


}