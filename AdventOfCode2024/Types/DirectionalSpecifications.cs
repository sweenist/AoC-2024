using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Types;

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

    public void Move(Vector source)
    {
        //TODO:
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
    public static DirectionalSpecifications Activate => new(KeyPad.Activate);
    public static DirectionalSpecifications Up => new(KeyPad.Up);
    public static DirectionalSpecifications Down => new(KeyPad.Down);
    public static DirectionalSpecifications Left => new(KeyPad.Left);
    public static DirectionalSpecifications Right => new(KeyPad.Right);
}