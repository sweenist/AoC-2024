namespace AdventOfCode2024.Days;

public class Day9 : IDay
{
    private string _example = "2333133121414131402";

    private readonly string _input;

    public Day9(bool useExample = false)
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
        var fileIds = ParseDiskSpace().ToList();
        var compressed = Compress(fileIds);
        var checkSum = compressed.Select((x, i) => (long)x * i).Sum();

        Console.WriteLine($"Disk checksum is {checkSum}");
    }

    public void Part2()
    {
        throw new NotImplementedException();
    }

    private IEnumerable<int> ParseDiskSpace()
    {
        var id = 0;
        IEnumerable<int> ParseBlock(string seg, int index)
        {
            var length = int.Parse(seg);
            var isFreeSpace = index % 2 == 1;
            var result = Enumerable.Repeat(isFreeSpace ? -1 : id, length);
            if (!isFreeSpace)
                id++;
            return result;
        }
        return _input.SelectMany((x, i) => ParseBlock(x.ToString(), i));
    }

    private static IEnumerable<int> Compress(IEnumerable<int> diskBlocks)
    {
        var reversed = new Queue<int>(diskBlocks.Reverse());
        var blocks = diskBlocks.ToList();

        while (true)
        {
            var nextFreeSpace = blocks.IndexOf(-1);
            var nextId = reversed.Dequeue();
            if (nextId == -1)
                continue;

            blocks[nextFreeSpace] = nextId;
            var originalPosition = blocks.LastIndexOf(nextId);
            blocks.RemoveAt(originalPosition);

            if (blocks.IndexOf(-1) == -1) break;
        }
        return blocks;
    }
}