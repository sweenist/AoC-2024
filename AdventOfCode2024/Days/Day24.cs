using System.Text.RegularExpressions;

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
        var buffers = _input.Where(s => s.Contains(':'))
                                  .Select(x => new KeyValuePair<string, byte?>(x.Split(':')[0], byte.Parse(x.Split(':')[1])))
                                  .Concat(_input.Where(s => s.Contains("->"))
                                    .Select(x => new KeyValuePair<string, byte?>(x.Split("->")[1].Trim(), null)))
                                  .ToDictionary();

        var instructions = _input.Where(s => s.Contains("->"))
            .OrderBy(x => x, new GateComparer(StringComparer.CurrentCulture))
            .Select(s => s.Split())
            .Select(s => (buffer1: s[0], opcode: s[1], buffer2: s[2], targetBuffer: s[4]))
            .ToList();

        Operate(instructions, buffers);
        var result = GetAddressBuffer(buffers);

        Console.WriteLine($"The z buffer produces {result}");
    }

    public void Part2()
    {
        var buffers = _input.Where(s => s.Contains(':'))
                                  .Select(x => new KeyValuePair<string, byte?>(x.Split(':')[0], byte.Parse(x.Split(':')[1])))
                                  .Concat(_input.Where(s => s.Contains("->"))
                                    .Select(x => new KeyValuePair<string, byte?>(x.Split("->")[1].Trim(), null)))
                                  .ToDictionary();

        var instructions = _input.Where(s => s.Contains("->"))
            .OrderBy(x => x, new GateComparer(StringComparer.InvariantCulture))
            .Select(s => s.Split())
            .Select(s => (buffer1: s[0], opcode: s[1], buffer2: s[2], targetBuffer: s[4]))
            .ToList();

        Operate(instructions, buffers);
        var bogus = new KeyValuePair<string, (string, string)>(string.Empty, (string.Empty, string.Empty));
        var matchedBits = AddBuffers(buffers);
        var chains = matchedBits.SelectMany((m, i) => m ? [bogus] : BackChain(buffers.Keys
                                    .Where(k => k.StartsWith('z'))
                                    .OrderBy(x => x).ToList()[i]))
                                .Where(x => x.Key != bogus.Key)
                                .Distinct().ToDictionary();

        Console.WriteLine($"There are {chains.Count} chains");

        Console.WriteLine($"Buffer swaps performed on: ");
    }

    public static void Operate(IEnumerable<(string buffer1, string opcode, string buffer2, string targetBuffer)> instructions,
        Dictionary<string, byte?> bufferValues)
    {
        var uninitiated = new Queue<(string buffer1, string opcode, string buffer2, string targetBuffer)>(instructions);


        while (uninitiated.Count > 0)
        {
            var (buffer1, opcode, buffer2, targetBuffer) = uninitiated.Dequeue();

            var found = bufferValues.TryGetValue(buffer1, out var left);
            found |= bufferValues.TryGetValue(buffer2, out var right);
            if (!found || !left.HasValue || !right.HasValue)
            {
                uninitiated.Enqueue((buffer1, opcode, buffer2, targetBuffer));
                continue;
            }

            bufferValues[targetBuffer] = Perform(left.Value, right.Value, opcode);
        }
    }

    private static byte Perform(byte left, byte right, string opcode)
    {
        return opcode switch
        {
            "OR" => (left == 1 || right == 1) ? (byte)1 : (byte)0,
            "XOR" => (left == 1 ^ right == 1) ? (byte)1 : (byte)0,
            "AND" => (left == 1 && right == 1) ? (byte)1 : (byte)0,
            _ => throw new Exception("opcode not found")
        };
    }

    private static long GetAddressBuffer(Dictionary<string, byte?> buffers, char bufferStart = 'z')
    {
        var bufferSet = buffers.Where(x => x.Key.StartsWith(bufferStart)).OrderByDescending(z => z.Key);
        var result = 0L;
        foreach (var key in bufferSet)
        {
            result <<= 1;
            result += buffers[key.Key] ?? 0;
        }

        return result;
    }

    private IEnumerable<KeyValuePair<string, (string left, string right)>> BackChain(string tail)
    {
        var pattern = @"(?<left>\w+)\s\w+\s(?<right>\w+)\s->\s(?<buffer>\w+)";
        var groups = Regex.Matches(string.Join('\n', _input), pattern);

        return Chain(groups, tail);
    }

    private static IEnumerable<KeyValuePair<string, (string left, string right)>> Chain(MatchCollection tokens, string tail)
    {
        if (tail.StartsWith('x') || tail.StartsWith('y')) yield break;

        var match = tokens.First(m => m.Groups["buffer"].Value == tail);
        var (left, right, buffer) = (match.Groups["left"].Value, match.Groups["right"].Value, match.Groups["buffer"].Value);
        Console.WriteLine($"{buffer}: left {left} right: {right}");

        yield return new KeyValuePair<string, (string left, string right)>(buffer, (left, right));
        foreach (var chain in Chain(tokens, left).Concat(Chain(tokens, right)))
            yield return chain;

    }

    private static List<bool> AddBuffers(Dictionary<string, byte?> buffers)
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