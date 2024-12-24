using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Types.Day21;

public enum KeyPad
{
    Activate,
    Up,
    Left,
    Down,
    Right
}
public class DirectionalSpecifications
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public DirectionalSpecifications(KeyPad name)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        Name = name;
        ConstructButton();
    }

    public Actor Button { get; set; }
    public KeyPad Name { get; set; }
    public char Symbol { get; set; }
    public List<Vector> Activate { get; set; }
    public List<Vector> Up { get; set; }
    public List<Vector> Down { get; set; }
    public List<Vector> Left { get; set; }
    public List<Vector> Right { get; set; }

    public List<Vector> Move(Vector source)
    {
        return source == Vector.North ? Up
            : source == Vector.East ? Right
            : source == Vector.South ? Down
            : source == Vector.West ? Left
            : Activate;
    }

    private void ConstructButton()
    {
        switch (Name)
        {
            case KeyPad.Activate:
                Button = new Actor(new Point(2, 0), Vector.Zero);
                Activate = [Vector.Zero];
                Up = [Vector.West];
                Down = [Vector.South, Vector.West];
                Left = [Vector.South, Vector.West, Vector.West];
                Right = [Vector.South];
                Symbol = 'A';
                break;
            case KeyPad.Up:
                Button = new Actor(new Point(1, 0), Vector.North);
                Activate = [Vector.East];
                Up = [Vector.Zero];
                Down = [Vector.South];
                Left = [Vector.South, Vector.West];
                Right = [Vector.South, Vector.East];
                Symbol = '^';
                break;
            case KeyPad.Down:
                Button = new Actor(new Point(1, 1), Vector.South);
                Activate = [Vector.East, Vector.North];
                Up = [Vector.North];
                Down = [Vector.Zero];
                Left = [Vector.West];
                Right = [Vector.East];
                Symbol = 'v';
                break;
            case KeyPad.Left:
                Button = new Actor(new Point(0, 1), Vector.West);
                Activate = [Vector.East, Vector.East, Vector.North];
                Up = [Vector.East, Vector.North];
                Down = [Vector.East];
                Left = [Vector.Zero];
                Right = [Vector.East, Vector.East];
                Symbol = '<';
                break;
            case KeyPad.Right:
                Button = new Actor(new Point(2, 1), Vector.East);
                Activate = [Vector.North];
                Up = [Vector.West, Vector.North];
                Down = [Vector.West];
                Left = [Vector.West, Vector.West];
                Right = [Vector.Zero];
                Symbol = '>';
                break;
            default:
                throw new InvalidOperationException("Enum property not found: typeof KeyPad");
        }
    }
}

public class SpecManager
{
    private readonly Dictionary<Point, DirectionalSpecifications> _dirPad = new(){
        { new Point(1,0), Up},
        { new Point(2,0), Activate},  //Activate Button
        { new Point(0,1), Left},
        { new Point(1,1), Down},
        { new Point(2,1), Right},
    };

    private readonly Dictionary<Vector, Point> _vectorMapping = new(){
        {Vector.North, new Point(1,0)},
        {Vector.Zero, new Point(2,0)},  //Activate Button
        {Vector.West, new Point(0,1)},
        {Vector.South, new Point(1,1)},
        {Vector.East, new Point(2,1)},
    };

    public static DirectionalSpecifications Activate => new(KeyPad.Activate);
    public static DirectionalSpecifications Up => new(KeyPad.Up);
    public static DirectionalSpecifications Down => new(KeyPad.Down);
    public static DirectionalSpecifications Left => new(KeyPad.Left);
    public static DirectionalSpecifications Right => new(KeyPad.Right);

    public Point GetNewPosition(Vector target) => _vectorMapping[target];


    public List<Vector> Act(Point robotPosition, Vector target)
    {
        return _dirPad[robotPosition].Move(target);
    }
}

public class Robot
{
    private readonly SpecManager _moveManager;
    public Robot(SpecManager manager)
    {
        _moveManager = manager;
        CurrentPosition = new Point(2, 0); //Activate button
    }

    public Point CurrentPosition { get; set; }
    public Robot? Controller { get; set; }
    public long ActionsPerformed { get; set; }

    public void Move(Vector target, bool act = false)
    {
        var targets = _moveManager.Act(CurrentPosition, target);
        ActionsPerformed += targets.Count;
        CurrentPosition = _moveManager.GetNewPosition(target);

        foreach (var t in targets)
            Controller?.Move(t, act);
        if (!(targets.Count == 1 || targets.First().Equals(Vector.Zero)))
            ActionsPerformed += 1; // act at the end of target set
    }
}