using CommandLine;

public class Options
{
    [Option(Required = true)]
    public int Day { get; set; }

    [Option(Required = true)]
    public int Part { get; set; }

    [Option('e', "useExample")]
    public bool UseExample { get; set; }

    [Option('p', "profile")]
    public bool UseProfiler { get; set; }
}
