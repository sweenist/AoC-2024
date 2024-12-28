using AdventOfCode2024.Types.Day24Types;

namespace AdventOfCode2024.Days;

public class Day24 : IDay
{
    private string _example = @"x00: 1
x01: 0
x02: 1
x03: 1
x04: 0
y00: 1
y01: 1
y02: 1
y03: 1
y04: 1

ntg XOR fgs -> mjb
y02 OR x01 -> tnw
kwq OR kpj -> z05
x00 OR x03 -> fst
tgd XOR rvg -> z01
vdt OR tnw -> bfw
bfw AND frj -> z10
ffh OR nrd -> bqk
y00 AND y03 -> djm
y03 OR y00 -> psh
bqk OR frj -> z08
tnw OR fst -> frj
gnj AND tgd -> z11
bfw XOR mjb -> z00
x03 OR x00 -> vdt
gnj AND wpb -> z02
x04 AND y00 -> kjc
djm OR pbm -> qhw
nrd AND vdt -> hwm
kjc AND fst -> rvg
y04 OR y02 -> fgs
y01 AND x02 -> pbm
ntg OR kjc -> kwq
psh XOR fgs -> tgd
qhw XOR tgd -> z09
pbm OR djm -> kpj
x03 XOR y03 -> ffh
x00 XOR y04 -> ntg
bfw OR bqk -> z06
nrd XOR fgs -> wpb
frj XOR qhw -> z04
bqk OR frj -> z07
y03 OR x01 -> nrd
hwm AND bqk -> z03
tgd XOR rvg -> z12
tnw OR pbm -> gnj";

    private readonly List<string> _input = [];

