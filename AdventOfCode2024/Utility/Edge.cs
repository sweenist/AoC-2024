
using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Utility;

public struct Edge
{
    public char Id { get; set; }
    public int Perimeter { get; set; }
    public int Side { get; set; }
}

public class EdgeDetector
{
    private readonly Dictionary<Vector, bool> _foundEdges;

    public EdgeDetector()
    {
        _foundEdges = Vector.CardinalPoints.Select(x => new KeyValuePair<Vector, bool>(x, false)).ToDictionary();
    }

    public bool this[Vector key]
    {
        get { return _foundEdges[key]; }
        set { _foundEdges[key] = value; }
    }

    public bool North => _foundEdges[Vector.North];
    public bool East => _foundEdges[Vector.East];
    public bool South => _foundEdges[Vector.South];
    public bool West => _foundEdges[Vector.West];

    public int EdgeCount => _foundEdges.Values.Count(x => x);
    public bool ParallelWalls => EdgeCount == 2 && ((North && South) || (East && West));
    public bool IsCorner => EdgeCount == 2 && ((North && East) || (East && South) || (South && West) || (West && North));
}

public static class EdgeMap
{
    //4 sides
    public static Edge O => new() { Id = 'o', Perimeter = 4, Side = 4 };

    //3 sides
    public static Edge C => new() { Id = 'c', Perimeter = 3, Side = 2 };
    public static Edge N => new() { Id = 'n', Perimeter = 3, Side = 2 };
    public static Edge D => new() { Id = ')', Perimeter = 3, Side = 2 };
    public static Edge U => new() { Id = 'u', Perimeter = 3, Side = 2 };

    //2 sides - corner
    public static Edge NW => new() { Id = 'r', Perimeter = 2, Side = 1 };
    public static Edge NE => new() { Id = '?', Perimeter = 2, Side = 1 };
    public static Edge SE => new() { Id = 'j', Perimeter = 2, Side = 1 };
    public static Edge SW => new() { Id = 'L', Perimeter = 2, Side = 1 };

    //2 sides - 1 wide
    public static Edge Vertical => new() { Id = '|', Perimeter = 2, Side = 0 };
    public static Edge Horizontal => new() { Id = '=', Perimeter = 2, Side = 0 };

    //1 side

    public static Edge North => new() { Id = 'N', Perimeter = 1, Side = 0 };
    public static Edge East => new() { Id = 'E', Perimeter = 1, Side = 0 };
    public static Edge South => new() { Id = 'S', Perimeter = 1, Side = 0 };
    public static Edge West => new() { Id = 'W', Perimeter = 1, Side = 0 };

    //Empty
    public static Edge None => new() { Id = '.', Perimeter = 0, Side = 0 };

    public static Edge ParseEdges(EdgeDetector detector)
    {
        return detector.EdgeCount switch
        {
            0 => None,
            1 => detector.North ? North : detector.East ? East : detector.South ? South : West, //gross
            2 when detector.ParallelWalls => detector.North ? Horizontal : Vertical,
            2 when detector.IsCorner => NW,
            3 => U,
            4 => O,
            _ => throw new Exception($"Cannot have more than 4 edges {detector.EdgeCount}")
        };
    }
}
