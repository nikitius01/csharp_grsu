using System.Globalization;
using laba_2.Application.Commands;
using laba_2.Domain.Enums;
using laba_2.Domain.Interfaces;

namespace laba_2.Application.Services;

public sealed class CommandParser : ICommandParser
{
    public GameCommand Parse(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            throw new ArgumentException("Команда не может быть пустой.", nameof(line));

        var parts = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        var command = parts[0].ToUpperInvariant();

        return command switch
        {
            "P" when parts.Length == 1 => new GameCommand(CommandType.PrintState),
            "M" => new GameCommand(CommandType.MouseMove, ParseValue(parts, line)),
            "C" => new GameCommand(CommandType.CatMove, ParseValue(parts, line)),
            _ => throw new FormatException($"Некорректная команда: {line}")
        };
    }

    private static int ParseValue(string[] parts, string originalLine)
    {
        if (parts.Length != 2 || !int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            throw new FormatException($"Некорректная команда: {originalLine}");

        return value;
    }
}
