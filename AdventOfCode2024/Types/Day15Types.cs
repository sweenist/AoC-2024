using System.Text;
using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Types.Day15;

public interface IMap
{
    Point Robot { get; set; }
}

public record Map : IMap
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

public record ExpandedMap : IMap
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
        if (move == Vector.North || move == Vector.South)
        {
            var result = CheckVertical(move, Robot, []).ToList();
            if (result.All(t => t.canMove))
            {
                Robot += move;
                result.SelectMany(t => t.boxes).Distinct().Shift(move);
            }
        }
        else
        {
            var (boxes, canMove) = CheckHorizontal(move, Robot, []);
            if (canMove)
            {
                Robot += move;
                boxes.Shift(move);
            }
        }
    }

    public IEnumerable<(HashSet<Box> boxes, bool canMove)> CheckVertical(Vector move, Point location, HashSet<Box> boxes)
    {
        var nextLocation = location + move;
        if (Walls.Any(w => w == nextLocation))
            yield return ([], false);
        if (Boxes.Any(b => b.Contains(nextLocation)))
        {
            var box = Boxes.Single(b => b.Contains(nextLocation));
            if (boxes.Add(box))
            {
                foreach (var result in CheckVertical(move, box.Left, boxes).Concat(CheckVertical(move, box.Right, boxes)))
                    yield return result;
            }
        }
        else
            yield return (boxes, true);
    }

    public (List<Box> boxes, bool canMove) CheckHorizontal(Vector move, Point location, List<Box> boxes)
    {
        var nextLocation = location + move;
        if (Walls.Any(x => x.Equals(nextLocation)))
            return ([], false);
        if (Boxes.Any(x => x.Contains(nextLocation)))
        {
            boxes.Add(Boxes.Single(x => x.Contains(nextLocation)));
            return CheckHorizontal(move, nextLocation + move, boxes);
        }
        else
            return (boxes, true);
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
                else if (Boxes.Any(b => b.Left.Equals(currentPoint))) printChar = '[';
                else if (Boxes.Any(b => b.Right.Equals(currentPoint))) printChar = ']';
                sb.Append(printChar);
            }
            sb.Append('\n');
        }
        return sb.ToString();
    }
}


public record Box(int X, int Y)
{
    public Point Left { get; set; } = new Point(X, Y);
    public Point Right => new Point(Left.X + 1, Left.Y);

    public bool Contains(Point p)
    {
        return Left == p || Right == p;
    }
}

public static class BoxCollection
{
    public static void Shift(this IEnumerable<Box> boxes, Vector shift)
    {
        using var e = boxes.GetEnumerator();
        while (e.MoveNext())
            e.Current.Left += shift;
    }
}
