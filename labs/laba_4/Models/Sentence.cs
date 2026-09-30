using System.Text;

namespace laba_4.Models;

public sealed class Sentence
{
    private const string NoSpaceBefore = ".,!?;:%)]}»…”";
    private const string NoSpaceAfter = "([{«";

    public List<object> Tokens { get; } = [];

    public IEnumerable<Word> Words => Tokens.OfType<Word>();

    public override string ToString()
    {
        var result = new StringBuilder();
        object? previous = null;
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

            result.Append(token);
            previous = token;
            previousOpensQuote = opensQuote;

            if (isStraightQuote)
                straightQuoteIsOpen = !straightQuoteIsOpen;
        }

        return result.ToString();
    }

    private static bool NeedsSpace(
        object? previous,
        object current,
        bool previousOpensQuote,
        bool closesStraightQuote)
    {
        if (previous is null || previousOpensQuote || closesStraightQuote)
            return false;

        if (current is Punctuation currentPunctuation &&
            currentPunctuation.Value.Length == 1 &&
            NoSpaceBefore.Contains(currentPunctuation.Value[0]))
            return false;

        if (previous is Punctuation previousPunctuation &&
            previousPunctuation.Value.Length == 1 &&
            NoSpaceAfter.Contains(previousPunctuation.Value[0]))
            return false;

        return true;
    }
}
