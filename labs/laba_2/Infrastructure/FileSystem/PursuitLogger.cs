using System.Globalization;
using System.Text;
using laba_2.Domain.Entities;
using laba_2.Domain.Enums;
using laba_2.Domain.Interfaces;

namespace laba_2.Infrastructure.FileSystem;

public sealed class PursuitLogger : IGameLogger
{
    private readonly StringBuilder _log = new();

    public void WriteHeader()
    {
        _log.AppendLine("Cat and Mouse");
        _log.AppendLine();
        _log.AppendLine("Cat Mouse  Distance");
        _log.AppendLine("-------------------");
    }

    public void WriteState(Game game)
    {
        var cat = FormatPosition(game.Cat);
        var mouse = FormatPosition(game.Mouse);
        var distance = game.GetDistance();

        if (distance.HasValue)
            _log.AppendLine($"{cat,3}  {mouse,5}{distance.Value,10}");
        else
            _log.AppendLine($"{cat,3}  {mouse,5}");
    }

    public void WriteFooter(Game game)
    {
        _log.AppendLine("-------------------");
        _log.AppendLine();
        _log.AppendLine();
        _log.AppendLine("Distance traveled:   Mouse    Cat");
        _log.AppendLine($"                        {game.Mouse.DistanceTraveled,2}     {game.Cat.DistanceTraveled,2}");
        _log.AppendLine();

        if (game.CaughtAt.HasValue)
            _log.AppendLine($"Mouse caught at:  {game.CaughtAt.Value}");
        else
            _log.AppendLine("Mouse evaded Cat");
    }

    public string Build() => _log.ToString();

    private static string FormatPosition(Player player) =>
        player.State == PlayerState.NotInGame
            ? "??"
            : player.Position.ToString(CultureInfo.InvariantCulture);
}
