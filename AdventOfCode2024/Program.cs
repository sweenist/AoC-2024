using AdventOfCode2024.Days;

using CommandLine;

Parser.Default.ParseArguments<Options>(args).WithParsed(opt =>
{
    var isDayValid = opt.Day > 0 && opt.Day <= 25;

    var isPartValid = opt.Part == 1 || opt.Part == 2;

    if (!(isDayValid && isPartValid))
        throw new ArgumentException($@"
    Usage: dotnet run --project AdventOfCode2024.csproj day part [-e|--example] [-p|--profile]
where 
    d is a number (1-25).
    p is a number (1-2).
    useExample - an optional flag for running example problem
    profile - an optional flag for profiling solutions

Recieved args: {string.Join(' ', args)}
");

    var fullyQualifiedClassName = $"{nameof(AdventOfCode2024)}.{nameof(AdventOfCode2024.Days)}.Day{opt.Day}";
    var dayType = Type.GetType(fullyQualifiedClassName)
        ?? throw new Exception($"Cannot derive type of {fullyQualifiedClassName}");

    Console.ForegroundColor = ConsoleColor.Yellow;
    IDay dayInstance = Activator.CreateInstance(dayType!, args: opt.UseExample) as IDay
        ?? throw new Exception($"Cannot instantiate {dayType?.Name}");

    Console.ForegroundColor = ConsoleColor.DarkMagenta;
    Console.WriteLine(new string('-', 80));
    switch (opt.Part)
    {
        case 1:
            dayInstance.Part1();
            break;
        case 2:
            dayInstance.Part2();
            break;
        default:
            throw new Exception($"Unexpected error occurred {opt}");
    }
    Console.WriteLine(new string('-', 80));
    Console.ResetColor();

})
.WithNotParsed(errs =>
{
    Console.WriteLine(string.Join('\n', errs));
});
