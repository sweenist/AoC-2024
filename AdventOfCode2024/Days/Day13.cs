using System.Text.RegularExpressions;
using AdventOfCode2024.Utility.Math;

namespace AdventOfCode2024.Days;

public class Day13 : IDay
{
    private string _example = @"Button A: X+94, Y+34
Button B: X+22, Y+67
Prize: X=8400, Y=5400

Button A: X+26, Y+66
Button B: X+67, Y+21
Prize: X=12748, Y=12176

Button A: X+17, Y+86
Button B: X+84, Y+37
Prize: X=7870, Y=6450

Button A: X+69, Y+23
Button B: X+27, Y+71
Prize: X=18641, Y=10279";

    private readonly List<string> _input = [];
    private readonly List<ClawMachine> _machines = [];

    public Day13(bool useExample = false)
    {
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            _input.AddRange(_example.Split("\n\n"));
        }
        else
        {
            var inputFile = $"inputData/{GetType().Name}.txt";
            using var sr = new StreamReader(inputFile);
            _input.AddRange(sr.ReadToEnd().Split("\n\n"));
        }
        _machines.AddRange(ConfigureMachines());
    }

    public void Part1()
    {
        var tokensNeeded = _machines.Select(x => x.TotalTokens).Sum();
        Console.WriteLine($"It takes a least {tokensNeeded} tokens to win all the prizes");
    }

    public void Part2()
    {
        throw new NotImplementedException();
    }

    private IEnumerable<ClawMachine> ConfigureMachines()
    {
        foreach (var parameters in _input)
        {
            var inputVectors = parameters.Split('\n').Select(ParseInput).ToDictionary();
            yield return new ClawMachine(inputVectors);
        }
    }

    private KeyValuePair<string, ICoordinate> ParseInput(string input)
    {
        if (input.StartsWith("Button"))
        {
            var pattern = @"Button ([A|B]{1}): X\+(\d+), Y\+(\d+)";
            var match = Regex.Match(input, pattern);
            return new KeyValuePair<string, ICoordinate>(
                key: match.Groups[1].Value,
                value: new Vector(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value)));
        }
        if (input.StartsWith("Prize"))
        {
            var pattern = @"X=(\d+), Y=(\d+)";
            var match = Regex.Match(input, pattern);
            return new KeyValuePair<string, ICoordinate>(
                key: "Prize",
                value: new Point(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value)));
        }
        throw new Exception($"no patches for {input}");
    }

    private record ClawMachine
    {
        private readonly long _precisionFix = 10000000000000L;
        public ClawMachine(Dictionary<string, ICoordinate> inputs)
        {
            ButtonA = (Vector)inputs["A"];
            ButtonB = (Vector)inputs["B"];
            Prize = (Point)inputs["Prize"];
            Solve();
        }
        public Vector ButtonA { get; private set; }
        public Vector ButtonB { get; private set; }
        public Point Prize { get; private set; }

        public int APresses { get; private set; }
        public int BPresses { get; private set; }

        public bool HasSolution { get; private set; }
        public int TotalTokens => (3 * APresses) + BPresses;

        public override string ToString()
        {
            return $"A: {ButtonA}; B: {ButtonB}; Prize: {Prize} {(HasSolution ? $"total tokens: {TotalTokens}" : "No solution")}";
        }

        private void Solve()
        {
            var matrixA = new Matrix2D(ButtonA, ButtonB, pivot: true);
            var matrix1 = new Matrix2D(Prize, ButtonB, pivot: true);
            var matrix2 = new Matrix2D(ButtonA, Prize, pivot: true);

            HasSolution = matrixA.HasWholeCoefficient(matrix1) && matrixA.HasWholeCoefficient(matrix2);
            if (HasSolution)
            {
                APresses = matrix1.Determinant / matrixA.Determinant;
                BPresses = matrix2.Determinant / matrixA.Determinant;
            }
        }

        private void ApplyPrecision()
        {

        }
    }
}