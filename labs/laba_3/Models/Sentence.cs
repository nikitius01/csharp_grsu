using System.Text;
using System.Xml.Serialization;

namespace laba_3.Models;

public sealed class Sentence
{
    private const string NoSpaceBefore = ".,!?;:%)]}»…”";
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
        var straightQuoteIsOpen = false;
        var previousOpensQuote = false;

        foreach (var token in Tokens)
        {
            var isStraightQuote = token is Punctuation { Value: "\"" };
            var closesStraightQuote = isStraightQuote && straightQuoteIsOpen;
            var opensQuote = token is Punctuation { Value: "“" } ||
                             isStraightQuote && !straightQuoteIsOpen;

            if (NeedsSpace(previous, token, previousOpensQuote, closesStraightQuote))
                result.Append(' ');

            result.Append(token.Value);
            previous = token;
            previousOpensQuote = opensQuote;

            if (isStraightQuote)
                straightQuoteIsOpen = !straightQuoteIsOpen;
        }

        return result.ToString();
    }

    private static bool NeedsSpace(
        Token? previous,
        Token current,
        bool previousOpensQuote,
        bool closesStraightQuote)
    {
        if (previous is null)
            return false;

        if (previousOpensQuote || closesStraightQuote)
            return false;

        if (current is Punctuation && current.Value.Length == 1 && NoSpaceBefore.Contains(current.Value[0]))
            return false;

        if (previous is Punctuation && previous.Value.Length == 1 && NoSpaceAfter.Contains(previous.Value[0]))
            return false;

        return true;
    }
}
