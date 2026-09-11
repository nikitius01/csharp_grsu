using laba_2.Domain.Entities;
using laba_2.Domain.Enums;
using laba_2.Domain.Interfaces;

namespace laba_2.Application.Commands;

public sealed class GameCommand : IGameCommand
{
    public CommandType Type { get; }
    public int Value { get; }

    public GameCommand(CommandType type, int value = 0)
    {
        Type = type;
        Value = value;
    }

    public void Execute(Game game)
    {
        ArgumentNullException.ThrowIfNull(game);

        switch (Type)
        {
            case CommandType.MouseMove:
                game.MoveMouse(Value);
                break;
            case CommandType.CatMove:
                game.MoveCat(Value);
                break;
            case CommandType.PrintState:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
