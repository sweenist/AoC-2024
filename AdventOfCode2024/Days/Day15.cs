using System.Text;
using AdventOfCode2024.Types;
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

        // Console.WriteLine(_map);
        Console.WriteLine($"All box GPS coordinates are {totalGpsScore}");
    }

    public void Part2()
    {
        throw new NotImplementedException();
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
                IMap map = expand ? new Map(mapLines) : new ExpandedMap(mapLines);
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
            else
                sb.Append(c);
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

    #region types
    private interface IMap
    {
        Point Robot { get; set; }
    }

    private record Map : IMap
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
        public List<Point> Walls { get; set; }
        public List<Point> Boxes { get; set; }
        public Point Robot { get; set; }

        public void Move(Vector move)
        {
            var (boxIndices, canMove) = CheckLine(move, Robot, []);
            if (canMove)
            {
                Robot += move;
                foreach (var index in boxIndices)
                    Boxes[index] += move;
            }
        }

        public (List<int> boxIndices, bool canMove) CheckLine(Vector move, Point location, List<int> boxIndices)
        {
            var nextLocation = location + move;
            if (Walls.Any(x => x.Equals(nextLocation)))
                return ([], false);
            if (Boxes.Any(x => x.Equals(nextLocation)))
            {
                boxIndices.Add(Boxes.IndexOf(nextLocation));
                return CheckLine(move, nextLocation, boxIndices);
            }
            else
                return (boxIndices, true);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var y = 0; y < Bounds.Height; y++)
            {
                for (var x = 0; x < Bounds.Width; x++)
                {
                    var printChar = '.';
                    var currentPoint = new Point(x, y);
                    if (Robot == currentPoint) printChar = '@';
                    else if (Walls.Any(w => w.Equals(currentPoint))) printChar = '#';
                    else if (Boxes.Any(b => b.Equals(currentPoint))) printChar = 'O';
                    sb.Append(printChar);
                }
                sb.Append('\n');
            }
            return sb.ToString();
        }
    }

    struct Box(int x, int y)
    {
        public Point Left { get; set; } = new Point(x, y);
        public readonly Point Right => new Point(Left.X + 1, Left.Y);

        public readonly bool Contains(Point p)
        {
            return Left == p || Right == p;
        }
    }
    private record ExpandedMap : IMap
    {
        public ExpandedMap(List<string> rawMap)
        {
            Bounds = new Boundary(rawMap.Count, rawMap[0].Length);
            var walls = new List<Point>();
            var boxes = new List<Box>();

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
                        case '[':
                            boxes.Add(new Box(x, y));
                            x++;
                            break;
                        default:
                            break;
                    }
                }
            Walls = [.. walls];
            Boxes = [.. boxes];
        }

        public Boundary Bounds { get; set; }
        public List<Point> Walls { get; set; }
        public List<Box> Boxes { get; set; }
        public Point Robot { get; set; }

        public void Move(Vector move)
        {
            var (boxIndices, canMove) = CheckMove(move, Robot, []);
            if (canMove)
            {
                Robot += move;
                foreach (var index in boxIndices)
                    Boxes[index] += move;
            }
        }

        public (List<int> boxIndices, bool canMove) CheckMove(Vector move, Point location, List<int> boxIndices)
        {
            var nextLocation = location + move;
            if (Walls.Any(x => x.Equals(nextLocation)))
                return ([], false);
            if (Boxes.Any(x => x.Equals(nextLocation)))
            {
                boxIndices.Add(Boxes.IndexOf(nextLocation));
                return CheckLine(move, nextLocation, boxIndices);
            }
            else
                return (boxIndices, true);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var y = 0; y < Bounds.Height; y++)
            {
                for (var x = 0; x < Bounds.Width; x++)
                {
                    var printChar = '.';
                    var currentPoint = new Point(x, y);
                    if (Robot == currentPoint) printChar = '@';
                    else if (Walls.Any(w => w.Equals(currentPoint))) printChar = '#';
                    else if (Boxes.Any(b => b.Equals(currentPoint))) printChar = 'O';
                    sb.Append(printChar);
                }
                sb.Append('\n');
            }
            return sb.ToString();
        }
    }

    #endregion
}