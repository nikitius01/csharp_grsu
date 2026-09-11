using System.Text;
using laba_2.Presentation;
using laba_2.Application.Services;
using laba_2.Infrastructure.FileSystem;

namespace laba_2;

internal static class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        try
        {
            var input = args.Length > 0 ? args[0] : SelectInputFile();
            var output = args.Length > 1 ? args[1] : GetOutputPath(input);

            var application = new ConsoleApplication(
                new TextFileService(),
                new CommandParser());

            application.Run(input, output);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Ошибка: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static string SelectInputFile()
    {
        var contentDirectory = Path.Combine(Directory.GetCurrentDirectory(), "content");
        var files = Directory
            .GetFiles(contentDirectory, "*ChaseData.txt")
            .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (files.Length == 0)
            throw new FileNotFoundException("Не найдено ни одного файла *ChaseData.txt.");

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
}
