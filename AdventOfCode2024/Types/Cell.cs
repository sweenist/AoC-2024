namespace AdventOfCode2024.Types;

public record Cell<T>(T Parent)
{
    public T Parent { get; set; } = Parent;
    public int TotalScore { get; set; } = int.MaxValue;
    public int Accumulated { get; set; } = int.MaxValue;
    public int Heuristic { get; set; } = int.MaxValue;

    public override string ToString()
    {
        return $"Cell: {Parent}; f={TotalScore} g={Accumulated} h={Heuristic}";
    }
}

