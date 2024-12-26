using System.Text.RegularExpressions;

namespace AdventOfCode2024.Types.Day24Types;

public record Instruction
{
    public Instruction(string instruction)
    {
        var pattern = @"(?<left>\w+)\s(?<op>\w+)\s(?<right>\w+)\s->\s(?<buffer>\w+)";
        var matched = Regex.Match(string.Join('\n', instruction), pattern);

        Left = matched.Groups["left"].Value;
        Right = matched.Groups["right"].Value;
        Operator = matched.Groups["op"].Value;
        Output = matched.Groups["buffer"].Value;
    }
    public static Instruction Create(string instruction) => new(instruction);
    public string Left { get; set; }
    public string Right { get; set; }
    public string Operator { get; set; }
    public string Output { get; set; }

    public bool HasInput(string inputNode)
    {
        return Left == inputNode || Right == inputNode;
    }

    public byte Perform(int left, int right)
    {
        return Operator switch
        {
            "OR" => (left == 1 || right == 1) ? (byte)1 : (byte)0,
            "XOR" => (left == 1 ^ right == 1) ? (byte)1 : (byte)0,
            "AND" => (left == 1 && right == 1) ? (byte)1 : (byte)0,
            _ => throw new Exception("opcode not found")
        };
    }
}

public class Circuit
{
    public int BitIndex { get; set; }
    public bool IsTerminus { get; set; }
    public Instruction InputXor { get; set; } = Unassigned;
    public Instruction InputAnd { get; set; } = Unassigned;

    /// <summary>Input Xor and Cin feed this to pass to Z out</summary>
    public Instruction ZOut { get; set; } = Unassigned;

    /// <summary>Input Xor and Cin feed this to pass to Carry Out.</summary>
    public Instruction? CarryAnd { get; set; }
    /// <summary>Should be an OR function for carry or last Z in word.</summary>
    public Instruction? CarryOut { get; set; }
    public static readonly Instruction Unassigned = new("unassigned unassigned unassigned -> unassigned");

    public List<Instruction> Misc { get; set; } = [];
    public List<string> Fixed { get; set; } = [];

    public bool Validate()
    {
        if (!IsTerminus && (CarryOut?.Output.StartsWith('z') ?? false))
        {
            if (!ZOut.Output.StartsWith('z') && ZOut.Operator != "XOR")
            {
                ZOut = Misc.First(z => z.Operator == "XOR");
                Misc.Remove(ZOut);
                (ZOut.Output, CarryOut.Output) = (CarryOut.Output, ZOut.Output);
                Fixed.AddRange([CarryOut.Output, ZOut.Output]);
                Console.WriteLine($"{BitIndex}(fixed): found zout in CarryOut\n{this}");
            }
        }
        if (!IsTerminus)
        {
            if (CarryAnd == Unassigned && CarryOut == Unassigned)
            {
                (InputAnd.Output, InputXor.Output) = (InputXor.Output, InputAnd.Output);
                Fixed.AddRange([InputAnd.Output, InputXor.Output]);

                ZOut = Misc.First(x => x.Operator == "XOR");
                CarryAnd = Misc.First(x => x.Operator == "AND");
                CarryOut = Misc.First(x => x.Operator == "OR");
                Misc.RemoveAll(x => x.Equals(ZOut) || ZOut.Equals(CarryAnd) || x.Equals(CarryOut));

                Console.WriteLine($"{BitIndex}(fixed): Swapped Input outputs\n{this}");
            }

            if (ZOut == Unassigned)
                if (InputAnd.Output.StartsWith('z'))
                {
                    ZOut = Misc.First(x => x.Operator == "XOR" && x.HasInput(InputXor.Output));
                    Misc.Remove(ZOut);
                    (ZOut.Output, InputAnd.Output) = (InputAnd.Output, ZOut.Output);
                    Fixed.AddRange([InputAnd.Output, ZOut.Output]);

                    Console.WriteLine($"{BitIndex}(fixed): ZOut swap with input AND\n{this}");
                }
                else if (CarryAnd?.Output.StartsWith('z') ?? false)
                {
                    ZOut = Misc.First(x => x.Operator == "XOR");
                    Misc.Remove(ZOut);
                    (CarryAnd.Output, ZOut.Output) = (ZOut.Output, CarryAnd.Output);
                    Fixed.AddRange([ZOut.Output, CarryAnd.Output]);

                    Console.WriteLine($"{BitIndex}(fixed): ZOut swap with carry AND\n{this}");
                }

            // Console.WriteLine(this);
        }
        return true;
    }

    public override string ToString()
    {
        var cIn = ZOut.Left == InputXor.Output ? ZOut.Right : ZOut.Left;
        var (xIn, yIn) = InputAnd.Left.StartsWith('x') ? (InputAnd.Left, InputAnd.Right) : (InputAnd.Right, InputAnd.Left);
        var output = $"Stage {BitIndex}:\n\n";
        output += BitIndex > 0 ? $"-{cIn}-----\n" : "\n";
        output += $"-{xIn}-     {ZOut.Operator}-{ZOut.Output}\n";
        output += $"     XOR-{(BitIndex == 0 ? ZOut.Output : InputXor.Output)}\n";
        output += $"-{yIn}-     {CarryAnd?.Operator}--\n";
        output += BitIndex > 0 ? $"      {cIn}-    |-\n" : "\n";
        output += $"-{xIn}-            {CarryOut?.Operator}-{CarryOut?.Output}\n";
        output += $"     AND-{InputAnd.Output}-----\n";
        output += $"-{yIn}-\n\n";

        return output;
    }
}

