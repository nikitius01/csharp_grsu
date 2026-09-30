namespace laba_5.Models;

public abstract class Candy : Sweet
{
    protected Candy(
        string name,
        double weightGrams,
        double sugarContentPercent,
        string filling)
        : base(name, weightGrams, sugarContentPercent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filling);
        Filling = filling;
    }

    public string Filling { get; }
}
