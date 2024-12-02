using AdventOfCode2024.Days;

int day;
int part;
bool useExample = false;

var isDayValid = int.TryParse(args[0], out day)
    && day > 0
    && day <= 25;

var isPartValid = int.TryParse(args[1], out part)
    && (part == 1 || part == 2);

if (!(isDayValid && isPartValid))
    throw new ArgumentException($@"
    Usage: dotnet run --project AdventOfCode2024.csproj d p [useExample]
where 
    d is a number (1-25).
    p is a number (1-2).
    useExample(optional) - pass 'true' to args if opting to use the example data.
Recieved args: {string.Join(' ', args)}
");

useExample = args.Length >= 3 && args[2] == "true";

var fullyQualifiedClassName = $"{nameof(AdventOfCode2024)}.{nameof(AdventOfCode2024.Days)}.Day{day}";
var dayType = Type.GetType(fullyQualifiedClassName)
    ?? throw new Exception($"Cannot derive type of {fullyQualifiedClassName}");

Console.ForegroundColor = ConsoleColor.Yellow;
IDay dayInstance = Activator.CreateInstance(dayType!, args: useExample) as IDay
    ?? throw new Exception($"Cannot instantiate {dayType?.Name}");

Console.ForegroundColor = ConsoleColor.DarkMagenta;
Console.WriteLine(new string('-', 80));
switch (part)
{
    case 1:
        dayInstance.Part1();
        break;
    case 2:
        dayInstance.Part2();
        break;
    default:
        throw new Exception($"Unexpected error for part {part}");
}
Console.WriteLine(new string('-', 80));
Console.ResetColor();