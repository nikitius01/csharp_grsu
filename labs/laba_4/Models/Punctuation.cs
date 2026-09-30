namespace laba_4.Models;

public sealed class Punctuation
{
    public Punctuation(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
