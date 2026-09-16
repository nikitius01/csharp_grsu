using System.Text;
using System.Xml.Serialization;

namespace laba_3.Models;

public sealed class Sentence
{
    private const string NoSpaceBefore = ".,!?;:%)]}»…";
    private const string NoSpaceAfter = "([{«";

    [XmlElement("word", typeof(Word))]
    [XmlElement("punctuation", typeof(Punctuation))]
    public List<Token> Tokens { get; set; } = [];

    [XmlIgnore]
    public IEnumerable<Word> Words => Tokens.OfType<Word>();

    [XmlIgnore]
    public int WordCount => Words.Count();

    [XmlIgnore]
    public int Length => ToString().Length;

    [XmlIgnore]
    public bool IsQuestion => Tokens
        .OfType<Punctuation>()
        .Any(punctuation => punctuation.Value == "?");

    public override string ToString()
    {
        var result = new StringBuilder();
        Token? previous = null;

        foreach (var token in Tokens)
        {
            if (NeedsSpace(previous, token))
                result.Append(' ');

            result.Append(token.Value);
            previous = token;
        }

        return result.ToString();
    }

    private static bool NeedsSpace(Token? previous, Token current)
    {
        if (previous is null)
            return false;

        if (current is Punctuation && current.Value.Length == 1 && NoSpaceBefore.Contains(current.Value[0]))
            return false;

        if (previous is Punctuation && previous.Value.Length == 1 && NoSpaceAfter.Contains(previous.Value[0]))
            return false;

        return true;
    }
}
