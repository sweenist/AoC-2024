namespace AdventOfCode2024.Utility;

public static class StreamHelper
{
    public static Stream GetStream(string input)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(input);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}