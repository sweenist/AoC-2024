using System.Text.RegularExpressions;

namespace AdventOfCode2024.Days;

public class Day17 : IDay
{
    private string _example = @"Register A: 729
Register B: 0
Register C: 0

Program: 0,1,5,4,3,0";
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
        var pointer = 0;
        var outBuffer = new List<int>();

        while (pointer < instructionSet.Count)
        {
            switch (instructionSet[pointer])
            {
                case 0:
                    adv(instructionSet[++pointer]);
                    pointer++;
                    break;
                case 1:
                    bxl(instructionSet[++pointer]);
                    pointer++;
                    break;
                case 2:
                    bst(instructionSet[++pointer]);
                    pointer++;
                    break;
                case 3:
                    pointer = jnz(instructionSet[++pointer], pointer);
                    break;
                case 4:
                    pointer += 2;
                    bxc();
                    break;
                case 5:
                    outBuffer.Add(Out(instructionSet[++pointer]));
                    pointer++;
                    break;
                case 6:
                    bdv(instructionSet[++pointer]);
                    pointer++;
                    break;
                case 7:
                    cdv(instructionSet[++pointer]);
                    pointer++;
                    break;
                default:
                    throw new InvalidOperationException($"no operation associated with {pointer}");
            }

        }
        Console.WriteLine($"Output:\n{string.Join(',', outBuffer)}");
    }

    public void Part2()
    {
        var commandString = _input[^1].Split(' ')[1];
        SetRegisters();
        var instructionSet = _input[^1].Split(' ')[1].Split(',').Select(int.Parse).ToList();
        var pointer = 0;
        var outBuffer = new List<int>();

        while (pointer < instructionSet.Count)
        {
            switch (instructionSet[pointer])
            {
                case 0:
                    adv(instructionSet[++pointer]);
                    pointer++;
                    break;
                case 1:
                    bxl(instructionSet[++pointer]);
                    pointer++;
                    break;
                case 2:
                    bst(instructionSet[++pointer]);
                    pointer++;
                    break;
                case 3:
                    pointer = jnz(instructionSet[++pointer], pointer);
                    break;
                case 4:
                    pointer += 2;
                    bxc();
                    break;
                case 5:
                    outBuffer.Add(Out(instructionSet[++pointer]));
                    pointer++;
                    break;
                case 6:
                    bdv(instructionSet[++pointer]);
                    pointer++;
                    break;
                case 7:
                    cdv(instructionSet[++pointer]);
                    pointer++;
                    break;
                default:
                    throw new InvalidOperationException($"no operation associated with {pointer}");
            }

        }
    }

    private Dictionary<char, int> Registers { get; set; } = [];

    private void SetRegisters()
    {
        var pattern = @"\d+$";
        var dict = new Dictionary<char, int>();
        dict['A'] = int.Parse(Regex.Match(_input[0], pattern).Value);
        dict['B'] = int.Parse(Regex.Match(_input[1], pattern).Value);
        dict['C'] = int.Parse(Regex.Match(_input[2], pattern).Value);

        Registers = dict;
    }

    private void adv(int operand, char registerIndex = 'A')
    {
        var numerator = Registers['A'];
        if (operand <= 3 || operand == 7)
            Registers[registerIndex] = numerator / (int)Math.Pow(2, operand);
        else if (operand == 4)
            Registers[registerIndex] = 1;
        else if (operand == 5)
        {
            var divisor = (int)Math.Pow(2, Registers['B']);
            Registers[registerIndex] = numerator / divisor;
        }
        else if (operand == 6)
        {
            var divisor = (int)Math.Pow(2, Registers['C']);
            Registers[registerIndex] = numerator / divisor;
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

    private int jnz(int operand, int pointer)
    {
        if (Registers['A'] == 0) return ++pointer;
        return operand;
    }

    private void bxc()
    {
        Registers['B'] ^= Registers['C'];
    }

    private int Out(int operand)
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
}