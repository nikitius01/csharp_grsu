using laba_2.Domain.Entities;
using laba_2.Domain.Enums;
using laba_2.Domain.Interfaces;

namespace laba_2.Application.Services;

public sealed class GameRunner
{
    private readonly ICommandParser _parser;

    public GameRunner(ICommandParser parser) => _parser = parser;

    public void Run(IEnumerable<string> lines, Game game, IGameLogger logger)
    {
        logger.WriteHeader();

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var command = _parser.Parse(line);
            command.Execute(game);

            if (command.Type == CommandType.PrintState)
                logger.WriteState(game);

            if (game.IsFinished)
                break;
        }

        logger.WriteFooter(game);
    }
}
