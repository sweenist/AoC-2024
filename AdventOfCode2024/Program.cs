using AdventOfCode2024.Days;

var day = args[0];
var part = args[1];

switch (day)
{
    case "1":
        var solver = new Day1();
        if (part == "1")
        {
            solver.Part1();
        }
        else
            throw new ArgumentException($"Part {part} not defined.");
        break;
    default:
        throw new NotImplementedException($"no day matching {day} yet");
}
