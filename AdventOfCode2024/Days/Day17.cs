using System.Text.RegularExpressions;

namespace AdventOfCode2024.Days;

public class Day17 : IDay
{
    private string _example2 = @"Register A: 117440
Register B: 0
Register C: 0

Program: 0,3,5,4,3,0";


    private readonly List<string> _input = [];

    public Day17(bool useExample = false)
    {
        if (useExample)
        {
            Console.WriteLine("Using the example data");
            _input.AddRange(_example2.Split('\n'));
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
        SetRegisters();
        var instructionSet = _input[^1].Split(' ')[1].Split(',').Select(int.Parse).ToList();


        for (long i = 64; i < 512; i++)
        {
            Registers['A'] = i;
            Registers['B'] = 7L;
            Registers['C'] = 7L;
            var pointer = 0;
            var outBuffer = new List<long>();

            while (pointer < instructionSet.Count)
            {
                switch (instructionSet[pointer])
                {
                    case 0:
                        adv(instructionSet[++pointer]);
                        pointer++;
                        PrintRegisters("adv");
                        break;
                    case 1:
                        bxl((int)instructionSet[++pointer]);
                        pointer++;
                        PrintRegisters("bxl");
                        break;
                    case 2:
                        bst((int)instructionSet[++pointer]);
                        pointer++;
                        break;
                    case 3:
                        pointer = jnz(instructionSet[++pointer], pointer);
                        break;
                    case 4:
                        pointer += 2;
                        bxc();
                        PrintRegisters("bxc");
                        break;
                    case 5:
                        outBuffer.Add(Out((int)instructionSet[++pointer]));
                        pointer++;
                        PrintRegisters("out");
                        break;
                    case 6:
                        bdv(instructionSet[++pointer]);
                        pointer++;
                        break;
                    case 7:
                        cdv(instructionSet[++pointer]);
                        pointer++;
                        PrintRegisters("cdv");
                        break;
                    default:
                        throw new InvalidOperationException($"no operation associated with {pointer}");
                }

            }
            if (string.Join(',', outBuffer).EndsWith("5,3,0"))
                Console.WriteLine($"Output: {i} {string.Join(',', outBuffer)} {Convert.ToString(i, 2)} o:{Convert.ToString(i, 8)}");
        }
    }

    public void Part2()
    {
        var commandString = _input[^1].Split(' ')[1];
        SetRegisters();
        var instructionSet = commandString.Split(',').Select(long.Parse).ToList();

        Registers['A'] = Solve(0L, instructionSet, 0) ?? 0L;

        var outBuffer = new List<long>();
        Console.WriteLine($"{Registers['A']} is the uncorrupted qine value");
        while (Registers['A'] != 0)
        {
            bst(4);
            bxl(5);
            cdv(5);
            bxl(6);
            adv(3);
            bxc();
            outBuffer.Add(Out(5));
        }

        Console.WriteLine($"Output:\n{string.Join(',', outBuffer)}");
    }

    private Dictionary<char, long> Registers { get; set; } = [];

    private void SetRegisters()
    {
        var pattern = @"\d+$";
        var dict = new Dictionary<char, long>
        {
            ['A'] = long.Parse(Regex.Match(_input[0], pattern).Value),
            ['B'] = long.Parse(Regex.Match(_input[1], pattern).Value),
            ['C'] = long.Parse(Regex.Match(_input[2], pattern).Value)
        };

        Registers = dict;
    }

    private long? Solve(long answer, List<long> instructionSet, int endPointer)
    {
        foreach (var aTest in Enumerable.Range(0, 8))
        {
            long b = aTest;
            var a = answer * 8 + aTest;
            var result = RunProgram(a);
            if (result.TakeLast(endPointer + 1).SequenceEqual(instructionSet.TakeLast(endPointer + 1)))
            {
                if (endPointer == instructionSet.Count - 1)
                    return a;
                var nextResult = Solve(a, instructionSet, endPointer + 1);
                if (nextResult.HasValue) return nextResult;
            }

        }
        return null;
    }

    private IEnumerable<long> RunProgram(long a)
    {
        var b = 0L;
        var c = 0L;
        while (a != 0)
        {
            b = bst2(4, a);
            b ^= 5;
            c = adv2(5, a, b);
            b ^= 6;
            a = adv2(3, a);
            b ^= c;
            yield return b % 8;
        }

    }

    private void adv(int operand, char registerIndex = 'A')
    {
        var numerator = Registers['A'];
        if (operand <= 3 || operand == 7)
            Registers[registerIndex] = numerator / (long)Math.Pow(2, operand);
        else if (operand == 4)
            Registers[registerIndex] = 1;
        else if (operand == 5)
        {
            var divisor = (long)Math.Pow(2, Registers['B']);
            Registers[registerIndex] = numerator / divisor;
        }
        else if (operand == 6)
        {
            var divisor = (long)Math.Pow(2, Registers['C']);
            Registers[registerIndex] = numerator / divisor;
        }
        else
        {
            throw new InvalidOperationException($"operand {operand} is not a valid operation");
        }
    }
    private static long adv2(int operand, long numerator, long? otherRegister = null)
    {
        if (operand <= 3 || operand == 7)
            return numerator / (long)Math.Pow(2, operand);
        else if (operand == 4)
            return 1;
        else if (operand == 5 || operand == 6)
        {
            var divisor = (long)Math.Pow(2, (double)otherRegister!);
            return numerator / divisor;
        }
        else
        {
            throw new InvalidOperationException($"operand {operand} is not a valid operation");
        }
    }

    private void bxl(int operand)
    {
        Registers['B'] ^= operand;
    }

    private void bst(int operand)
    {
        if (operand <= 3 || operand == 7)
            Registers['B'] = operand;
        else if (operand == 4)
            Registers['B'] = Registers['A'] % 8;
        else if (operand == 5)
            Registers['B'] = Registers['B'] % 8;
        else if (operand == 6)
            Registers['B'] = Registers['C'] % 8;
        else throw new InvalidOperationException($"Cannot operate with {operand}");
    }

    private long bst2(int operand, long? overrideA = null)
    {
        if (operand <= 3 || operand == 7)
            return operand;
        else if (operand == 4)
            return (overrideA ?? Registers['A']) % 8;
        else if (operand == 5)
            return Registers['B'] % 8;
        else if (operand == 6)
            return Registers['C'] % 8;
        else throw new InvalidOperationException($"Cannot operate with {operand}");
    }


    private int jnz(int operand, int pointer)
    {
        if (Registers['A'] == 0) return ++pointer;
        return operand;
    }

    private void bxc()
    {
        Registers['B'] ^= Registers['C'];
    }

    private long Out(int operand)
    {
        if (operand <= 3 || operand == 7)
            return operand;
        else if (operand == 4)
            return Registers['A'] % 8;
        else if (operand == 5)
            return Registers['B'] % 8;
        else if (operand == 6)
            return Registers['C'] % 8;
        else throw new InvalidOperationException($"Cannot operate with {operand}");
    }

    private void bdv(int operand)
    {
        adv(operand, 'B');
    }

    private void cdv(int operand)
    {
        adv(operand, 'C');
    }

    private void PrintRegisters(string caller, bool print = false)
    {
        if (!print) return;
        Console.WriteLine($"Caller: {caller}");
        Console.WriteLine($"\tA: {Registers['A']}");
        Console.WriteLine($"\tB: {Registers['B']}");
        Console.WriteLine($"\tC: {Registers['C']}\n");
    }
}