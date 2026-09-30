using System.Collections;
using laba_5.Interfaces;

namespace laba_5.Models;

public sealed class Gift : IEnumerable<IGiftItem>
{
    private readonly List<IGiftItem> _items = [];

    public Gift(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }

    public string Name { get; }

    public double TotalWeightGrams => _items.Sum(item => item.WeightGrams);

    public void Add(IGiftItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
    }

    public IReadOnlyList<Candy> GetCandiesSortedByWeight() =>
        _items
            .OfType<Candy>()
            .OrderBy(candy => candy.WeightGrams)
            .ToList();

    public IReadOnlyList<Candy> FindCandiesBySugar(double minimum, double maximum)
    {
        if (minimum is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(minimum),
                "Нижняя граница должна быть от 0 до 100 процентов.");

        if (maximum is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(maximum),
                "Верхняя граница должна быть от 0 до 100 процентов.");

        if (minimum > maximum)
            throw new ArgumentException("Нижняя граница не может быть больше верхней.");

        return _items
            .OfType<Candy>()
            .Where(candy => candy.SugarContentPercent >= minimum &&
                            candy.SugarContentPercent <= maximum)
            .ToList();
    }

    public IEnumerator<IGiftItem> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
