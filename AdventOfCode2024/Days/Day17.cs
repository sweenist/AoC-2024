using System.Text.RegularExpressions;

namespace AdventOfCode2024.Days;

public class Day17 : IDay
{
    //     private string _example = @"Register A: 729
    // Register B: 0
    // Register C: 0

    // Program: 0,1,5,4,3,0";
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
                        bxl((uint)instructionSet[++pointer]);
                        pointer++;
                        PrintRegisters("bxl");
                        break;
                    case 2:
                        bst((uint)instructionSet[++pointer]);
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

                        // Console.WriteLine(Convert.ToString((long)Registers['A'], 8));
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
        var instructionSet = commandString.Split(',').Select(int.Parse).ToList();

        Registers['A'] = Solve(0L, [5, 5, 3, 0], 0).First();
        var outBuffer = new List<long>();
        Console.WriteLine($"{Registers['A']} is the uncorrupted qine value");
        while (Registers['A'] != 0)
        {
            // 2,4,1,5,7,5,1,6,0,3,4,0,5,5,3,0
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

    private static IEnumerable<long> Solve(long answer, List<int> instructionSet, int endPointer)
    {
        if (instructionSet.Count == 0)
        {
            yield return answer;
            yield break;
        }
        // 2,4,1,5,7,5,1,6,0,3,4,0,5,5,3,0
        // B = A%8;
        // B XOR 5
        // A/B => C
        // B XOR 6 => B
        // A / 8 => A
        // B XOR C => B
        foreach (var aTest in Enumerable.Range(0, 8))
        {
            long b = aTest;
            var a = (answer << 3) | b;
            Console.WriteLine($"step {aTest}: {a}");
            b = a % 8;

            Console.WriteLine($"step {aTest}: b:{b}");
            b ^= 5;

            Console.WriteLine($"step {aTest}: b:{b}");
            var c = a / (long)Math.Pow(2, b);

            Console.WriteLine($"step {aTest}: c:{c}");
            b = (b ^ 6L ^ c) % 8;
            a /= 8;

            Console.WriteLine($"step {aTest}: final b:{b}");
            if (b != instructionSet[^1])
                continue;
            foreach (var nextValue in Solve(a, instructionSet[..^1]))
                yield return nextValue;
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

    private void bxl(uint operand)
    {
        Registers['B'] ^= operand;
    }

    private void bst(uint operand)
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