using System.Globalization;
using System.Text;

namespace laba_2;

internal sealed class GameLog
{
    private readonly StringBuilder _text = new();

    public GameLog()
    {
        _text.AppendLine("Cat and Mouse")
            .AppendLine()
            .AppendLine("Cat Mouse  Distance")
            .AppendLine("-------------------");
    }

    public void AppendState(Game game)
    {
        var cat = FormatPosition(game.CatPosition);
        var mouse = FormatPosition(game.MousePosition);
        var distance = game.GetDistance();

        if (distance.HasValue)
            _text.AppendLine($"{cat,3}  {mouse,5}{distance.Value,10}");
        else
            _text.AppendLine($"{cat,3}  {mouse,5}");
    }

    public void AppendFooter(Game game)
    {
        _text.AppendLine("-------------------")
            .AppendLine()
            .AppendLine()
            .AppendLine("Distance traveled:   Mouse    Cat")
            .AppendLine($"                        {game.MouseDistance,2}     {game.CatDistance,2}")
            .AppendLine()
            .AppendLine(game.CaughtAt.HasValue
                ? $"Mouse caught at:  {game.CaughtAt.Value}"
                : "Mouse evaded Cat");
    }

    public string Build() => _text.ToString();

    private static string FormatPosition(int? position) =>
        position?.ToString(CultureInfo.InvariantCulture) ?? "??";
}
