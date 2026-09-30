using laba_5.Interfaces;

namespace laba_5.Models;

public abstract class Sweet : IGiftItem
{
    protected Sweet(string name, double weightGrams, double sugarContentPercent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (weightGrams <= 0)
            throw new ArgumentOutOfRangeException(nameof(weightGrams), "Вес должен быть положительным.");

        if (sugarContentPercent is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(sugarContentPercent),
                "Содержание сахара должно быть от 0 до 100 процентов.");

        Name = name;
        WeightGrams = weightGrams;
        SugarContentPercent = sugarContentPercent;
    }

    public string Name { get; }

    public double WeightGrams { get; }

    public double SugarContentPercent { get; }

    protected abstract string Description { get; }

    public override string ToString() =>
        $"{Name}: {Description}, {WeightGrams:g} г, сахар {SugarContentPercent:g}%";
}
