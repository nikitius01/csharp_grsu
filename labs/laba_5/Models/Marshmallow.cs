namespace laba_5.Models;

public sealed class Marshmallow : Sweet
{
    public Marshmallow(
        string name,
        double weightGrams,
        double sugarContentPercent,
        string flavor)
        : base(name, weightGrams, sugarContentPercent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(flavor);
        Flavor = flavor;
    }

    public string Flavor { get; }

    protected override string Description => $"зефир, вкус {Flavor}";
}
