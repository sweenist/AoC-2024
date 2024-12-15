using AdventOfCode2024.Types;
using AdventOfCode2024.Utility;
using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Days;

public class Day15 : IDay
{
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

    private readonly Vector[] _movements = [];
    private readonly Map _map;

    public Day15(bool useExample = false)
    {
        (_map, _movements) = ParseInput(useExample);
    }

    public void Part1()
    {
        throw new NotImplementedException();
    }

    public void Part2()
    {
        throw new NotImplementedException();
    }

    private (Map Map, Vector[] Movements) ParseInput(bool isExample)
    {
        var inputFile = $"inputData/{GetType().Name}.txt";
        using var reader = isExample
            ? new StreamReader(StreamHelper.GetStream(_example))
            : new StreamReader(inputFile);

        bool parseMovements = false;
        var mapLines = new List<string>();

        while (!reader.EndOfStream)
        {
            if (parseMovements)
            {
                var rawMovements = string.Join(",", reader.ReadToEnd().Split('\n'));
                var movements = InitializeMovements(rawMovements);
                var map = new Map(mapLines);
                return (map, movements);
            }
            var line = reader.ReadLine();
            if (line!.Trim().Equals(string.Empty))
            {
                parseMovements = true;
                continue;
            }
            mapLines.Add(line);
        }

        throw new InvalidDataException("Input format was incorrect");
    }

    private Vector[] InitializeMovements(string input)
    {
        var hatStick = new Dictionary<char, Vector>{
            {'^', Vector.North},
            {'>', Vector.East},
            {'v', Vector.South},
            {'<', Vector.West},
        };
        return input.Select(hatStick.GetValueOrDefault).ToArray();
    }

    private record Map
    {
        public Map(List<string> rawMap)
        {
            Bounds = new Boundary(rawMap.Count, rawMap[0].Length);
            var walls = new List<Point>();
            var boxes = new List<Point>();

            for (var y = 0; y < Bounds.Height; y++)
                for (var x = 0; x < Bounds.Width; x++)
                {
                    switch (rawMap[y][x])
                    {
                        case '#':
                            walls.Add(new Point(x, y));
                            break;
                        case '@':
                            Robot = new Point(x, y);
                            break;
                        case 'O':
                            boxes.Add(new Point(x, y));
                            break;
                        default:
                            break;
                    }
                }
            Walls = [.. walls];
            Boxes = [.. boxes];
        }
        public Boundary Bounds { get; set; }
        public Point[] Walls { get; set; }
        public Point[] Boxes { get; set; }
        public Point Robot { get; set; }
    }
}