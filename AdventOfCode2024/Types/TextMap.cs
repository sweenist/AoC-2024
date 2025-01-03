namespace AdventOfCode2024.Types;

public abstract class TextMap
{
    public TextMap(string[] input)
    {
        Bounds = new Boundary(input.Length, input[0].Length);
        Walkable = new bool[Bounds.Width, Bounds.Height];
    }

    public bool[,] Walkable { get; set; }
    public Boundary Bounds { get; set; }
}
