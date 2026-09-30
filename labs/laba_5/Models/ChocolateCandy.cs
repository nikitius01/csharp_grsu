namespace laba_5.Models;

public sealed class ChocolateCandy : Candy
{
    public ChocolateCandy(
        string name,
        double weightGrams,
        double sugarContentPercent,
        string filling,
        double cocoaContentPercent)
        : base(name, weightGrams, sugarContentPercent, filling)
    {
        if (cocoaContentPercent is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(cocoaContentPercent),
                "Содержание какао должно быть от 0 до 100 процентов.");

        CocoaContentPercent = cocoaContentPercent;
    }

    public double CocoaContentPercent { get; }

    protected override string Description =>
        $"шоколадная конфета, начинка {Filling}, какао {CocoaContentPercent:g}%";
}
