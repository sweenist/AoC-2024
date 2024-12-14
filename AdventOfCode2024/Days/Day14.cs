using System.Text.RegularExpressions;
using AdventOfCode2024.Types;
using AdventOfCode2024.Utility;
using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Days;

public class Day14 : IDay
{
    private string _example = @"p=0,4 v=3,-3
p=6,3 v=-1,-3
p=10,3 v=-1,2
p=2,0 v=2,-1
p=0,0 v=1,3
p=3,0 v=-2,-2
p=7,6 v=-1,-3
p=3,0 v=-1,-2
p=9,3 v=2,3
p=7,3 v=-1,2
p=2,4 v=2,-3
p=9,5 v=-3,-3";

    private readonly List<string> _input = [];
    private readonly Boundary _bounds;
    private readonly List<(Point Min, Point Max)> _quadrants;

    public Day14(bool useExample = false)
    {
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            _input.AddRange(_example.Split('\n'));
            _bounds = new Boundary(7, 11);
        }
        else
        {
            var inputFile = $"inputData/{GetType().Name}.txt";
            using var sr = new StreamReader(inputFile);
            _input.AddRange(sr.ReadToEnd().Split('\n'));
            _bounds = new Boundary(103, 101);
        }

        _quadrants = [
            (Min:new Point(0, 0), Max: new Point(_bounds.BoundX / 2 -1, _bounds.BoundY / 2 - 1)),
            (Min:new Point(_bounds.BoundX /2 +1, 0), Max: new Point(_bounds.BoundX, _bounds.BoundY/2-1)),
            (Min:new Point(0,_bounds.BoundY/2+1), Max: new Point(_bounds.BoundX /2 -1, _bounds.BoundY)),
            (Min:new Point(_bounds.BoundX/2+1,_bounds.BoundY/2+1), Max: new Point(_bounds.BoundX, _bounds.BoundY)),
        ];
    }

    public void Part1()
    {
        var robots = SetupRobots();

        foreach (var robot in robots)
            MoveRobot(robot, 100);



        var safetyScore = _quadrants.Select(x => robots.Where(r => r.Location.IsBetween(x.Min, x.Max)).Count()).Product();

        Console.WriteLine($"The safety score after 100 seconds is {safetyScore}");
    }

    public void Part2()
    {
        var lcm = _bounds.Height * _bounds.Width;
        var robots = SetupRobots();
        var scores = new Dictionary<int, int>();

        for (var i = 1; i < lcm; i += 1)
        {
            MoveRobots(robots, 1);
            var safetyScore = _quadrants.Select(x => robots.Where(r => r.Location.IsBetween(x.Min, x.Max)).Count()).Product();
            scores[i] = safetyScore;
        }

        var secondsToLowestScore = scores.OrderBy(x => x.Value).First().Key;
        robots = SetupRobots();
        MoveRobots(robots, secondsToLowestScore);

        var map = Diagnostics.PrintMapWithCounts(robots.Select(x => (ICoordinate)x.Location), _bounds);

        Console.WriteLine(map);
        Console.WriteLine($"Christmas tree found at {secondsToLowestScore}");
    }

    private List<Robot> SetupRobots()
    {
        var pattern = @"p=(-?\d+),(-?\d+) v=(-?\d+),(-?\d+)";

        (Point Position, Vector Velocity) ParseRoboSpecs(string specs)
        {
            var matches = Regex.Match(specs, pattern);
            var position = new Point(int.Parse(matches.Groups[1].Value), int.Parse(matches.Groups[2].Value));
            var velocity = new Vector(int.Parse(matches.Groups[3].Value), int.Parse(matches.Groups[4].Value));
            return (position, velocity);
        }

        return _input.Select(ParseRoboSpecs).Select(x => new Robot(x.Position, x.Velocity)).ToList();
    }

    private void MoveRobot(Robot robot, int moves)
    {
        robot.Move(moves);
        robot.Normalize(_bounds);
    }

    private void MoveRobots(List<Robot> robots, int seconds)
    {
        foreach (var robot in robots)
        {
            MoveRobot(robot, seconds);
        }
    }

    public record Robot(Point Location, Vector Velocity)
    {
        public Point Location { get; set; } = Location;
        public Vector Velocity { get; set; } = Velocity;

        public void Move(int times)
        {
            Location += Velocity * times;
        }

        public void Normalize(Boundary bounds)
        {
            var xMod = Location.X % bounds.Width;
            var yMod = Location.Y % bounds.Height;
            var x = xMod < 0 ? bounds.Width + xMod : xMod;
            var y = yMod < 0 ? bounds.Height + yMod : yMod;
            Location = new Point(x, y);
        }

        public override string ToString()
        {
            return $"P:{Location}; V: {Velocity}";
        }
    }
}