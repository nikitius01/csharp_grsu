using System.Globalization;
using System.Text;

namespace laba_2;

internal static class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        try
        {
            Run(args);
        }
        catch (FileNotFoundException exception)
        {
            PrintError($"Файл не найден: {exception.FileName ?? exception.Message}");
        }
        catch (UnauthorizedAccessException)
        {
            PrintError("Нет доступа к указанному файлу.");
        }
        catch (IOException exception)
        {
            PrintError($"Ошибка при работе с файлом: {exception.Message}");
        }
        catch (Exception exception)
        {
            PrintError(exception.Message);
        }
    }

    private static void Run(string[] args)
    {
        var inputPath = args.Length > 0 ? args[0] : SelectInputFile();
        if (inputPath is null)
            return;

        var outputPath = args.Length > 1 ? args[1] : GetOutputPath(inputPath);
        var lines = File.ReadAllLines(inputPath);

        if (lines.Length == 0)
        {
            PrintError("Входной файл пуст.");
            return;
        }

        if (!int.TryParse(lines[0].Trim(), out var boardSize) || boardSize <= 0)
        {
            PrintError("Первая строка должна содержать положительный размер поля.");
            return;
        }

        var game = new Game(boardSize);
        var log = new StringBuilder()
            .AppendLine("Cat and Mouse")
            .AppendLine()
            .AppendLine("Cat Mouse  Distance")
            .AppendLine("-------------------");

        foreach (var rawLine in lines.Skip(1))
        {
            var line = rawLine.Trim();
            if (line.Length == 0)
                continue;

            if (!TryExecuteCommand(line, game, log))
            {
                PrintError($"Некорректная команда: {line}");
                return;
            }

            if (game.IsFinished)
                break;
        }

        AppendFooter(log, game);

        var result = log.ToString();
        File.WriteAllText(outputPath, result, new UTF8Encoding(false));
        Console.WriteLine(result);
    }

    private static bool TryExecuteCommand(string line, Game game, StringBuilder log)
    {
        var parts = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 1 && parts[0].Equals("P", StringComparison.OrdinalIgnoreCase))
        {
            AppendState(log, game);
            return true;
        }

        if (parts.Length != 2 ||
            !int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var steps))
            return false;

        if (parts[0].Equals("M", StringComparison.OrdinalIgnoreCase))
            game.MoveMouse(steps);
        else if (parts[0].Equals("C", StringComparison.OrdinalIgnoreCase))
            game.MoveCat(steps);
        else
            return false;

        return true;
    }

    private static void AppendState(StringBuilder log, Game game)
    {
        var cat = FormatPosition(game.CatPosition);
        var mouse = FormatPosition(game.MousePosition);
        var distance = game.GetDistance();

        if (distance.HasValue)
            log.AppendLine($"{cat,3}  {mouse,5}{distance.Value,10}");
        else
            log.AppendLine($"{cat,3}  {mouse,5}");
    }

    private static void AppendFooter(StringBuilder log, Game game)
    {
        log.AppendLine("-------------------")
            .AppendLine()
            .AppendLine()
            .AppendLine("Distance traveled:   Mouse    Cat")
            .AppendLine($"                        {game.MouseDistance,2}     {game.CatDistance,2}")
            .AppendLine()
            .AppendLine(game.CaughtAt.HasValue
                ? $"Mouse caught at:  {game.CaughtAt.Value}"
                : "Mouse evaded Cat");
    }

    private static string FormatPosition(int? position) =>
        position?.ToString(CultureInfo.InvariantCulture) ?? "??";

    private static string? SelectInputFile()
    {
        var contentDirectory = Path.Combine(Directory.GetCurrentDirectory(), "content");
        var files = Directory
            .GetFiles(contentDirectory, "*ChaseData.txt")
            .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (files.Length == 0)
        {
            PrintError("Не найдено ни одного файла *ChaseData.txt.");
            return null;
        }

        Console.WriteLine("Выберите входной файл:");
        for (var i = 0; i < files.Length; i++)
            Console.WriteLine($"{i + 1}. {Path.GetFileName(files[i])}");

        while (true)
        {
            Console.Write("Номер файла: ");
            var answer = Console.ReadLine();

            if (int.TryParse(answer, out var number) && number >= 1 && number <= files.Length)
            {
                Console.WriteLine();
                return files[number - 1];
            }

            Console.WriteLine($"Введите число от 1 до {files.Length}.");
        }
    }

    private static string GetOutputPath(string inputPath)
    {
        const string inputSuffix = "ChaseData.txt";
        var fileName = Path.GetFileName(inputPath);
        var prefix = fileName.EndsWith(inputSuffix, StringComparison.OrdinalIgnoreCase)
            ? fileName[..^inputSuffix.Length]
            : string.Empty;

        return Path.Combine(
            Path.GetDirectoryName(Path.GetFullPath(inputPath))!,
            $"{prefix}PursuitLog.txt");
    }

    private static void PrintError(string message) =>
        Console.Error.WriteLine($"Ошибка: {message}");
}