    public Day24(bool useExample = false)
    {
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            _input.AddRange(_example.Split('\n'));
        }
        else
        {
            var inputFile = $"inputData/{GetType().Name}.txt";
            using var sr = new StreamReader(inputFile);
            _input.AddRange(sr.ReadToEnd().Split('\n'));
        }
    }

    public void Part1()
    {
        var outputBuffers = _input.Where(s => s.Contains(':'))
                                  .Select(x => new KeyValuePair<string, byte>(x.Split(':')[0], byte.Parse(x.Split(':')[1])))
                                  .ToDictionary();

        var instructions = _input.Where(s => s.Contains("->"))
            .OrderBy(x => x, new GateComparer(StringComparer.CurrentCulture))
            .Select(Instruction.Create)
            .ToList();

        Operate(instructions, outputBuffers);
        var result = GetAddressBuffer(outputBuffers);
        AddBuffers(outputBuffers);

        Console.WriteLine($"The z buffer produces {result}");
    }

    public void Part2()
    {
        var buffers = _input.Where(s => s.Contains(':'))
                                  .Select(x => new KeyValuePair<string, byte>(x.Split(':')[0], byte.Parse(x.Split(':')[1])))
                                  .ToDictionary();

        var instructions = _input.Where(s => s.Contains("->"))
            .OrderBy(x => x, new GateComparer(StringComparer.InvariantCulture))
            .Select(Instruction.Create)
            .ToList();

        var adderCircuit = CombineCircuits(instructions, buffers).ToList();

        byte? carryIn = null;
        foreach (var circuit in adderCircuit)
        {
            carryIn = circuit.Execute(carryIn);
        }
        var result = string.Join("", adderCircuit.Select(x => x.ZO));
        result = string.Join("", result.Reverse());
        var fixedOutputs = adderCircuit.SelectMany(x => x.Fixed).OrderBy(x => x);

        Console.WriteLine($"result = {result}");
        Console.WriteLine($"Buffer swaps performed on: {string.Join(',', fixedOutputs)}");
    }

    public static void Operate(IEnumerable<Instruction> instructions,
        Dictionary<string, byte> bufferValues)
    {
        var uninitiated = new Queue<Instruction>(instructions);

        while (uninitiated.Count > 0)
        {
            var circuit = uninitiated.Dequeue();

            var found = bufferValues.TryGetValue(circuit.Left, out var left);
            found &= bufferValues.TryGetValue(circuit.Right, out var right);

            if (!found)
            {
                uninitiated.Enqueue(circuit);
                continue;
            }
            bufferValues[circuit.Output] = circuit.Perform(left, right);
        }
    }

    private static IEnumerable<Circuit> CombineCircuits(List<Instruction> instructions, Dictionary<string, byte> inputValues)
    {
        var maxBitNumber = instructions.Where(k => k.Output.StartsWith('z'))
            .Select(k => int.Parse(k.Output.TrimStart('z')))
            .Max();
        var carryIn = string.Empty;

        for (var i = 0; i < maxBitNumber; i++)
        {
            var inputX = $"x{i.ToString().PadLeft(2, '0')}";
            var inputY = $"y{i.ToString().PadLeft(2, '0')}";

            var circuit = new Circuit { BitIndex = i, XIn = inputValues[inputX], YIn = inputValues[inputY] };
            var inputs = instructions.Where(x => x.Left == inputX || x.Left == inputY);
            circuit.InputAnd = inputs.First(x => x.Operator == "AND");
            circuit.InputXor = inputs.First(x => x.Operator == "XOR");
            if (i == 0)
            {
                carryIn = circuit.InputAnd.Output;
                yield return circuit;
                continue;
            }

            var sumNode = circuit.InputXor.Output;
            var nextStages = instructions.Where(inst => inst.HasInput(carryIn))
                    .Concat(instructions.Where(inst => inst.HasInput(sumNode)))
                    .Distinct().ToList();
            circuit.ZOut = nextStages.FirstOrDefault(a => a.HasInput(carryIn)
                && a.HasInput(sumNode)
                && a.Operator == "XOR"
                && a.Output.StartsWith('z'), Circuit.Unassigned);
            nextStages.Remove(circuit.ZOut);

            circuit.CarryAnd = nextStages.FirstOrDefault(a => a.HasInput(carryIn)
                && a.HasInput(sumNode)
                && a.Operator == "AND", Circuit.Unassigned);
            nextStages.Remove(circuit.CarryAnd);

            circuit.Misc.AddRange(nextStages);

            circuit.CarryOut = instructions.FirstOrDefault(x => (x.HasInput(circuit.CarryAnd.Output)
                || x.HasInput(circuit.InputAnd.Output)) && x.Operator == "OR", Circuit.Unassigned);

            circuit.Validate();
            carryIn = circuit.CarryOut.Output;


            if (circuit.CarryOut == Circuit.Unassigned) Console.WriteLine($"Could not find viable Cout at index {i}");

            yield return circuit;
        }
    }

    private static long GetAddressBuffer(Dictionary<string, byte> buffers, char bufferStart = 'z')
    {
        var bufferSet = buffers.Where(x => x.Key.StartsWith(bufferStart)).OrderByDescending(z => z.Key);
        var result = 0L;
        foreach (var key in bufferSet)
        {
            result <<= 1;
            result += buffers[key.Key];
        }

        return result;
    }

    private static List<bool> AddBuffers(Dictionary<string, byte> buffers)
    {
        var originalZValue = Convert.ToString(GetAddressBuffer(buffers), 2);

        var xBufferValue = GetAddressBuffer(buffers, 'x');
        var yBufferValue = GetAddressBuffer(buffers, 'y');
        var realZValue = xBufferValue + yBufferValue;

        Console.WriteLine($"  {Convert.ToString(xBufferValue, 2)}\n +{Convert.ToString(yBufferValue, 2)}\n={Convert.ToString(realZValue, 2)}");
        Console.WriteLine($" {originalZValue} was expected");
        var indexHelpers = BuildIndexString(originalZValue.Length);
        Console.WriteLine($"\n {indexHelpers[0]}\n {indexHelpers[1]}");

        return Convert.ToString(realZValue, 2).Zip(originalZValue, (r, o) => r == o).ToList();
    }

    private static List<string> BuildIndexString(int stringLength)
    {
        var top = "";
        var bottom = "";
        while (stringLength != 0)
        {
            var b = stringLength % 10 == 0 ? 10 : stringLength % 10;
            var t = stringLength / 10 - (b == 10 ? 1 : 0);
            top += t == 0 ? string.Join("", Enumerable.Repeat(' ', b)) : string.Join("", Enumerable.Repeat(t.ToString(), b));
            _ = Enumerable.Range(0, b).Reverse().Select(x => bottom += x.ToString()).ToList();
            stringLength -= b;
        }
        return [bottom, top];
    }
}


internal class GateComparer(IComparer<string> baseComparer) : IComparer<string>
{
    private readonly IComparer<string> _baseComparer = baseComparer;

#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
    public int Compare(string x, string y)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
    {
        if (_baseComparer.Compare(x, y) == 0)
            return 0;

        if (x.StartsWith('x') && !y.StartsWith('x')) return -1;
        if (y.StartsWith('x') && !x.StartsWith('x')) return 1;
        if (x.StartsWith('y') && !y.StartsWith('y')) return -1;
        if (y.StartsWith('y') && !x.StartsWith('y')) return 1;

        return _baseComparer.Compare(x, y);
    }
}