using System.Xml.Serialization;

namespace laba_3.Models;

public sealed class Word
{
    public Word()
    {
    }

    public Word(string value)
    {
        Value = value;
    }

    [XmlText]
    public string Value { get; set; } = string.Empty;

    public override string ToString() => Value;
}
