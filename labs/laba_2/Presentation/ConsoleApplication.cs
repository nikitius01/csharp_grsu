using laba_2.Application.Services;
using laba_2.Domain.Entities;
using laba_2.Infrastructure.FileSystem;

namespace laba_2.Presentation;

public sealed class ConsoleApplication
{
    private readonly TextFileService _files;
    private readonly CommandParser _parser;

    public ConsoleApplication(TextFileService files, CommandParser parser)
    {
        _files = files;
        _parser = parser;
    }

    public void Run(string inputPath, string outputPath)
    {
        if (!File.Exists(inputPath))
            throw new FileNotFoundException($"Не найден входной файл: {inputPath}");

        var lines = _files.ReadAllLines(inputPath);
        if (lines.Length == 0)
            throw new InvalidOperationException("Входной файл пуст.");

        if (!int.TryParse(lines[0].Trim(), out var boardSize))
            throw new FormatException("Первая строка должна содержать размер поля.");

        var game = new Game(
            new Board(boardSize),
            new Player("Mouse"),
            new Player("Cat"));

        var logger = new PursuitLogger();
        var runner = new GameRunner(_parser);
        runner.Run(lines.Skip(1), game, logger);

        _files.WriteAllText(outputPath, logger.Build());
        Console.WriteLine(logger.Build());
    }
}
