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
        var compressed = CompressFiles();
        var checkSum = compressed.Select((x, i) => x == -1 ? 0L : (long)x * i).Sum();

        Console.WriteLine($"Disk checksum after file moving is {checkSum}");
    }

    private IEnumerable<int> ParseDiskSpace()
    {
        var id = 0;
        return _input.SelectMany((x, i) =>
        {
            var length = int.Parse(x.ToString());
            var result = Enumerable.Repeat(id++ % 2 == 1 ? -1 : id / 2, length);
            return result;
        });
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

    private IEnumerable<(int Id, int Index, int Length)> UncompressDiskSpace()
    {
        var index = 0;
        return _input.Select((x, i) =>
        {
            var length = int.Parse(x.ToString());
            var returnValue = (Id: i % 2 == 1 ? -1 : i / 2, Index: index, Length: length);
            index += length;
            return returnValue;
        });
    }

    private IEnumerable<int> CompressFiles()
    {
        var chunks = UncompressDiskSpace().ToList();
        var fileChunks = new Queue<(int Id, int Index, int Length)>([.. chunks.Where(t => t.Id != -1).OrderByDescending(t => t.Index)]);

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