using System.Text;
using AdventOfCode2024.Types;
using AdventOfCode2024.Types.Day15;
using AdventOfCode2024.Utility;
using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Days;

public class Day15 : IDay
{
    private readonly string _small = @"########
#..O.O.#
##@.O..#
#...O..#
#.#.O..#
#...O..#
#......#
########

<^^>>>vv<v>>v<<";

    private readonly string _example = @"##########
#..O..O.O#
#......O.#
#.OO..O.O#
#..O@..O.#
#O#..O...#
#O..O..O.#
#.OO.O.OO#
#....O...#
##########

<vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
<<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
>^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
<><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^";

    private readonly bool _useExample;

    public Day15(bool useExample = false)
    {
        _useExample = useExample;
    }

    public void Part1()
    {
        var (iMap, movements) = ParseInput();
        var map = (Map)iMap;

        foreach (var move in movements)
        {
            map.Move(move);
        }
        var totalGpsScore = map.Boxes.Select(b => (long)b.Y * 100 + b.X).Sum();

        Console.WriteLine($"All box GPS coordinates in warehouse 1 are {totalGpsScore}");
    }

    public void Part2()
    {
        var (iMap, movements) = ParseInput(expand: true);
        var map = (ExpandedMap)iMap;

        foreach (var move in movements)
        {
            map.Move(move);
        }
        var totalGpsScore = map.Boxes.Select(b => (long)b.Left.Y * 100 + b.Left.X).Sum();

        Console.WriteLine($"All box GPS coordinates in warehouse 2 are {totalGpsScore}");
    }

    private (IMap Map, Vector[] Movements) ParseInput(bool expand = false)
    {
        bool useSmall = false;
        var inputFile = $"inputData/{GetType().Name}.txt";
        using var reader = _useExample
            ? new StreamReader(StreamHelper.GetStream(useSmall ? _small : _example))
            : new StreamReader(inputFile);

        bool parseMovements = false;
        var mapLines = new List<string>();

        while (!reader.EndOfStream)
        {
            if (parseMovements)
            {
                var rawMovements = string.Join(",", reader.ReadToEnd().Split('\n'));
                var movements = InitializeMovements(rawMovements);
                IMap map = expand ? new ExpandedMap(mapLines) : new Map(mapLines);
                return (map, movements);
            }

            var line = reader.ReadLine();
            if (line!.Trim().Equals(string.Empty))
            {
                parseMovements = true;
                continue;
            }
            mapLines.Add(expand ? ExpandLine(line) : line);
        }

        throw new InvalidDataException("Input format was incorrect");
    }

    private static string ExpandLine(string line)
    {
        var sb = new StringBuilder();
        foreach (var c in line)
        {
            if (c == '#')
                sb.Append("##");
            else if (c == 'O')
                sb.Append("[]");
            else if (c == '@')
                sb.Append("@.");
            else
                sb.Append("..");
        }
        return sb.ToString();
    }

    private static Vector[] InitializeMovements(string input)
    {
        var hatStick = new Dictionary<char, Vector>{
            {'^', Vector.North},
            {'>', Vector.East},
            {'v', Vector.South},
            {'<', Vector.West},
        };
        return input.Select(hatStick.GetValueOrDefault).ToArray();
    }
}