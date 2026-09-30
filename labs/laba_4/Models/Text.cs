using System.Text.RegularExpressions;

namespace laba_4.Models;

public sealed partial class Text
{
    private const string SentenceEndings = ".!?";
    private const string OpeningPunctuation = "([{«";

    public List<Sentence> Sentences { get; } = [];

    public static Text ParseFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return Parse(File.ReadAllText(path));
    }

    public static Text Parse(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var text = new Text();
        var sentence = new Sentence();
        var sentenceEnded = false;
        var straightQuoteIsOpen = false;
        var lines = source.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            foreach (Match match in TokenPattern().Matches(lines[lineIndex]))
            {
                var value = match.Value;
                var isWord = char.IsLetterOrDigit(value[0]);
                var startsQuotedFragment = value == "\"" && !straightQuoteIsOpen;

                if (sentence.Tokens.Count > 0 && sentenceEnded &&
                    (isWord || IsOpeningPunctuation(value) || startsQuotedFragment))
                {
                    text.Sentences.Add(sentence);
                    sentence = new Sentence();
                    sentenceEnded = false;
                    straightQuoteIsOpen = false;
                }

                object token = isWord
                    ? new Word(value, lineIndex + 1)
                    : new Punctuation(value);

                sentence.Tokens.Add(token);

                if (value == "\"")
                    straightQuoteIsOpen = !straightQuoteIsOpen;

                if (token is Punctuation punctuation && IsSentenceEnding(punctuation.Value))
                    sentenceEnded = true;
            }
        }

        if (sentence.Tokens.Count > 0)
            text.Sentences.Add(sentence);

        return text;
    }

    public override string ToString() => string.Join(' ', Sentences);

    private static bool IsSentenceEnding(string value) =>
        value.Length == 1 && SentenceEndings.Contains(value[0]);

    private static bool IsOpeningPunctuation(string value) =>
        value.Length == 1 && OpeningPunctuation.Contains(value[0]);

    [GeneratedRegex(@"[\p{L}\p{M}\p{N}]+(?:['’\-][\p{L}\p{M}\p{N}]+)*|[^\p{L}\p{M}\p{N}\s]",
        RegexOptions.CultureInvariant)]
    private static partial Regex TokenPattern();
}
