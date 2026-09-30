namespace laba_5.Models;

public sealed class CaramelCandy : Candy
{
    public CaramelCandy(
        string name,
        double weightGrams,
        double sugarContentPercent,
        string filling)
        : base(name, weightGrams, sugarContentPercent, filling)
    {
    }

    protected override string Description => $"карамель, вкус {Filling}";
}
