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
        var compressed = CompressBlocks(fileIds);
        var checkSum = compressed.Select((x, i) => (long)x * i).Sum();

        Console.WriteLine($"Disk checksum after block moving is {checkSum}");
    }

    public void Part2()
    {
        var fileIds = ParseDiskSpace().ToList();
        var compressed = CompressFiles(fileIds);
        var checkSum = compressed.Select((x, i) => x == -1 ? 0L : (long)x * i).Sum();

        Console.WriteLine($"Disk checksum after file moving is {checkSum}");
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

    private static IEnumerable<int> CompressBlocks(IEnumerable<int> diskBlocks)
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

    private static IEnumerable<int> CompressFiles(IEnumerable<int> diskBlocks)
    {
        var blocks = diskBlocks.ToList();
        var allocations = blocks.Zip(Enumerable.Range(0, blocks.Count), Tuple.Create)
            .Where((x, i) => i == 0 || blocks[i - 1] != x.Item1).ToList();
        var chunks = allocations
            .Select((x, i) => i == allocations.Count - 1
                ? (Id: x.Item1, Index: x.Item2, Length: blocks.Count - x.Item2)
                : (Id: x.Item1, Index: x.Item2, Length: allocations[i + 1].Item2 - x.Item2)).ToList();
        var fileChunks = new Queue<(int Id, int Index, int Length)>(chunks.Where(t => t.Id != -1).OrderByDescending(t => t.Index).ToList());

        var notFound = (Id: -2, Index: 0, Length: 0);
        while (fileChunks.Count > 0)
        {
            var file = fileChunks.Dequeue();
            if (file.Id == 0) break;

            var freeSpaces = chunks.Where(t => t.Id == -1).ToList();
            var freeSpace = freeSpaces.FirstOrDefault(x => x.Length >= file.Length, notFound);

            if (freeSpace == notFound)
                continue;
            if (freeSpace.Index > file.Index)
                continue;

            var freeIndex = chunks.IndexOf(freeSpace);
            var fileIndex = chunks.IndexOf(file);
            chunks[freeIndex] = (file.Id, freeSpace.Index, file.Length);
            chunks[fileIndex] = (Id: -1, file.Index, file.Length);

            if (file.Length != freeSpace.Length)
            {
                var remaining = (Id: -1, Index: freeSpace.Index + file.Length, Length: freeSpace.Length - file.Length);
                chunks.Insert(freeIndex + 1, remaining);
                fileIndex++;
            }
            if (chunks.Count - 1 >= fileIndex + 1 && chunks[fileIndex].Id == -1 && chunks[fileIndex + 1].Id == -1)
            {
                var freeSpaceToPop = chunks[fileIndex + 1];
                var newFree = chunks[fileIndex];
                chunks[fileIndex] = (newFree.Id, newFree.Index, newFree.Length + freeSpaceToPop.Length);
                chunks.RemoveAt(fileIndex + 1);
            }
            if (fileIndex > 0 && chunks[fileIndex].Id == -1 && chunks[fileIndex - 1].Id == -1)
            {
                var freeSpaceToAppend = chunks[fileIndex];
                var oldFree = chunks[fileIndex - 1];
                chunks[fileIndex - 1] = (oldFree.Id, oldFree.Index, oldFree.Length + freeSpaceToAppend.Length);
                chunks.RemoveAt(fileIndex);
            }
        }
        return chunks.SelectMany(x => Enumerable.Repeat(x.Id, x.Length));
    }
}